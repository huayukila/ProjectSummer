using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class PlayerControl : Player
{
    float _currentMoveSpeed;         // �v���C���[�̌��ݑ��x
    GameObject _rootTail;            // �K���̓�
    GameObject _tipTail;             // �K���̔�
    bool _isPainting;                // �n�ʂɕ`���邩�ǂ����̐M��
    float _timer;                    // �O��̕`�悪�I����Ă���̌o�ߎ���
    Rigidbody _rigidBody;   
    
    public GameObject tailPrefab;
    public Paintable p;             // �n��

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
        _tipTail = tc?.GetTipTail();

    }

    private void OnTriggerEnter(Collider other)
    {
        // DropPoint�ɓ���������
        if(other.gameObject.CompareTag("DropPoint") && !_isPainting)
        {
            _isPainting = true;
            // �`�悷�ׂ��̈�̒��_���擾����
            List<Vector3> verts = DropPointManager.Instance.GetPaintablePointVector3(other.gameObject);
            if(verts != null)
            {
                TailControl tc = _rootTail.GetComponent<TailControl>();
                GameObject[] tails = tc?.GetTails();
                for (int i = 1; i < Global.iMAX_TAIL_COUNT + 1;++i)
                {
                    verts.Add(tails[^i].transform.position);
                }
            }
            verts.Add(transform.position);

            // �̈��`�悷��
            PolygonPaintManager.Instance.Paint(p, verts?.ToArray());
            // DropPoint������
            DropPointManager.Instance.Clear();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DropPoint"))
        {
            SphereCollider temp = collision.collider.gameObject.GetComponent<SphereCollider>();
            Debug.Log("DP");
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Wall");
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("BOOM!!!!!!!");
        }
    }
    private void Awake()
    {
        _isPainting = false;
        _timer = 0.0f;
        _currentMoveSpeed = 0.0f;
        SetTail();
        _tipTail?.AddComponent<DropPointControl>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
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
    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerRotation();
        _rootTail.transform.position = transform.position;
    }


    /// <summary>
    /// �v���C���[�̈ړ��𐧌䂷��
    /// </summary>
    private void PlayerMovement()
    {
        // �����^�������āA�ő呬�x�܂ŉ�������
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.deltaTime;

        Vector3 movementDirection = transform.forward * _currentMoveSpeed * Time.deltaTime;

        _rigidBody.velocity = movementDirection;

    }

    /// <summary>
    /// �v���C���[�̉�]�𐧌䂷��
    /// </summary>
    private void PlayerRotation()
    {
        // �������͂��擾����
        float horizontal = Input.GetAxis(name + "_Horizontal");
        float vertical = Input.GetAxis(name + "_Vertical");

        Vector3 rotationDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotationDirection != Vector3.zero)
        {
            // ���͂��ꂽ�����։�]����
            Quaternion rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }

    }
}
