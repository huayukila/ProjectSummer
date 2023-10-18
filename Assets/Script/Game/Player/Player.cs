using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
public enum PlayerStatus
{
    Fine,
    Dead
}
[RequireComponent(typeof(ColorCheck),typeof(PlayerInput))]
public abstract class Player : MonoBehaviour
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
    protected PlayerStatus status;
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
    private string _mTag;
    private int _mID;

    private Timer _respawnAnimationTimer;

    // public InputActionReference rotateAction;
    public bool IsGotSilk { get; protected set; }

    /// <summary>
    /// �Փ˂��������Ƃ���������
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����玀�S��Ԃɐݒ肷��

        // ���S�����v���C���[�͋��̖Ԃ������Ă�����
        if (IsGotSilk)
        {
            dropSilkEvent.pos = transform.position;
            if (collision.gameObject.CompareTag("Wall"))
            {
                dropSilkEvent.dropMode = DropMode.Edge; 
            }
            TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
        }
        SetDeadStatus();
    }

    protected virtual void Awake()
    {
        Init();
    }
    private void Start()
    {
        _mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _mSpriteRenderer.transform.localPosition = Vector3.zero - new Vector3(0.0f,0.05f,0.0f);
        _mShadow = Instantiate(Resources.Load("Prefabs/PlayerShadow") as GameObject,_mRespawnPos,Quaternion.identity);
        _mShadow.transform.localScale = Vector3.zero;
        _mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        _mShadowSpriteRenderer = _mShadow.GetComponent<SpriteRenderer>();

        _mShadowSpriteRenderer.color = Color.clear;


    }
    private void Update()
    {
        _mSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        if (status == PlayerStatus.Fine)
        {
            UpdateFine();
        }
        //todo
        if (transform.forward.x < 0.0f)
        {
            _mSpriteRenderer.flipX = false;
        }
        else
        {
            _mSpriteRenderer.flipX = true;
        }
        if (_respawnAnimationTimer != null)
        {
            UpdateRespawnAnimation();
            if(_respawnAnimationTimer.IsTimerFinished())
            {
                ResetRespawnAnimation();
            }
        }

    }

    private void UpdateFine()
    {
        if (_pS.isStopped)
        {
            _pS.Play();
        };

        //�G�t�F�N�g�̍X�V
        {
            _pSMain.startSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 2.0f;
            _pSMain.simulationSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 4.0f + 1.0f;
            _pSMain.startLifetime = _pSMain.simulationSpeed * 0.5f;

        };

        if(IsGotSilk == true)
        {
            _mGotSilkImage.transform.position = transform.position + Vector3.forward * 6.5f;
        }
        Vector2 rotateInput = rotateAction.ReadValue<Vector2>();
        _rotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);
        GroundColorCheck();
        // �`��𐧌�����i���Łj
        if (!isPainting)
        {
            CheckCanPaint();
        }
        else
        {
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
        if(status == PlayerStatus.Fine)
        {
            PlayerMovement();
            PlayerRotation();
        }
    }

    protected virtual void Init()
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
        status = PlayerStatus.Fine;
        offset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _particlePrefab = Resources.Load("Prefabs/DustParticlePrefab") as GameObject;
        _particleObject = Instantiate(_particlePrefab, transform);
        _particleObject.transform.localPosition = Vector3.zero;
        _particleObject.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        _pS = _particleObject.GetComponent<ParticleSystem>();
        _pSMain = _pS.main;
        _pSMain.startSize = 0.4f;
        _pSMain.startColor = Color.gray;

        _explosionPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
        _bigSpider = Instantiate(GameManager.Instance.bigSpiderPrefab,_mRespawnPos,Quaternion.identity);
        _bigSpider.transform.position = _mRespawnPos + new Vector3(0.0f,0.0f,100.0f);
        _bigSpider.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        _mBigSpiderLineRenderer = _bigSpider.GetComponentInChildren<LineRenderer>();
        _mBigSpiderLineRenderer.positionCount = 2;
        _mBigSpiderLineRenderer.startWidth = 0.2f;
        _mBigSpiderLineRenderer.endWidth = 0.2f;

        _mGotSilkImage = Instantiate(Resources.Load("Prefabs/GoldenSilkImage") as GameObject);
        _mGotSilkImage.SetActive(false);


    }
    /// <summary>
    /// �v���C���[�̈ړ��𐧌䂷��
    /// </summary>
    private void PlayerMovement()
    {
        // �����^�������āA�ő呬�x�܂ŉ�������
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.fixedDeltaTime;
        Vector3 moveDirection = transform.forward * _currentMoveSpeed * Time.fixedDeltaTime * _moveSpeedCoefficient;
        _rigidbody.velocity = moveDirection;

    }

    /// <summary>
    /// �v���C���[�̎��S��Ԃ�ݒ肷��
    /// </summary>
    protected virtual void SetDeadStatus()
    {
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        AudioManager.Instance.PlayFX("BoomFX",0.7f);

        transform.position = _bigSpider.transform.position;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        status = PlayerStatus.Dead;
        transform.localScale = Vector3.one;
        _currentMoveSpeed = 0.0f;
        IsGotSilk = false;
        _isBoosting = false;
        _boostDurationTime = Global.BOOST_DURATION_TIME;
        _mBoostCoolDown = null;
        PlayerRespawnEvent playerRespawnEvent = new PlayerRespawnEvent()
        {
            player = gameObject
        };
        TypeEventSystem.Instance.Send<PlayerRespawnEvent>(playerRespawnEvent);
        GetComponent<DropPointControl>().enabled = false;
        GetComponentInChildren<TrailRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        if(_pS.isPlaying)
        {
            _pS.Stop();
        }

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
    public PlayerStatus GetStatus() => status;
    public void SetStatus(PlayerStatus status)
    {
        this.status = status;
    }
    /// <summary>
    /// �v���C���[�̃��W�b�h�{�f�B����]������
    /// </summary>
    /// <param name="quaternion">��]�������</param>
    protected void RotateRigidbody(Quaternion quaternion)
    {
        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotationSpeed * Time.fixedDeltaTime);
    }    
    
    /// <summary>
    /// �ړ����x�̌W����ݒ肷��
    /// </summary>
    /// <param name="coefficient"></param>
    protected void SetMoveSpeedCoefficient(float coefficient)
    {
        _moveSpeedCoefficient = coefficient;
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

    private void ResetRespawnAnimation()
    {
        _bigSpider.transform.position = _mRespawnPos + new Vector3(0.0f,0.0f,100.0f);
        _mBigSpiderLineRenderer.positionCount = 0;
        _mShadow.transform.localScale = Vector3.zero;
        _mShadowSpriteRenderer.color = Color.clear;

    }

    private void UpdateRespawnAnimation()
    {
        if(_respawnAnimationTimer.GetTime() >= Global.RESPAWN_TIME /2.0f)
        {
            _bigSpider.transform.Translate(new Vector3(0.0f, 0.0f,-20.0f * Time.deltaTime),Space.World);
            transform.position = _bigSpider.transform.position + new Vector3(0.0f,0.5f,0.0f);
        }
        else
        {
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
    /// �n�ʂ̐F���`�F�b�N����
    /// </summary>
    protected abstract void GroundColorCheck();

    protected abstract void CheckCanPaint();

    /// <summary>
    /// �̈��`��
    /// </summary>
    /// <param name="object">���������������g�����Ƃ���DropPoint</param>
    protected abstract void PaintArea(GameObject @object);

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
    }
    private void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_isBoosting == false)
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
