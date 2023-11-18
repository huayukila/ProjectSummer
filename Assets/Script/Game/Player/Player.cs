using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public enum Status
    {
        Fine,
        Dead,
        Invincible
    }

    [SerializeField]
    private float mMaxMoveSpeed;                         // �v���C���[�̍ő呬�x
    [Min(0.0f)][SerializeField]
    private float mAcceleration;                         // �v���C���[�̉����x
    [Min(0.0f)][SerializeField]
    private float mRotationSpeed;                        // �v���C���[�̉�]���x

    private ColorCheck mColorCheck;                      // �J���[�`�F�b�N�R���|�l���g
    //TODO �����ɂ���
    private InputAction mBoostAction;                    // �v���C���[�̃u�[�X�g����
    private InputAction mRotateAction;                   // �v���C���[�̉�]����
    private PlayerInput mPlayerInput;                    // playerInputAsset
    private Status mStatus;                            // �v���C���[�̃X�e�[�^�X
    private float mColliderOffset;                     // �v���C���[�R���C�_�[�̒����i�����`�j
    private float mCurrentMoveSpeed;                    // �v���C���[�̌��ݑ��x
    private float mMoveSpeedCoefficient;                // �v���C���[�̈ړ����x�̌W��
    private Rigidbody mRigidbody;                       // �v���C���[��Rigidbody
    private Vector3 mRotateDirection;                   // �v���C���[�̉�]����

    private Timer _mBoostCoolDown;                      // �u�[�X�g�^�C�}�[�i�B��d�l�j
    private float _boostDurationTime;                   // �u�[�X�g�������ԁi�B��d�l�j
    private bool _isBoosting = false;                   // �u�[�X�g���Ă��邩�̃t���O
    private SpriteRenderer _mImageSpriteRenderer;       // �v���C���[�摜��SpriteRenderer
    private PlayerAnim _mAnim;
    private CharacterParticleSystem mParticleSystem;
    //TODO
    private GameObject _mHasSilkImage;                  // ���̎��������Ă��邱�Ƃ������摜

    //TODO refactorying
    private int _mID;                                   // �v���C���[ID
    private Color _mColor;                              // �v���C���[�̗̈�̐F
    private DropPointControl _mDropPointControl;        // �v���C���[��DropPointControl

    public bool hasSilk { get; private set; }         // �v���C���[�����̎��������Ă��邩�̃t���O
    //TODO
    private int _mSilkCount;                            // �v���C���[�������Ă�����̎��̐�

    private void Awake()
    {
        // ����������
        Init();
    }
    private void Update()
    {
        // �v���C���[�摜�������Ɠ��������Ɍ������Ƃɂ���
        _mImageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // �v���C���[���u�ʏ�v��Ԃ���Ȃ��ƌ�قǂ̏��������s���Ȃ�
        if (mStatus != Status.Fine)
        {
            return;
        }
        // �ʏ��Ԃ̏���
        UpdateFine();
    }

    private void FixedUpdate()
    {
        // �v���C���[���u�ʏ�v��Ԃ���Ȃ��Ɠ����Ɋւ��鏈�������s���Ȃ�
        if (mStatus != Status.Fine)
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
        if (hasSilk == true)
        {
            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                pos = transform.position,
                dropMode = DropMode.Standard
            };
            // ���̎��̃h���b�v�ꏊ��ݒ肷��

            // �ǂɂԂ�������
            if (collision.gameObject.CompareTag("Wall"))
            {
                dropSilkEvent.dropMode = DropMode.Edge; 
            }
            TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
        }
        // �Փ˂����玀�S��Ԃɐݒ肷��
        SetDeadStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Contains("DropPoint"))
        {
            if(other.gameObject.tag.Contains(_mID.ToString()) == false)
            {
                if (hasSilk == true)
                {
                    DropSilkEvent dropSilkEvent = new DropSilkEvent()
                    {
                        pos = transform.position,
                        dropMode = DropMode.Standard
                    };
                    TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
                }
                SetDeadStatus();
            }
        }
        // ���̎��ɓ���������
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            if (_mSilkCount == 3)
                return;
            //TODO (3�܂Œǉ�)

            // ���̎��̉摜��\��
            hasSilk = true;
            _mHasSilkImage.SetActive(true);
            _mHasSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
            TypeEventSystem.Instance.Send<PickSilkEvent>();
            _mSilkCount++;
        }
        // �S�[���ɓ���������
        if (other.gameObject.CompareTag("Goal"))
        {
            // ���������̎��������Ă�����
            if (hasSilk == true)
            {
                AddScoreEvent addScoreEvent = new AddScoreEvent()
                {
                    playerID = _mID,
                    silkCount = _mSilkCount
                };

                TypeEventSystem.Instance.Send<AddScoreEvent>(addScoreEvent);
                hasSilk = false;
                _mHasSilkImage.SetActive(false);
                _mSilkCount = 0;
            }
        }
    }


    private void UpdateFine()
    {

        // �v���C���[�摜�̌�����ς���
        FlipCharacterImage();
        // ���̖Ԃ̂������Ă���΁A�v���C���[�摜�̏�ɕ\������
        if (hasSilk == true)
        {
            // �L�����N�^�[�摜�̏c�̑傫�����擾���ĉ摜�̏�ŕ\������
            _mHasSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
        }

        // �v���C���[�C���v�b�g���擾����
        Vector2 rotateInput = mRotateAction.ReadValue<Vector2>();
        // ��]���������߂�
        mRotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);

        // �v���C���[������Ƃ���̒n�ʂ̐F���`�F�b�N����
        CheckGroundColor();

        // �̈�͕`��ł��邩�ǂ������`�F�b�N����
        CheckCanPaint();

        //TODO �u�[�X�g�i�B��d�l�j
        if(_mBoostCoolDown != null)
        {
            _boostDurationTime -= Time.deltaTime;
            if(_boostDurationTime <= 0.0f)
            {
                mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            }
            if(_mBoostCoolDown.IsTimerFinished())
            {
                _mBoostCoolDown = null;
            }
        }

        mParticleSystem.Play();
    }


    /// <summary>
    /// �v���C���[�̃v���p�e�B������������
    /// </summary>
    private void Init()
    {
        mCurrentMoveSpeed = 0.0f;
        mRigidbody = GetComponent<Rigidbody>();
        mColorCheck = GetComponent<ColorCheck>();
        mColorCheck.layerMask = LayerMask.GetMask("Ground");
        mMoveSpeedCoefficient = 1.0f;
        mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        mAcceleration = Global.PLAYER_ACCELERATION;
        mRotationSpeed = Global.PLAYER_ROTATION_SPEED;

        mPlayerInput = GetComponent<PlayerInput>();
        mRotateAction = mPlayerInput.actions["Rotate"];
        mBoostAction = mPlayerInput.actions["Boost"];
        mBoostAction.performed += OnBoost;
        mStatus = Status.Fine;
        mColliderOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _boostDurationTime = Global.BOOST_DURATION_TIME;

        _mHasSilkImage = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
        _mHasSilkImage.SetActive(false);

        // �v���C���[�����̉摜�̃����_���[���擾����
        _mImageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // �\�����ʂ�ϊ�����
        _mImageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
        _mSilkCount = 0;
        _mID = -1;

        // DropPointControl�R���|�l���g��ǉ�����
        _mDropPointControl = gameObject.AddComponent<DropPointControl>();
        // PlayerAnim�R���|�l���g��ǉ�����
        _mAnim = gameObject.AddComponent<PlayerAnim>();
        mParticleSystem = gameObject.AddComponent<CharacterParticleSystem>();

    }
    /// <summary>
    /// �v���C���[�̈ړ��𐧌䂷��
    /// </summary>
    private void PlayerMovement()
    {
        // �����^�������āA�ő呬�x�܂ŉ�������
        mCurrentMoveSpeed = mCurrentMoveSpeed >= mMaxMoveSpeed ? mMaxMoveSpeed : mCurrentMoveSpeed + mAcceleration;
        // �O�����̈ړ�������
        Vector3 moveDirection = transform.forward * mCurrentMoveSpeed * mMoveSpeedCoefficient;
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
            RotateRigidbody(rotation);
        }
    }

    /// <summary>
    /// �v���C���[�̃��W�b�h�{�f�B����]������
    /// </summary>
    /// <param name="quaternion">��]�������</param>
    private void RotateRigidbody(Quaternion quaternion)
    {
        mRigidbody.rotation = Quaternion.Slerp(transform.rotation, quaternion, mRotationSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// �L�����N�^�[�̉摜�𔽓]����֐�
    /// </summary>
    private void FlipCharacterImage()
    {
        if (transform.forward.x < 0.0f)
        {
            _mImageSpriteRenderer.flipX = false;
        }
        else
        {
            _mImageSpriteRenderer.flipX = true;
        }

    }
    /// <summary>
    /// �v���C���[�̎��S��Ԃ�ݒ肷��
    /// </summary>
    private void SetDeadStatus()
    {
        _mAnim.Play(AnimType.Explode);
        // �v���C���[�̏�Ԃ����Z�b�g����
        ResetStatus();
        // �v���C���[�̌��������Z�b�g����
        FlipCharacterImage();
        // �R���|�l���g�𖳌����ɂ���
        GetComponent<DropPointControl>().enabled = false;
        GetComponentInChildren<TrailRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // �v���C���[�����C�x���g�����N����
        PlayerRespawnEvent playerRespawnEvent = new PlayerRespawnEvent()
        {
            player = gameObject
        };
        TypeEventSystem.Instance.Send<PlayerRespawnEvent>(playerRespawnEvent);

        _mHasSilkImage.SetActive(false);

        _mAnim.Play(AnimType.Respawn);
        mParticleSystem.Stop();

    }

    /// <summary>
    /// �v���C���[�̃X�e�C�^�X�����Z�b�g����
    /// </summary>
    private void ResetStatus()
    {
        mRigidbody.velocity = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;
        mStatus = Status.Dead;
        transform.localScale = Vector3.one;
        mCurrentMoveSpeed = 0.0f;
        hasSilk = false;
        _isBoosting = false;
        _boostDurationTime = Global.BOOST_DURATION_TIME;
        _mBoostCoolDown = null;
        _mSilkCount = 0;
        transform.forward = Global.PLAYER_DEFAULT_FORWARD[(_mID - 1)];
        DropPointSystem.Instance.ClearDropPoints(_mID);
        mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        _mDropPointControl.ResetTrail();

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
        else if (mColorCheck.isTargetColor(_mColor))
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
    private void CheckCanPaint()
    {
        Vector3[] dropPoints = DropPointSystem.Instance.GetPlayerDropPoints(_mID);
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
                    PolygonPaintManager.Instance.Paint(verts.ToArray(), _mID, _mColor);
                    // �S�Ă�DropPoint������
                    DropPointSystem.Instance.ClearDropPoints(_mID);
                    // �K����TrailRenderer�̏�Ԃ����Z�b�g����
                    _mDropPointControl.ResetTrail();
                    break;
                }
            }
        }

    }

    public int GetID() => _mID;
    public Color GetColor() => _mColor;
    public float GetCurrentMoveSpeed() => mCurrentMoveSpeed;

    //todo �A�N�Z�X�C���q�̕ύX�\��
    public void SetStatus(Status status)
    {
        mStatus = status;
    }

    public void SetProperties(int ID, Color color)
    {
        if (_mID == -1)
        {
            _mID = ID;
            _mColor = color;
            if (_mID <= Global.PLAYER_START_POSITIONS.Length)
            {
                _mAnim.Init();
            }
            name = "Player" + _mID.ToString();
        }
    }


    private void OnEnable()
    {
        mRotateAction.Enable();
        mBoostAction.Enable();
    }
    private void OnDisable()
    {
        mRotateAction.Disable();
        mBoostAction.Disable();
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
            if(mStatus == Status.Fine && _isBoosting == false)
            {
                mMaxMoveSpeed *= 1.5f;
                mCurrentMoveSpeed = mMaxMoveSpeed;
                _isBoosting = true;
                _mBoostCoolDown = new Timer();
                _mBoostCoolDown.SetTimer(Global.BOOST_COOLDOWN_TIME,
                    () =>
                    {
                        _boostDurationTime = Global.BOOST_DURATION_TIME;
                        _isBoosting = false;
                    });
            }
        }
    }

}
