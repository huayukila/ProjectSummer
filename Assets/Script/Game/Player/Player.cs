using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public enum PlayerStatus
{
    Fine,
    Dead,
    Invincible
}
[RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float maxMoveSpeed;                         // �v���C���[�̍ő呬�x
    [Min(0.0f)][SerializeField]
    private float acceleration;                         // �v���C���[�̉����x
    [Min(0.0f)][SerializeField]
    private float rotationSpeed;                        // �v���C���[�̉�]���x

    private ColorCheck colorCheck;                      // �J���[�`�F�b�N�R���|�l���g
    private DropSilkEvent dropSilkEvent;                // ���̖Ԃ𗎂Ƃ��C�x���g
    private PickSilkEvent pickSilkEvent;                // ���̖Ԃ��E���C�x���g
    //TODO �����ɂ���
    private InputAction boostAction;                    // �v���C���[�̃u�[�X�g����
    private InputAction rotateAction;                   // �v���C���[�̉�]����
    private PlayerInput playerInput;                    // playerInputAsset
    private PlayerStatus _mStatus;                      // �v���C���[�̃X�e�[�^�X
    private float _mColliderOffset;                     // �v���C���[�R���C�_�[�̒����i�����`�j
    private float _currentMoveSpeed;                    // �v���C���[�̌��ݑ��x
    private float _moveSpeedCoefficient;                // �v���C���[�̈ړ����x�̌W��
    private Rigidbody _rigidbody;                       // �v���C���[��Rigidbody
    private Vector3 _rotateDirection;                   // �v���C���[�̉�]����

    //TODO �R�����g�t��
    private GameObject _particleObject;                 // �p�[�e�B�N���V�X�e���������Ă���I�u�W�F�N�g
    private ParticleSystem _mParticleSystem;            // �p�[�e�B�N���V�X�e��
    private ParticleSystem.MainModule _pSMain;          // �p�[�e�B�N���V�X�e���̃v���p�e�B��ݒ肷�邽�߂̕ϐ�
    private Timer _mBoostCoolDown;                      // �u�[�X�g�^�C�}�[�i�B��d�l�j
    private float _boostDurationTime;                   // �u�[�X�g�������ԁi�B��d�l�j
    private bool _isBoosting = false;                   // �u�[�X�g���Ă��邩�̃t���O
    private SpriteRenderer _mImageSpriteRenderer;       // �v���C���[�摜��SpriteRenderer
    private GameObject _mShadow;                        // �v���C���[���������鎞�̉e
    private SpriteRenderer _mShadowSpriteRenderer;      // �v���C���[���������鎞�̉e��SpriteRenderer
    private GameObject _bigSpider;                      // �v���C���[���������邷�鎞�̑傫���w�
    private Timer _respawnAnimationTimer;               // �v���C���[�����p�^�C�}�[
    private LineRenderer _mBigSpiderLineRenderer;       // �v���C���[�������鎞�̋󒆓������鎞�Ɍq�����Ă��鎅

    private GameObject _explosionPrefab;                // �����A�j���[�V�����v���n�u  
    //todo
    private GameObject _mGotSilkImage;                  // ���̎��������Ă��邱�Ƃ������摜

    //todo refactorying
    private Vector3 _mRespawnPos;                       // ��������ꏊ
    private int _mID;                                   // �v���C���[ID
    private Color _mColor;                              // �v���C���[�̗̈�̐F
    private DropPointControl _mDropPointControl;        // �v���C���[��DropPointControl

    public bool IsGotSilk { get; private set; }         // �v���C���[�����̎��������Ă��邩�̃t���O
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

        // �ʏ��Ԃ���Ȃ��ƕ����A�j���[�V��������������
        if (_mStatus != PlayerStatus.Fine || _respawnAnimationTimer != null)
        {
            // �����A�j���[�V�����̏���
            UpdateRespawnAnimation();
            if (_respawnAnimationTimer.IsTimerFinished())
            {
                ResetRespawnAnimation();
            }
            return;
        }
        // �ʏ��Ԃ̏���
        UpdateFine();


    }

    private void FixedUpdate()
    {
        // �v���C���[���u�ʏ�v��Ԃ���Ȃ��Ɠ����Ɋւ���֐������s���Ȃ�
        if (_mStatus != PlayerStatus.Fine)
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
        if (IsGotSilk == true)
        {
            // ���̎��̃h���b�v�ꏊ��ݒ肷��
            dropSilkEvent.pos = transform.position;
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
                if (IsGotSilk)
                {
                    dropSilkEvent.pos = transform.position;
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
            IsGotSilk = true;
            _mGotSilkImage.SetActive(true);
            _mGotSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
            _mSilkCount++;
        }
        // �S�[���ɓ���������
        if (other.gameObject.CompareTag("Goal"))
        {
            // ���������̎��������Ă�����
            if (IsGotSilk)
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = _mID,
                    silkCount = _mSilkCount
                };
                TypeEventSystem.Instance.Send<AddScoreEvent>(AddScoreEvent);
                IsGotSilk = false;
                _mGotSilkImage.SetActive(false);
                _mSilkCount = 0;
            }
        }
    }


    private void UpdateFine()
    {
        // �G�t�F�N�g�X�^�[�g
        if (_mParticleSystem.isStopped)
        {
            _mParticleSystem.Play();
        }
        // �G�t�F�N�g�̍X�V
        {
            _pSMain.startSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 2.0f;
            _pSMain.simulationSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 4.0f + 1.0f;
            _pSMain.startLifetime = _pSMain.simulationSpeed * 0.5f;

        }

        // �v���C���[�摜�̌�����ς���
        FlipCharacterImage();
        // ���̖Ԃ̂������Ă���΁A�v���C���[�摜�̏�ɕ\������
        if (IsGotSilk == true)
        {
            // �L�����N�^�[�摜�̏c�̑傫�����擾���ĉ摜�̏�ŕ\������
            _mGotSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
        }

        // �v���C���[�C���v�b�g���擾����
        Vector2 rotateInput = rotateAction.ReadValue<Vector2>();
        // ��]���������߂�
        _rotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);

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
                maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            }
            if(_mBoostCoolDown.IsTimerFinished())
            {
                _mBoostCoolDown = null;
            }
        }
    }


    /// <summary>
    /// �v���C���[�̃v���p�e�B������������
    /// </summary>
    private void Init()
    {
        _currentMoveSpeed = 0.0f;
        _rigidbody = GetComponent<Rigidbody>();
        colorCheck = GetComponent<ColorCheck>();
        colorCheck.layerMask = LayerMask.GetMask("Ground");
        _moveSpeedCoefficient = 1.0f;
        maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        acceleration = Global.PLAYER_ACCELERATION;
        rotationSpeed = Global.PLAYER_ROTATION_SPEED;
        dropSilkEvent = new DropSilkEvent()
        {
            dropMode = DropMode.Standard,
        };
        pickSilkEvent = new PickSilkEvent();

        playerInput = GetComponent<PlayerInput>();
        rotateAction = playerInput.actions["Rotate"];
        boostAction = playerInput.actions["Boost"];
        boostAction.performed += OnBoost;
        _mStatus = PlayerStatus.Fine;
        _mColliderOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _particleObject = Instantiate(GameResourceSystem.Instance.GetPrefabResource("DustParticlePrefab"), transform);
        _particleObject.transform.localPosition = Vector3.zero;
        _particleObject.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        _mParticleSystem = _particleObject.GetComponent<ParticleSystem>();
        _pSMain = _mParticleSystem.main;
        _pSMain.startSize = 0.4f;
        _pSMain.startColor = Color.gray;
        _boostDurationTime = Global.BOOST_DURATION_TIME;

        _explosionPrefab = GameResourceSystem.Instance.GetPrefabResource("Explosion");
        _bigSpider = Instantiate(GameResourceSystem.Instance.GetPrefabResource("BigSpider"),Vector3.zero,Quaternion.identity);
        _bigSpider.transform.position = new Vector3(0.0f,0.0f,100.0f);
        _bigSpider.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        _mBigSpiderLineRenderer = _bigSpider.GetComponentInChildren<LineRenderer>();
        _mBigSpiderLineRenderer.positionCount = 2;
        _mBigSpiderLineRenderer.startWidth = 0.2f;
        _mBigSpiderLineRenderer.endWidth = 0.2f;

        _mGotSilkImage = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
        _mGotSilkImage.SetActive(false);

        // �v���C���[�����̉摜�̃����_���[���擾����
        _mImageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // �\�����ʂ�ϊ�����
        _mImageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
        // ��������Ƃ������e�̃v���n�u���C���X�^���X������
        _mShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("PlayerShadow"), Vector3.zero, Quaternion.identity);
        _mShadow.transform.localScale = Vector3.zero;
        //TODO �e�̕�����ς���
        _mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // �e�𓧖��ɂ���
        _mShadowSpriteRenderer = _mShadow.GetComponent<SpriteRenderer>();
        _mShadowSpriteRenderer.color = Color.clear;

        _mSilkCount = 0;
        _mID = -1;

        // DropPointControl�R���|�l���g��ǉ�����
        _mDropPointControl = gameObject.AddComponent<DropPointControl>();

    }
    /// <summary>
    /// �v���C���[�̈ړ��𐧌䂷��
    /// </summary>
    private void PlayerMovement()
    {
        // �����^�������āA�ő呬�x�܂ŉ�������
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.fixedDeltaTime;
        // �O�����̈ړ�������
        Vector3 moveDirection = transform.forward * _currentMoveSpeed * Time.fixedDeltaTime * _moveSpeedCoefficient;
        _rigidbody.velocity = moveDirection;

    }

    /// <summary>
    /// �v���C���[�̉�]�𐧌䂷��
    /// </summary>
    private void PlayerRotation()
    {
        // �������͂��擾����
        if (_rotateDirection != Vector3.zero)
        {
            // ���͂��ꂽ�����։�]����
            Quaternion rotation = Quaternion.LookRotation(_rotateDirection, Vector3.up);
            RotateRigidbody(rotation);
        }
    }

    /// <summary>
    /// �v���C���[�̃��W�b�h�{�f�B����]������
    /// </summary>
    /// <param name="quaternion">��]�������</param>
    private void RotateRigidbody(Quaternion quaternion)
    {
        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotationSpeed * Time.fixedDeltaTime);
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
        // �����G�t�F�N�g
        {
            // �����A�j���[�V�������C���X�^���X������
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
            // �����̌��ʉ��𗬂�
            AudioManager.Instance.PlayFX("BoomFX", 0.7f);
        }

        // �v���C���[�̏�Ԃ����Z�b�g����
        ResetPlayerStatus();
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

        // �G�t�F�N�g���~�߂�
        if(_mParticleSystem.isPlaying)
        {
            _mParticleSystem.Stop();
        }

        // �����A�j���[�V����������������
        _mBigSpiderLineRenderer.positionCount = 2;
        _respawnAnimationTimer = new Timer();
        _respawnAnimationTimer.SetTimer(Global.RESPAWN_TIME,
            () =>
            {
                _respawnAnimationTimer = null;
            });
        _mGotSilkImage.SetActive(false);
    }



    /// <summary>
    /// �����A�j���[�V���������Z�b�g����
    /// </summary>
    private void ResetRespawnAnimation()
    {
        _bigSpider.transform.position = _mRespawnPos + new Vector3(0.0f, 0.0f, 100.0f);
        _mBigSpiderLineRenderer.positionCount = 0;
        _mShadow.transform.localScale = Vector3.zero;
        _mShadowSpriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// �����A�j���[�V�������X�V����֐�
    /// </summary>
    //TODO �J�������ɂ��鎞�ɕύX����\��
    private void UpdateRespawnAnimation()
    {
        // �����A�j���[�V�����O�������̏���
        if (_respawnAnimationTimer.GetTime() >= Global.RESPAWN_TIME / 2.0f)
        {
            _bigSpider.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f * Time.deltaTime), Space.World);
            transform.position = _bigSpider.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }
        // �����A�j���[�V�����㔼�����̏���
        else
        {
            //TODO
            transform.Translate(-(_bigSpider.transform.position - _mRespawnPos) * 0.4f * Time.deltaTime, Space.World);
            transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
            _mShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
            _mShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
            Vector3[] temp = new Vector3[2];
            temp[0] = _bigSpider.transform.position;
            temp[1] = transform.position + new Vector3(0.0f, -0.5f, _mColliderOffset);
            _mBigSpiderLineRenderer.SetPositions(temp);
        }
    }

    /// <summary>
    /// �v���C���[�̃X�e�C�^�X�����Z�b�g����
    /// </summary>
    private void ResetPlayerStatus()
    {
        transform.position = _bigSpider.transform.position;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _mStatus = PlayerStatus.Dead;
        transform.localScale = Vector3.one;
        _currentMoveSpeed = 0.0f;
        IsGotSilk = false;
        _isBoosting = false;
        _boostDurationTime = Global.BOOST_DURATION_TIME;
        _mBoostCoolDown = null;
        _mSilkCount = 0;
        transform.forward = Global.PLAYER_DEFAULT_FORWARD[(_mID - 1)];
        DropPointSystem.Instance.ClearDropPoints(_mID);
        maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        _mDropPointControl.ResetTrail();

    }
    /// <summary>
    /// �n�ʂ̐F���`�F�b�N����
    /// </summary>
    private void CheckGroundColor()
    {
        // �����̗̈�ɂ�����
        if (colorCheck.isTargetColor(Color.clear))
        {
            _moveSpeedCoefficient = 1.0f;
        }
        // �ʂ̃v���C���[�̗̈�ɂ�����
        else if (colorCheck.isTargetColor(_mColor))
        {
            _moveSpeedCoefficient = Global.SPEED_UP_COEFFICIENT;
        }
        // �h���Ă��Ȃ��n�ʂɂ�����
        else
        {
            _moveSpeedCoefficient = Global.SPEED_DOWN_COEFFICIENT;
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
            Vector3 endPoint1 = transform.position + transform.forward * _mColliderOffset;
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


    
    public PlayerStatus GetStatus() => _mStatus;
    public int GetID() => _mID;
    public Color GetColor() => _mColor;

    //todo �A�N�Z�X�C���q�̕ύX�\��
    public void SetStatus(PlayerStatus status)
    {
        _mStatus = status;
    }

    public void SetProperties(int ID, Color color)
    {
        if (_mID == -1)
        {
            _mID = ID;
            _mColor = color;
            if (_mID <= Global.PLAYER_START_POSITIONS.Length)
            {
                _mRespawnPos = Global.PLAYER_START_POSITIONS[(_mID - 1)];
                _bigSpider.transform.position += _mRespawnPos;
                _mShadow.transform.position = _mRespawnPos;
            }
            name = "Player" + _mID.ToString();
        }
    }


    private void OnEnable()
    {
        rotateAction.Enable();
        boostAction.Enable();
    }
    private void OnDisable()
    {
        rotateAction.Disable();
        boostAction.Disable();
    }
    private void OnDestroy()
    {
        boostAction.performed -= OnBoost;
    }

    // �B��d�l
    private void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_mStatus == PlayerStatus.Fine && _isBoosting == false)
            {
                maxMoveSpeed *= 1.5f;
                _currentMoveSpeed = maxMoveSpeed;
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
