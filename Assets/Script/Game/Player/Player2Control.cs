using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player2DropControl))]
public class Player2Control : Player
{
    private Player2DropControl p2dc;
    

    /// <summary>
    /// ���������Ƃ��̏������X
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // �ʂ̃v���C���[��DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint1"))
        {

            if (IsGotSilk)
            {
                dropSilkEvent.pos = transform.position;
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
            SetDeadStatus();
        }
        // DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint2") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        // ���̖Ԃɓ���������
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            IsGotSilk = true;
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // �S�[���ɓ���������
        if (other.gameObject.CompareTag("Goal"))
        {
            // ���������̖Ԃ������Ă�����
            if (IsGotSilk)
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = 2
                };
                TypeEventSystem.Instance.Send<AddScoreEvent>(AddScoreEvent);
                gameObject.GetComponent<Renderer>().material.color = Color.black;
                IsGotSilk = false;
            }
        }
        
    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();
        transform.position = Global.PLAYER2_START_POSITION * 100.0f;
    }
    protected override void GroundColorCheck()
    {
        // �����̗̈�ɂ�����
        if (colorCheck.isTargetColor(Global.PLAYER_TWO_TRACE_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // �ʂ̃v���C���[�̗̈�ɂ�����
        else if (colorCheck.isTargetColor(Global.PLAYER_ONE_TRACE_COLOR))
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
        // �̈��`�悷��@���Q�̓v���C���[�Q���w��
        PolygonPaintManager.Instance.Paint(verts.ToArray(),2,GetAreaColor());
        // DropPoint��S�ď���
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();

    }

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.playerTwo = gameObject;
        p2dc = GetComponent<Player2DropControl>();
    }

    private void Start()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
