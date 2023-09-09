using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public float maxMoveSpeed;
    [Min(0.0f)] public float acceleration;
    [Min(0.0f)] public float rotationSpeed;    
    public GameObject tailPrefab;

    float _currentMoveSpeed;                    // �v���C���[�̌��ݑ��x

    protected GameObject rootTail;             // �K���̓�
    protected GameObject tipTail;              // �K���̔�

    protected bool isPainting;                 // �n�ʂɕ`���邩�ǂ����̐M��
    Timer _timer;
    private Rigidbody _rigidbody;
    protected ColorCheck colorCheck;

    private float _moveSpeedCoefficient;

    /// <summary>
    /// �K�����C���X�^���X������
    /// </summary>
    private void SetTail()
    {
        GameObject tail = Instantiate(tailPrefab);
        tail.name = gameObject.name + "Tail";
        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.position = transform.position;
        tail.AddComponent<TailControl>();

        rootTail = tail;

        TailControl tc = rootTail.GetComponent<TailControl>();
        tipTail = tc.GetTipTail();

    }


    private void OnCollisionEnter(Collision collision)
    {
        bool isCollision = collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall");
        if (isCollision)
        {
            SetDeadStatus();
        }

    }

    protected virtual void Awake()
    {
        isPainting = false;
        _currentMoveSpeed = 0.0f;
        SetTail();
        _rigidbody = GetComponent<Rigidbody>();
        colorCheck = GetComponent<ColorCheck>();
        _moveSpeedCoefficient = 1.0f;
    }

    private void Update()
    {
        // �`��𐧌�����i���Łj
        if (isPainting)
        {
            if(_timer == null)
            {
                _timer = new Timer();
                _timer.SetTimer(0.5f,
                    () =>
                    {
                        isPainting = false;
                    }
                    );
            }
            if(_timer.IsTimerFinished())
            {
                _timer = null;
            }
        }
        GroundColorCheck();

    }
    protected virtual void FixedUpdate()
    {
        PlayerMovement();
        rootTail.transform.position = transform.position;
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

    protected virtual void SetDeadStatus()
    {
        gameObject.SetActive(false);
        rootTail.GetComponent<TailControl>().SetDeactive();
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        if(ScoreItemManager.Instance.IsGotSilk(gameObject))
        {
            ScoreItemManager.Instance.DropGoldenSilk();
        }

    }
    public void Respawn()
    {
        if(!gameObject.activeSelf)
        {
            _currentMoveSpeed = 0.0f;
            ResetPlayerTransform();
            rootTail.GetComponent<TailControl>().SetActive(transform.position);
            gameObject.SetActive(true);
            gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
    }
    protected void RotateRigidbody(Quaternion quaternion)
    {
        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotationSpeed * Time.fixedDeltaTime);
    }    
    
    protected void SetMoveSpeedCoefficient(float coefficient)
    {
        _moveSpeedCoefficient = coefficient;
    }
    /// <summary>
    /// �v���C���[�̉�]�𐧌䂷��
    /// </summary>
    protected abstract void PlayerRotation();

    protected abstract void ResetPlayerTransform();

    protected abstract void GroundColorCheck();

    protected abstract void PaintArea(GameObject @object);
}
