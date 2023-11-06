using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public enum PlayerStatus
{
    Fine,
    Dead
}
[RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField]
    float maxMoveSpeed;                 // �v���C���[�̍ő呬�x
    [Min(0.0f)][SerializeField]
    float acceleration;                 // �v���C���[�̉����x
    [Min(0.0f)][SerializeField]
    float rotationSpeed;                // �v���C���[�̉�]���x

    protected bool isPainting;                          // �n�ʂɕ`���邩�ǂ����̐M��   
    protected ColorCheck colorCheck;                    // �J���[�`�F�b�N�R���|�l���g
    protected DropSilkEvent dropSilkEvent;              // ���̖Ԃ𗎂Ƃ��C�x���g
    protected PickSilkEvent pickSilkEvent;              // ���̖Ԃ��E���C�x���g
    protected InputAction playerAction;
    private InputAction rotateAction;
    protected PlayerInput playerInput;
    protected PlayerStatus mStatus;
    protected float offset;

    private Timer _paintableTimer;                      // �̈��`���Ԋu���Ǘ�����^�C�}�[
    private float _currentMoveSpeed;                    // �v���C���[�̌��ݑ��x
    private float _moveSpeedCoefficient;                // �v���C���[�̈ړ����x�̌W��
    private Rigidbody _rigidbody;                       // �v���C���[��Rigidbody
    private Vector3 _rotateDirection;
    private GameObject _particleObject;
    private GameObject _particlePrefab;
    private ParticleSystem _pS;
    private ParticleSystem.MainModule _pSMain;
    private Timer _mBoostCoolDown;
    private float _boostDurationTime = Global.BOOST_DURATION_TIME;
    private bool _isBoosting = false;
    private SpriteRenderer _mSpriteRenderer;
    private SpriteRenderer _mShadowSpriteRenderer;
    private GameObject _explosionPrefab;
    private GameObject _bigSpider;
    private LineRenderer _mBigSpiderLineRenderer;
    private GameObject _mShadow;
    //todo
    protected GameObject _mGotSilkImage;

    //todo refactorying
    protected Vector3 _mRespawnPos;
    private int _mID;
    private Color _mColor;
    protected DropPointControl _mDropPointControl;

    private Timer _respawnAnimationTimer;

    public bool IsGotSilk { get; protected set; }
    //todo
    private int _mSilkCount;

    public void SetProperties(int ID , Color color)
    {
        if(_mID == -1)
        {
            _mID = ID;
            _mColor = color;
            if(_mID <= Global.PLAYER_START_POSITIONS.Length)
            {
                _mRespawnPos = Global.PLAYER_START_POSITIONS[(_mID - 1)];
                _bigSpider.transform.position += _mRespawnPos;
                _mShadow.transform.position = _mRespawnPos;
            }
            name = "Player" + _mID.ToString();
        }
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
            // ���̖Ԃ̃h���b�v�ꏊ��ݒ肷��
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

    protected virtual void OnTriggerEnter(Collider other)
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
        // ���̖Ԃɓ���������
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            if (_mSilkCount == 3)
                return;
            //TODO �C��(3�܂Œǉ�)
            IsGotSilk = true;
            _mGotSilkImage.SetActive(true);
            _mGotSilkImage.transform.position = transform.position + Vector3.forward * 6.5f;
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
            _mSilkCount++;
        }
        // �S�[���ɓ���������
        if (other.gameObject.CompareTag("Goal"))
        {
            // ���������̖Ԃ������Ă�����
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

    protected virtual void Awake()
    {
        // ����������
        Init();
    }
    private void Update()
    {
        // �v���C���[�摜�������Ɠ��������Ɍ������Ƃɂ���
        _mSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // �ʏ��Ԃ���Ȃ��ƕ����A�j���[�V��������������
        if (mStatus != PlayerStatus.Fine�@|| _respawnAnimationTimer != null)
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

    private void UpdateFine()
    {
        // �G�t�F�N�g�X�^�[�g
        if (_pS.isStopped)
        {
            _pS.Play();
        }
        // �G�t�F�N�g�̍X�V
        {
            _pSMain.startSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 2.0f;
            _pSMain.simulationSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 4.0f + 1.0f;
            _pSMain.startLifetime = _pSMain.simulationSpeed * 0.5f;

        }

        // �v���C���[�摜�̌�����ς���
        if (transform.forward.x < 0.0f)
        {
            _mSpriteRenderer.flipX = false;
        }
        else
        {
            _mSpriteRenderer.flipX = true;
        }

        // ���̖Ԃ̂������Ă���΁A�v���C���[�摜�̏�ɕ\������
        if(IsGotSilk == true)
        {
            _mGotSilkImage.transform.position = transform.position + Vector3.forward * 6.5f;
        }

        // �v���C���[�C���v�b�g���擾����
        Vector2 rotateInput = rotateAction.ReadValue<Vector2>();
        // ��]���������߂�
        _rotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);

        // �v���C���[������Ƃ���̒n�ʂ̐F���`�F�b�N����
        GroundColorCheck();
        // �`��𐧌�����i���Łj
        if (!isPainting)
        {
            // �w��肪�ł��邩�ǂ������`�F�b�N����
            CheckCanPaint();
        }
        else
        {
            // ���̐w���ɂ�0.3�b�̊Ԋu��݂���
            if (_paintableTimer == null)
            {
                _paintableTimer = new Timer();
                _paintableTimer.SetTimer(0.3f,
                    () =>
                    {
                        isPainting = false;
                    }
                    );
            }
            if (_paintableTimer.IsTimerFinished())
            {
                _paintableTimer = null;
            }
        }

        //TODO �u�[�X�g
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

    private void FixedUpdate()
    {
        if(mStatus != PlayerStatus.Fine)
        {
            return;
        }
        // �v���C���[�̓���
        PlayerMovement();
        // �v���C���[�̉�]
        PlayerRotation();
    }

    /// <summary>
    /// �v���C���[�̃v���p�e�B������������
    /// </summary>
    private void Init()
    {
        isPainting = false;
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
        playerAction = playerInput.actions["Boost"];
        playerAction.performed += OnBoost;
        mStatus = PlayerStatus.Fine;
        offset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _particlePrefab = GameResourceSystem.Instance.GetResource("DustParticlePrefab");
        _particleObject = Instantiate(_particlePrefab, transform);
        _particleObject.transform.localPosition = Vector3.zero;
        _particleObject.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        _pS = _particleObject.GetComponent<ParticleSystem>();
        _pSMain = _pS.main;
        _pSMain.startSize = 0.4f;
        _pSMain.startColor = Color.gray;

        _explosionPrefab = GameResourceSystem.Instance.GetResource("Explosion");
        _bigSpider = Instantiate(GameResourceSystem.Instance.GetResource("BigSpider"),Vector3.zero,Quaternion.identity);
        _bigSpider.transform.position = new Vector3(0.0f,0.0f,100.0f);
        _bigSpider.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        _mBigSpiderLineRenderer = _bigSpider.GetComponentInChildren<LineRenderer>();
        _mBigSpiderLineRenderer.positionCount = 2;
        _mBigSpiderLineRenderer.startWidth = 0.2f;
        _mBigSpiderLineRenderer.endWidth = 0.2f;

        _mGotSilkImage = Instantiate(GameResourceSystem.Instance.GetResource("GoldenSilkImage"));
        _mGotSilkImage.SetActive(false);

        // �v���C���[�����̉摜�̃����_���[���擾����
        _mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // �\�����ʂ�ϊ�����
        _mSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
        // ��������Ƃ������e�̃v���n�u���C���X�^���X������
        _mShadow = Instantiate(GameResourceSystem.Instance.GetResource("PlayerShadow"), Vector3.zero, Quaternion.identity);
        _mShadow.transform.localScale = Vector3.zero;
        //TODO �e�̕�����ς���
        _mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // �e�𓧖��ɂ���
        _mShadowSpriteRenderer = _mShadow.GetComponent<SpriteRenderer>();
        _mShadowSpriteRenderer.color = Color.clear;

        _mSilkCount = 0;
        _mID = -1;

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
        if(_pS.isPlaying)
        {
            _pS.Stop();
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

    //todo �A�N�Z�X�C���q�̕ύX�\��
    public PlayerStatus GetStatus() => mStatus;
    public int GetID() => _mID;
    public Color GetColor() => _mColor;
    public void SetStatus(PlayerStatus status)
    {
        mStatus = status;
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
    /// �����A�j���[�V���������Z�b�g����
    /// </summary>
    private void ResetRespawnAnimation()
    {
        _bigSpider.transform.position = _mRespawnPos + new Vector3(0.0f,0.0f,100.0f);
        _mBigSpiderLineRenderer.positionCount = 0;
        _mShadow.transform.localScale = Vector3.zero;
        _mShadowSpriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// �����A�j���[�V�������X�V����֐�
    /// </summary>
    private void UpdateRespawnAnimation()
    {
        // �����A�j���[�V�����O�������̏���
        if(_respawnAnimationTimer.GetTime() >= Global.RESPAWN_TIME /2.0f)
        {
            _bigSpider.transform.Translate(new Vector3(0.0f, 0.0f,-20.0f * Time.deltaTime),Space.World);
            transform.position = _bigSpider.transform.position + new Vector3(0.0f,0.5f,0.0f);
        }
        // �����A�j���[�V�����㔼�����̏���
        else
        {
            //TODO
            transform.Translate(-(_bigSpider.transform.position - _mRespawnPos) * 0.4f * Time.deltaTime,Space.World);
            transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
            _mShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
            _mShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
            Vector3[] temp = new Vector3[2];
            temp[0] = _bigSpider.transform.position;
            temp[1] = transform.position + new Vector3(0.0f,-0.5f,offset);
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
        mStatus = PlayerStatus.Dead;
        transform.localScale = Vector3.one;
        _currentMoveSpeed = 0.0f;
        IsGotSilk = false;
        _isBoosting = false;
        _boostDurationTime = Global.BOOST_DURATION_TIME;
        _mBoostCoolDown = null;
        _mSilkCount = 0;
        transform.forward = Global.PLAYER_DEFAULT_FORWARD[(_mID-1)];
        DropPointManager.Instance.ClearDropPoints(_mID);
        maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        //TODO mix two func
        _mDropPointControl.ClearTrail();
        _mDropPointControl.ResetTrail();

    }
    /// <summary>
    /// �n�ʂ̐F���`�F�b�N����
    /// </summary>
    private void GroundColorCheck()
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
        else
        {
            _moveSpeedCoefficient = Global.SPEED_DOWN_COEFFICIENT;
        }

    }

    private void CheckCanPaint()
    {
        Vector3[] dropPoints = DropPointManager.Instance.GetPlayerDropPoints(_mID);
        if (dropPoints.Length >= 4)
        {
            //todo �v���C���[�̑傫���ɂ���ăI�t�Z�b�g���ς��
            Vector3 endPoint1 = transform.position + transform.forward * offset;
            Vector3 endPoint2 = dropPoints[dropPoints.Length - 1];
            int endIndex = dropPoints.Length - 2;
            if (endPoint1 == endPoint2)
            {
                endPoint2 = dropPoints[endIndex];
                endIndex--;
            }
            for (int i = 0; i < endIndex; ++i)
            {
                if (VectorMath.IsParallel(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1))
                {
                    continue;
                }

                float pointPos1 = VectorMath.PointOfLine(dropPoints[i], endPoint2, endPoint1);
                float pointPos2 = VectorMath.PointOfLine(dropPoints[i + 1], endPoint2, endPoint1);
                float pointPos3 = VectorMath.PointOfLine(endPoint2, dropPoints[i], dropPoints[i + 1]);
                float pointPos4 = VectorMath.PointOfLine(endPoint1, dropPoints[i], dropPoints[i + 1]);

                if (pointPos1 * pointPos2 < 0 && pointPos3 * pointPos4 < 0)
                {
                    isPainting = true;
                    Vector3 crossPoint = VectorMath.GetCrossPoint(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1);
                    List<Vector3> verts = new List<Vector3>();
                    for (int j = i + 1; j < dropPoints.Length; j++)
                    {
                        verts.Add(dropPoints[j]);
                    }
                    verts.Add(crossPoint);
                    PolygonPaintManager.Instance.Paint(verts.ToArray(), _mID, _mColor);
                    DropPointManager.Instance.ClearDropPoints(_mID);
                    _mDropPointControl.ClearTrail();
                    _mDropPointControl.ResetTrail();
                    break;
                }
            }
        }

    }

    private void OnEnable()
    {
        rotateAction.Enable();
        playerAction.Enable();
    }
    private void OnDisable()
    {
        rotateAction.Disable();
        playerAction.Disable();
    }
    private void OnDestroy()
    {
        playerAction.performed -= OnBoost;
        Resources.UnloadUnusedAssets();
    }
    private void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(mStatus == PlayerStatus.Fine && _isBoosting == false)
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
