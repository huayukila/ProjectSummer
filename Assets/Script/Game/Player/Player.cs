using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public float maxMoveSpeed;
    [Min(0.0f)] public float acceleration;
    [Min(0.0f)] public float rotationSpeed;    
    public GameObject tailPrefab;
    public Color _areaColor;

    float _currentMoveSpeed;                    // �v���C���[�̌��ݑ��x

    protected GameObject _rootTail;             // �K���̓�
    protected GameObject _tipTail;              // �K���̔�

    protected bool _isPainting;                 // �n�ʂɕ`���邩�ǂ����̐M��
    float _timer;                               // �O��̕`�悪�I����Ă���̌o�ߎ���
    protected Rigidbody _rigidBody;

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

        _rootTail = tail;

        TailControl tc = _rootTail.GetComponent<TailControl>();
        _tipTail = tc.GetTipTail();

    }


    private void OnCollisionEnter(Collision collision)
    {
        bool isCollision = collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall");
        if (isCollision)
        {
            _rigidBody.Sleep();
            SetDeadStatus();
        }

    }

    protected virtual void Awake()
    {
        _isPainting = false;
        _timer = 0.0f;
        _currentMoveSpeed = 0.0f;
        SetTail();
        _rigidBody = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        // �`��𐧌�����i���Łj
        if (_isPainting)
        {
            _timer += Time.deltaTime;
        }
        if (_timer >= 0.5f)
        {
            _isPainting = false;
            _timer = 0.0f;
        }

    }
    protected virtual void FixedUpdate()
    {
        PlayerMovement();
        _rootTail.transform.position = transform.position;
    }

    /// <summary>
    /// �v���C���[�̈ړ��𐧌䂷��
    /// </summary>
    private void PlayerMovement()
    {
        // �����^�������āA�ő呬�x�܂ŉ�������
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.deltaTime;
        Vector3 moveDirection = transform.forward * _currentMoveSpeed * Time.fixedDeltaTime;
        _rigidBody.velocity = moveDirection;
    }

    protected void SetDeadStatus()
    {
        gameObject.SetActive(false);
        _rootTail.GetComponent<TailControl>().SetDeactive();
        _rigidBody.velocity = Vector3.zero;
    }
    public virtual void Respawn()
    {

        ResetPlayerTransform();
        _rootTail.GetComponent<TailControl>().SetActive(transform.position);
        gameObject.SetActive(true);
        _rigidBody.WakeUp();
    }

    /// <summary>
    /// �v���C���[�̉�]�𐧌䂷��
    /// </summary>
    protected abstract void PlayerRotation();

    protected abstract void ResetPlayerTransform();
}
