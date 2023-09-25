using System.Collections.Generic;
using UnityEngine;

public class Player2Control : Player
{
    private Player2DropControl p2dc;
    protected override void PlayerRotation()
    {
        // �������͂��擾����
        float horizontal = Input.GetAxis("Player2_Horizontal");
        float vertical = Input.GetAxis("Player2_Vertical");

        Vector3 rotateDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotateDirection != Vector3.zero)
        {
            // ���͂��ꂽ�����։�]����
            Quaternion rotation = Quaternion.LookRotation(rotateDirection, Vector3.up);
            RotateRigidbody(rotation);
        }

    }

    /// <summary>
    /// ���������Ƃ��̏������X
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // �ʂ̃v���C���[��DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint1"))
        {
            SetDeadStatus();
            if (ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
        }
        // DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint2") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        // ���̖Ԃɓ���������
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // �S�[���ɓ���������
        if (other.gameObject.CompareTag("Goal"))
        {
            // ���������̖Ԃ������Ă�����
            if (ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = 2
                };
                TypeEventSystem.Instance.Send<AddScoreEvent>(AddScoreEvent);
                gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }

    protected override void ResetPlayerTransform()
    {
        transform.position = Global.PLAYER2_START_POSITION;
        transform.forward = Vector3.back;
    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();
    }

    protected override void GroundColorCheck()
    {
        // �����̗̈�ɂ�����
        if (colorCheck.isTargetColor(GetAreaColor()))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // �ʂ̃v���C���[�̗̈�ɂ�����
        else if (colorCheck.isTargetColor(GameManager.Instance.playerOne.GetComponent<Player1Control>().GetAreaColor()))
        {
            SetMoveSpeedCoefficient(Global.SPEED_DOWN_COEFFICIENT);
        }
        else
        {
            SetMoveSpeedCoefficient(1.0f);
        }
    }

    protected override void PaintArea(GameObject ob)
    {
        isPainting = true;
        // �`�悷�ׂ��̈�̒��_���擾����
        List<Vector3> verts = DropPointManager.Instance.GetPlayerTwoPaintablePointVector3(ob.gameObject);
        verts.Add(transform.position);
        // �̈��`�悷��
        PolygonPaintManager.Instance.Paint(verts.ToArray(), GetAreaColor());
        // DropPoint��S�ď���
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();

    }

    protected override void Awake()
    {
        base.Awake();
        p2dc = gameObject.AddComponent<Player2DropControl>();
    }

    private void Start()
    {
        GameManager.Instance.playerTwo = gameObject;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerRotation();
    }
}
