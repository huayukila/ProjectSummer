using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Gaming.PowerUp;
using Math;
using Unity.VisualScripting.Antlr3.Runtime;

namespace Character
{
    [RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
    public class Player : Character
    {
        private enum State
        {
            None = 0,
            Fine,
            Dead,
            Invincible
        }
        private struct PlayerSilkData
        {
            public int SilkCount;                   // �v���C���[�������Ă�����̎��̐�
            public GameObject SilkRenderer;         // �v���C���[�������Ă�����̎�����ʂɕ\������GameObject
        }
        private ColorCheck mColorCheck;                     // �J���[�`�F�b�N�R���|�l���g
        //TODO �����ɂ���
        private InputAction mBoostAction;                   // �v���C���[�̃u�[�X�g����
        private InputAction mRotateAction;                  // �v���C���[�̉�]����
        private PlayerInput mPlayerInput;                   // playerInputAsset
        private State mState;                               // �v���C���[�̃X�e�[�^�X
        private float mColliderOffset;                      // �v���C���[�R���C�_�[�̒����i�����`�j
        private float mCurrentMoveSpeed;                    // �v���C���[�̌��ݑ��x
        private float mMoveSpeedCoefficient;                // �v���C���[�̈ړ����x�̌W��
        private Rigidbody mRigidbody;                       // �v���C���[��Rigidbody
        private Vector3 mRotateDirection;                   // �v���C���[�̉�]����
        private Timer mBoostCoolDownTimer = null;           // �u�[�X�g�^�C�}�[�i�B��d�l�j
        private float mBoostDurationTime;                   // �u�[�X�g�������ԁi�B��d�l�j
        private bool mIsBoosting = false;                    // �u�[�X�g���Ă��邩�̃t���O
        private SpriteRenderer mImageSpriteRenderer;        // �v���C���[�摜��SpriteRenderer
        private PlayerAnim mAnim;
        private DropPointControl mDropPointControl;         // �v���C���[��DropPointControl
        private PlayerParticleSystemControl mParticleSystemControl;

        //TODO refactorying
        private int mID = -1;                               // �v���C���[ID
        private Color mColor;                               // �v���C���[�̗̈�̐F                          
        private PlayerSilkData mSilkData;
        private float mBoostCoefficient;
        //TODO �e�X�g�p
        public Sprite[] silkCountSprites;

        private void Awake()
        {
            // ����������
            Init();
        }
        private void Start()
        {
            GetComponent<DropPointControl>()?.Init();
            transform.forward = Global.PLAYER_DEFAULT_FORWARD[mID - 1];
            mParticleSystemControl.Play();
        }
        private void Update()
        {
            // �v���C���[�摜�������Ɠ��������Ɍ������Ƃɂ���
            mImageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
            // �v���C���[���u�ʏ�v��Ԃ���Ȃ��ƌ�قǂ̏��������s���Ȃ�
            if (mState != State.Fine)
            {
                return;
            }
            // �ʏ��Ԃ̏���
            UpdateFine();
        }

        private void FixedUpdate()
        {
            // �v���C���[���u�ʏ�v��Ԃ���Ȃ��Ɠ����Ɋւ��鏈�������s���Ȃ�
            if (mState != State.Fine)
            {
                return;
            }
            // �v���C���[�̓���
            PlayerMovement();
            // �v���C���[�̉�]
            PlayerRotation();
        }


        /// <summary>
        /// �Փ˂��������Ƃ���������
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            // ���S�����v���C���[�͋��̖Ԃ������Ă�����
            if (mSilkData.SilkCount > 0)
            {
                DropSilkEvent dropSilkEvent = new DropSilkEvent()
                {
                    pos = transform.position,
                };
                // ���̎��̃h���b�v�ꏊ��ݒ肷��
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
            // �Փ˂����玀�S��Ԃɐݒ肷��
            SetDeadStatus();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Contains("DropPoint"))
            {
                // ������DropPoint�ȊO��DropPoint�ɓ���������
                if (other.gameObject.tag.Contains(mID.ToString()) == false)
                {
                    if (mSilkData.SilkCount > 0)
                    {
                        DropSilkEvent dropSilkEvent = new DropSilkEvent()
                        {
                            pos = transform.position,
                        };
                        TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
                    }
                    SetDeadStatus();
                }
            }
        }
        #region InternalLogic

        private void UpdateFine()
        {

            // �v���C���[�摜�̌�����ς���
            FlipCharacterImage();
            // �v���C���[�C���v�b�g���擾����
            Vector2 rotateInput = mRotateAction.ReadValue<Vector2>();
            // ��]���������߂�
            mRotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);
            // �v���C���[������Ƃ���̒n�ʂ̐F���`�F�b�N����
            CheckGroundColor();
            // �̈��`�悵�Ă݂�
            TryPaintArea();
            //TODO �u�[�X�g�i�B��d�l�j
            if (mBoostCoolDownTimer != null)
            {
                mBoostDurationTime -= Time.deltaTime;
                if (mBoostDurationTime <= 0.0f)
                {
                    mBoostCoefficient = 1f;
                }
                if (mBoostCoolDownTimer.IsTimerFinished())
                {
                    mBoostCoolDownTimer = null;
                }
            }
        }

