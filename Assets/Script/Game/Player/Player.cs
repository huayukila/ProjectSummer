using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public float maxMoveSpeed;                  // �v���C���[�̍ő呬�x
    [Min(0.0f)] public float acceleration;      // �v���C���[�̉����x
    [Min(0.0f)] public float rotationSpeed;     // �v���C���[�̉�]���x
    [SerializeField]Color32 _areaColor;         // �̈�܂��͈ړ������Ղ̐F

    protected bool isPainting;                  // �n�ʂɕ`���邩�ǂ����̐M�� 
    private Rigidbody _rigidbody;               // �v���C���[��Rigidbody
    protected ColorCheck colorCheck;            // �J���[�`�F�b�N�R���|�l���g
    protected DropSilkEvent dropSilkEvent;
    protected PickSilkEvent pickSilkEvent;

    private Timer _paintableTimer;              // �̈��`���Ԋu���Ǘ�����^�C�}�[
    private float _currentMoveSpeed;            // �v���C���[�̌��ݑ��x
    private float _moveSpeedCoefficient;        // �v���C���[�̈ړ����x�̌W��

    /// <summary>
    /// �Փ˂��������Ƃ���������
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����玀�S��Ԃɐݒ肷��
        SetDeadStatus();
        // ���S�����v���C���[�͋��̖Ԃ������Ă�����
        if (ScoreItemManager.Instance.IsGotSilk(gameObject))
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                dropSilkEvent.dropMode = DropMode.Edge;
            }
            TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);

        }

    }

    protected virtual void Awake()
    {
        isPainting = false;
        _currentMoveSpeed = 0.0f;
        _rigidbody = GetComponent<Rigidbody>();
        colorCheck = gameObject.AddComponent<ColorCheck>();
        colorCheck.layerMask = LayerMask.GetMask("Ground");
        _moveSpeedCoefficient = 1.0f;
        maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        acceleration = Global.PLAYER_ACCELERATION;
        rotationSpeed = Global.PLAYER_ROTATION_SPEED;
        dropSilkEvent = new DropSilkEvent()
        {
            dropMode = DropMode.Standard
        };
        pickSilkEvent = new PickSilkEvent()
        {
            player = gameObject
        };
    }

    private void Update()
    {
        // �`��𐧌�����i���Łj
        if (isPainting)
        {
            if(_paintableTimer == null)
            {
                _paintableTimer = new Timer();
                _paintableTimer.SetTimer(0.5f,
                    () =>
                    {
                        isPainting = false;
                    }
                    );
            }
            if(_paintableTimer.IsTimerFinished())
            {
                _paintableTimer = null;
            }
        }
        GroundColorCheck();

    }
    protected virtual void FixedUpdate()
    {
        PlayerMovement();
    }

    /// <summary>
    /// �v���C���[�̈ړ��𐧌䂷��
    /// </summary>
    private void PlayerMovement()
    {
        // �����^�������āA�ő呬�x�܂ŉ�������
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.deltaTime;
        Vector3 moveDirection = transform.forward * _currentMoveSpeed * Time.fixedDeltaTime * _moveSpeedCoefficient;
        _rigidbody.velocity = moveDirection;
    }

    /// <summary>
    /// �v���C���[�̎��S��Ԃ�ݒ肷��
    /// </summary>
    protected virtual void SetDeadStatus()
    {
        gameObject.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

    }

    // �v���C���[�𕜊�������
    public void Respawn()
    {
        if(!gameObject.activeSelf)
        {
            _currentMoveSpeed = 0.0f;
            ResetPlayerTransform();
            gameObject.SetActive(true);
            gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    public Color32 GetAreaColor() => _areaColor;

    public void SetAreaColor(Color32 color)
    {
        _areaColor = color;
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
    protected abstract void PlayerRotation();

    /// <summary>
    /// �v���C���[�̈ʒu�����Z�b�g����
    /// </summary>
    protected abstract void ResetPlayerTransform();

    /// <summary>
    /// �n�ʂ̐F���`�F�b�N����
    /// </summary>
    protected abstract void GroundColorCheck();

    /// <summary>
    /// �̈��`��
    /// </summary>
    /// <param name="object">���������������g�����Ƃ���DropPoint</param>
    protected abstract void PaintArea(GameObject @object);
}
