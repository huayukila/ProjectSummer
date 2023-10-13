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
    protected Image playerImage;
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

    }
    private void Update()
    {
        //todo
        playerImage.transform.position = transform.position - new Vector3(0.0f,0.1f,0.0f);
        playerImage.transform.forward = Vector3.down;
        if (status == PlayerStatus.Fine)
        {
            UpdateFine();
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
        Debug.Log(name + " " + maxMoveSpeed);
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
        playerImage = GetComponentInChildren<Image>();
        offset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _particlePrefab = Resources.Load("Prefabs/DustParticlePrefab") as GameObject;
        _particleObject = Instantiate(_particlePrefab, transform);
        _particleObject.transform.localPosition = Vector3.zero;
        _particleObject.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        _pS = _particleObject.GetComponent<ParticleSystem>();
        _pSMain = _pS.main;
        _pSMain.startSize = 0.4f;
        _pSMain.startColor = Color.gray;
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
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        status = PlayerStatus.Dead;
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
        playerImage.color = Color.white;
        if(_pS.isPlaying)
        {
            _pS.Stop();
        }
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
            Debug.Log("boost pressed");
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