        /// <summary>
        /// �v���C���[�̃v���p�e�B������������
        /// </summary>
        protected override void Init()
        {
            mRigidbody = GetComponent<Rigidbody>();
            mColorCheck = GetComponent<ColorCheck>();
            mPlayerInput = GetComponent<PlayerInput>();
            // �v���C���[�����̉摜�̃����_���[���擾����
            mImageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            mParticleSystemControl = gameObject.GetComponent<PlayerParticleSystemControl>();
            // DropPointControl�R���|�l���g��ǉ�����
            mDropPointControl = gameObject.AddComponent<DropPointControl>();
            // PlayerAnim�R���|�l���g��ǉ�����
            mAnim = gameObject.AddComponent<PlayerAnim>();

            mCurrentMoveSpeed = 0.0f;
            mColorCheck.layerMask = LayerMask.GetMask("Ground");
            mMoveSpeedCoefficient = 1.0f;
            mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            mAcceleration = Global.PLAYER_ACCELERATION;
            mStatus.mRotationSpeed = Global.PLAYER_ROTATION_SPEED;
            mState = State.Fine;
            mColliderOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
            mBoostDurationTime = Global.BOOST_DURATION_TIME;
            // �\�����ʂ�ϊ�����
            mImageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
            mSilkData.SilkCount = 0;
            mSilkData.SilkRenderer = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
            mSilkData.SilkRenderer.transform.parent = mImageSpriteRenderer.transform;
            mSilkData.SilkRenderer.SetActive(false);
            mBoostCoefficient = 1f;

        }

        /// <summary>
        /// �v���C���[�̈ړ��𐧌䂷��
        /// </summary>
        private void PlayerMovement()
        {
            // �����^�������āA�ő呬�x�܂ŉ�������
            mCurrentMoveSpeed = mCurrentMoveSpeed >= mStatus.mMaxMoveSpeed ? mStatus.mMaxMoveSpeed : mCurrentMoveSpeed + mAcceleration;
            // �O�����̈ړ�������
            Vector3 moveDirection = transform.forward * mCurrentMoveSpeed * mMoveSpeedCoefficient * mBoostCoefficient;
            mRigidbody.velocity = moveDirection;
        }

