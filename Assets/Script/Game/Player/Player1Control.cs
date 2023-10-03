using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player1DropControl))]
public class Player1Control : Player
{
    private Player1DropControl p1dc;

    /// <summary>
    /// ���������Ƃ��̏������X
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // �ʂ̃v���C���[��DropPoint�ɓ���������
        if(other.gameObject.CompareTag("DropPoint2"))
        {
            
            if(IsGotSilk)
            {
                dropSilkEvent.pos = transform.position;
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
            SetDeadStatus();
        }
        // ������DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint1") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        // ���̖Ԃɓ���������
        if(other.gameObject.CompareTag("GoldenSilk"))
        {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            IsGotSilk = true;
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // �S�[���ɓ���������
        if(other.gameObject.CompareTag("Goal"))
        {
            // ���������̖Ԃ������Ă�����
            if(IsGotSilk)
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = 1
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
        // DropPoint��S�ď���
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
        transform.position = Global.PLAYER1_START_POSITION * 100.0f;
    }

    protected override void GroundColorCheck()
    {
        // �����̗̈�ɂ�����
        if(colorCheck.isTargetColor(Global.PLAYER_ONE_TRACE_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // �ʂ̃v���C���[�̗̈�ɂ�����
        else if (colorCheck.isTargetColor(Global.PLAYER_TWO_TRACE_COLOR))
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
        List<Vector3> verts = DropPointManager.Instance.GetPlayerOnePaintablePointVector3(ob.gameObject);
        verts.Add(transform.position);
        // �̈��`�悷��@���P�̓v���C���[�P���w��
        PolygonPaintManager.Instance.Paint(verts.ToArray(),1,GetAreaColor());
        // DropPoint��S�ď���
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        gameObject.GetComponent<Player1DropControl>().ClearTrail();
    }

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.playerOne = gameObject;
        p1dc = GetComponent<Player1DropControl>();
    }

    private void Start()
    {

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
