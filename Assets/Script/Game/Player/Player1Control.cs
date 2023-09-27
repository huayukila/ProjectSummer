using System.Collections.Generic;
using UnityEngine;

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
            SetDeadStatus();
            if(ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
        }
        // ������DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint1") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        // ���̖Ԃɓ���������
        if(other.gameObject.CompareTag("GoldenSilk"))
        {
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // �S�[���ɓ���������
        if(other.gameObject.CompareTag("Goal"))
        {
            // ���������̖Ԃ������Ă�����
            if(ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = 1
                };
                TypeEventSystem.Instance.Send<AddScoreEvent>(AddScoreEvent);
                gameObject.GetComponent<Renderer>().material.color = Color.black;
            }
        }
    }

    protected override void PlayerRotation()
    {   
        
        float horizontal = 0.0f;
        float vertical = 0.0f;
        var controller = Input.GetJoystickNames()[0];
        if (!string.IsNullOrEmpty(controller))
        { 
            horizontal = Input.GetAxis("1L_Joystick_H");
            vertical = Input.GetAxis("1L_Joystick_V");
        }
        else
        {
            horizontal = Input.GetAxis("Player1_Horizontal");
            vertical = Input.GetAxis("Player1_Vertical");

        }
        // �������͂��擾����

        Vector3 rotateDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotateDirection != Vector3.zero)
        {
            // ���͂��ꂽ�����։�]����
            Quaternion rotation = Quaternion.LookRotation(rotateDirection, Vector3.up);
            RotateRigidbody(rotation);
        }

    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        // DropPoint��S�ď���
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
    }

    protected override void GroundColorCheck()
    {
        // �����̗̈�ɂ�����
        if(colorCheck.isTargetColor(GetAreaColor()))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // �ʂ̃v���C���[�̗̈�ɂ�����
        else if(colorCheck.isTargetColor(GameManager.Instance.playerTwo.GetComponent<Player2Control>().GetAreaColor()))
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
        p1dc = gameObject.AddComponent<Player1DropControl>();
    }

    private void Start()
    {
        GameManager.Instance.playerOne = gameObject;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerRotation();
    }

}