        /// <summary>
        /// �v���C���[�̉�]�𐧌䂷��
        /// </summary>
        private void PlayerRotation()
        {
            // �������͂��擾����
            if (mRotateDirection != Vector3.zero)
            {
                // ���͂��ꂽ�����։�]����
                Quaternion rotation = Quaternion.LookRotation(mRotateDirection, Vector3.up);
                mRigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, mStatus.mRotationSpeed * Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// �L�����N�^�[�̉摜�𔽓]����֐�
        /// </summary>
        private void FlipCharacterImage()
        {
            if (transform.forward.x < 0.0f)
            {
                mImageSpriteRenderer.flipX = false;
            }
            else
            {
                mImageSpriteRenderer.flipX = true;
            }
        }

        /// <summary>
        /// �v���C���[�̎��S��Ԃ�ݒ肷��
        /// </summary>
        private void SetDeadStatus()
        {
            mAnim.StartExplosionAnim();
            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                dropCount = mSilkData.SilkCount,
                pos = transform.position
            };
            TypeEventSystem.Instance.Send(dropSilkEvent);
            PlayerRespawnEvent playerRespawnEvent = new PlayerRespawnEvent()
            {
                ID = mID
            };
            TypeEventSystem.Instance.Send(playerRespawnEvent);

            // �v���C���[�̏�Ԃ����Z�b�g����
            ResetStatus();
            // �v���C���[�̌��������Z�b�g����
            FlipCharacterImage();
            // �R���|�l���g�𖳌����ɂ���
            GetComponent<DropPointControl>().enabled = false;
            GetComponentInChildren<TrailRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            // �v���C���[�����C�x���g�����N����
            mAnim.StartRespawnAnim();
            mParticleSystemControl.Stop();
            SetPowerUpLevel();
        }

        /// <summary>
        /// �v���C���[�̃X�e�C�^�X�����Z�b�g����
        /// </summary>
        private void ResetStatus()
        {
            mRigidbody.velocity = Vector3.zero;
            mRigidbody.angularVelocity = Vector3.zero;
            mState = State.Dead;
            transform.localScale = Vector3.one;
            mCurrentMoveSpeed = 0.0f;
            mIsBoosting = false;
            mBoostDurationTime = Global.BOOST_DURATION_TIME;
            mBoostCoolDownTimer = null;
            mSilkData.SilkCount = 0;
            transform.forward = Global.PLAYER_DEFAULT_FORWARD[(mID - 1)];
            DropPointSystem.Instance.ClearDropPoints(mID);
            mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            mDropPointControl.ResetTrail();
            mSilkData.SilkRenderer.SetActive(false);
        }
        /// <summary>
        /// �n�ʂ̐F���`�F�b�N����
        /// </summary>
        private void CheckGroundColor()
        {
            // �����̗̈�ɂ�����
            if (mColorCheck.isTargetColor(Color.clear))
            {
                mMoveSpeedCoefficient = 1.0f;
            }
            // �ʂ̃v���C���[�̗̈�ɂ�����
            else if (mColorCheck.isTargetColor(mColor))
            {
                mMoveSpeedCoefficient = Global.SPEED_UP_COEFFICIENT;
            }
            // �h���Ă��Ȃ��n�ʂɂ�����
            else
            {
                mMoveSpeedCoefficient = Global.SPEED_DOWN_COEFFICIENT;
            }
        }

        /// <summary>
        /// �v���C���[�̈��`�悵�Ă݂�֐�
        /// </summary>
        private void TryPaintArea()
        {
            Vector3[] dropPoints = DropPointSystem.Instance.GetPlayerDropPoints(mID);
            // DropPoint��4�ȏ゠��Ε`��ł���
            if (dropPoints.Length >= 4)
            {
                // �v���C���[�̐擪���W
                Vector3 endPoint1 = transform.position + transform.forward * mColliderOffset;
                // �v���C���[�����O�ɃC���X�^���X������DropPoint
                Vector3 endPoint2 = dropPoints[dropPoints.Length - 1];
                // endPoint1��endPoint2�ō�����x�N�g����endPoint2�ȊO��DropPoint��擪���珇�Ԃ�2���ō�����x�N�g����������Ă��邩�ǂ������`�F�b�N����
                for (int i = 0; i < dropPoints.Length - 2; ++i)
                {
                    // ��̃x�N�g�������s������continue
                    if (VectorMath.IsParallel(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1))
                    {
                        continue;
                    }
                    // ���ꂼ��̍��W�_�Ǝ������g�ȊO�̃x�N�g���̈ʒu�֌W���v�Z����(0���傫���Ȃ�x�N�g���̍����A0��菬�����Ȃ�x�N�g���̉E���A0�Ȃ�x�N�g���̒�)
                    float pointPos1 = VectorMath.PointOfLine(dropPoints[i], endPoint2, endPoint1);
                    float pointPos2 = VectorMath.PointOfLine(dropPoints[i + 1], endPoint2, endPoint1);
                    float pointPos3 = VectorMath.PointOfLine(endPoint2, dropPoints[i], dropPoints[i + 1]);
                    float pointPos4 = VectorMath.PointOfLine(endPoint1, dropPoints[i], dropPoints[i + 1]);
                    // ��̃x�N�g����������Ă�����`�悷��
                    if (pointPos1 * pointPos2 < 0 && pointPos3 * pointPos4 < 0)
                    {
                        // ��_���v�Z����
                        Vector3 crossPoint = VectorMath.GetCrossPoint(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1);
                        // �`�悷��̈�̒��_���擾����
                        List<Vector3> verts = new List<Vector3>();
                        for (int j = i + 1; j < dropPoints.Length; j++)
                        {
                            verts.Add(dropPoints[j]);
                        }
                        verts.Add(crossPoint);
                        // �`�悷��
                        PolygonPaintManager.Instance.Paint(verts.ToArray(), mID, mColor);
                        TryCaptureObject(verts.ToArray());
                        // �S�Ă�DropPoint������
                        DropPointSystem.Instance.ClearDropPoints(mID);
                        // �K����TrailRenderer�̏�Ԃ����Z�b�g����
                        mDropPointControl.ResetTrail();
                        break;
                    }
                }
            }
        }

        private void TryCaptureObject(Vector3[] verts)
        {
            Vector3[] silkPos = GoldenSilkManager.Instance.GetOnFieldSilkPos();
            List<Vector3> caputuredSilk = new List<Vector3>();
            bool IsPickedNew = false;
            foreach (Vector3 pos in silkPos)
            {
                if (VectorMath.InPolygon(pos, verts))
                {
                    mSilkData.SilkCount++;
                    caputuredSilk.Add(pos);
                    IsPickedNew = true;

                }
            }
            SilkCapturedEvent silkCapturedEvent = new SilkCapturedEvent()
            {
                ID = mID,
                positions = caputuredSilk.ToArray()
            };
            TypeEventSystem.Instance.Send<SilkCapturedEvent>(silkCapturedEvent);
            // ���̎��̉摜��\��
            if (IsPickedNew)
            {
                AudioManager.Instance.PlayFX("SpawnFX", 0.7f);
                // �L�����N�^�[�摜�̏c�̑傫�����擾���ĉ摜�̏�ŕ\������
                mSilkData.SilkRenderer.transform.localPosition = new Vector3(-mImageSpriteRenderer.bounds.size.x / 4f, mImageSpriteRenderer.bounds.size.z * 1.2f, 0);
                mSilkData.SilkRenderer.SetActive(true);
                Transform silkCount = mSilkData.SilkRenderer.transform.GetChild(0);
                if (silkCount != null)
                {
                    silkCount.GetComponent<SpriteRenderer>().sprite = silkCountSprites[mSilkData.SilkCount];
                    silkCount.transform.localPosition = new Vector3(mImageSpriteRenderer.bounds.size.x, 0, 0);
                }
                SetPowerUpLevel();
            }
        }

        private void SetPlayerInputProperties()
        {
            mPlayerInput.defaultActionMap = name;
            mPlayerInput.neverAutoSwitchControlSchemes = true;
            mPlayerInput.SwitchCurrentActionMap(name);
            mRotateAction = mPlayerInput.actions["Rotate"];
            mBoostAction = mPlayerInput.actions["Boost"];
            mBoostAction.performed += OnBoost;
        }

        private void SetPowerUpLevel()
        {
            if (mSilkData.SilkCount > 0)
            {
                mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED + Global.POWER_UP_PARAMETER[mSilkData.SilkCount - 1].SpeedUp;
                mStatus.mRotationSpeed = Global.PLAYER_ROTATION_SPEED + Global.POWER_UP_PARAMETER[mSilkData.SilkCount - 1].RotateUp;
            }
            else
            {
                mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
                mStatus.mRotationSpeed = Global.PLAYER_ROTATION_SPEED;
            }
        }
        private void OnEnable()
        {
            mPlayerInput?.ActivateInput();
        }
        private void OnDisable()
        {
            mPlayerInput?.DeactivateInput();
        }
        private void OnDestroy()
        {
            mBoostAction.performed -= OnBoost;
        }
        // �B��d�l
        private void OnBoost(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (mState == State.Fine && mIsBoosting == false)
                {
                    mBoostCoefficient = 1.5f;
                    mCurrentMoveSpeed = mStatus.mMaxMoveSpeed;
                    mIsBoosting = true;
                    mBoostCoolDownTimer = new Timer();
                    mBoostCoolDownTimer.SetTimer(Global.BOOST_COOLDOWN_TIME,
                        () =>
                        {
                            mBoostDurationTime = Global.BOOST_DURATION_TIME;
                            mIsBoosting = false;
                        });
                }
            }
        }

        #endregion
        #region interface
        public bool IsDead() => mState == State.Dead;
        public int GetID() => mID;
        public Color GetColor() => mColor;
        public void SetProperties(int ID, Color color)
        {
            if (mID == -1)
            {
                mID = ID;
                mColor = color;
                name = "Player" + mID.ToString();
            }
            if(mID != -1)
            {
                SetPlayerInputProperties();
            }
        }

        public void StartRespawn()
        {
            if(mState == State.Dead)
            {
                transform.position = Global.PLAYER_START_POSITIONS[mID - 1];
                transform.forward = Global.PLAYER_DEFAULT_FORWARD[mID - 1];
                mState = State.Fine;
                GetComponentInChildren<TrailRenderer>().enabled = true;
                GetComponent<DropPointControl>().enabled = true;
                GetComponent<Collider>().enabled = true;
                GameObject smoke = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Smoke"), transform.position, Quaternion.identity);
                smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
                mParticleSystemControl.Play();
            }
        }
        #endregion
    }

}
