using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Player
{
    float currentMoveSpeed;         // �v���C���[�̌��ݑ��x
    GameObject rootTail;            // �K���̓�
    GameObject tipTail;             // �K���̔�

    public GameObject tailPrefab;
    public Paintable p;             // �n��

    bool isPainting;                // �n�ʂɕ`���邩�ǂ����̐M��
    float timer;                    // �O��̕`�悪�I����Ă���̌o�ߎ���

    private void FixedUpdate()
    {

        PlayerMovement();
    }

    /// <summary>
    /// �K�����C���X�^���X������
    /// </summary>
    private void SetTail()
    {
        GameObject tail = Instantiate(tailPrefab, transform);

        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.parent = transform;
        tail.transform.localPosition = new Vector3(0.0f, 0.0f, -0.5f);
        tail.AddComponent<TailControl>();

        rootTail = tail;

        TailControl temp = rootTail.GetComponent<TailControl>();
        tipTail = temp?.GetTipTail();

    }

    private void OnTriggerEnter(Collider other)
    {
        // DropPoint�ɓ���������
        if(other.gameObject.CompareTag("DropPoint") && !isPainting)
        {
            isPainting = true;

            // �`�悷�ׂ��̈�̒��_���擾����
            List<Vector3> verts = DropPointManager.Instance.GetPaintablePointVector3(other.gameObject);
            if(verts != null)
            {
                TailControl tc = rootTail.GetComponent<TailControl>();
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

    private void Awake()
    {
        isPainting = false;
        timer = 0.0f;
        currentMoveSpeed = 0.0f;
        SetTail();
        tipTail?.AddComponent<DropPointControl>();
    }

    // Update is called once per frame
    private void Update()
    {
        // �`��𐧌�����i���Łj
        if (isPainting)
        {
            timer += Time.deltaTime;
        }
        if (timer >= 2.0f)
        {
            isPainting = false;
            timer = 0.0f;
        }
    }

    /// <summary>
    /// �v���C���[�̈ړ��𐧌䂷��
    /// </summary>
    private void PlayerMovement()
    {
        // �����^�������āA�ő呬�x�܂ŉ�������
        currentMoveSpeed = currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : currentMoveSpeed + acceleration * Time.deltaTime;

        Vector3 movementDirection = Vector3.forward * currentMoveSpeed;
        transform.Translate(movementDirection * Time.fixedDeltaTime);

    }

    /// <summary>
    /// �v���C���[�̉�]�𐧌䂷��
    /// </summary>
    private void PlayerRotation()
    {
        // �������͂��擾����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 rotationDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotationDirection != Vector3.zero)
        {
            // ���͂��ꂽ�����։�]����
            Quaternion rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }

    }
}
