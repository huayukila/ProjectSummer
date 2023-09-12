using System.Collections.Generic;
using UnityEngine;

public class Player1Control : Player
{
    private Player1DropControl p1dc;
    protected override void PlayerRotation()
    {
        // �������͂��擾����
        float horizontal = Input.GetAxis("Player1_Horizontal");
        float vertical = Input.GetAxis("Player1_Vertical");

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
        // �ʂ̃v���C���[�̐K���ɓ���������
        if(other.gameObject.CompareTag("Player2Tail"))
        {
            SetDeadStatus();
        }
        // ������DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint1") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        if(other.gameObject.CompareTag("DropPoint2"))
        {
            SetDeadStatus();
        }
        // ���̖Ԃɓ���������
        if(other.gameObject.CompareTag("GoldenSilk"))
        {
            ScoreItemManager.Instance.SetGotSilkPlayer(gameObject);
        }
        // �S�[���ɓ���������
        if(other.gameObject.CompareTag("Goal"))
        {
            // ���������̖Ԃ������Ă�����
            if(ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                ScoreItemManager.Instance.SetReachGoalProperties();
                gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }

    protected override void ResetPlayerTransform()
    {
        transform.position = Global.PLAYER1_START_POSITION;
        transform.localEulerAngles = new Vector3(0.0f,0.0f,0.0f);
        transform.forward = Vector3.forward;
    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        // DropPoint��S�ď���
        //tipTail.GetComponent<Player1DropControl>().ClearTrail();
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
    }

    protected override void GroundColorCheck()
    {
        // �����̗̈�ɂ�����
        if(colorCheck.isTargetColor(areaColor))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // �ʂ̃v���C���[�̗̈�ɂ�����
        else if(colorCheck.isTargetColor(GameManager.Instance.playerTwo.GetComponent<Player2Control>().areaColor))
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
        /*if (verts != null)
        {
            TailControl tc = rootTail.GetComponent<TailControl>();
            GameObject[] tails = tc?.GetTails();
            for (int i = 1; i < Global.iMAX_TAIL_COUNT + 1; ++i)
            {
                verts.Add(tails[^i].transform.position);
            }
        }*/
        verts.Add(transform.position);

        // �̈��`�悷��
        PolygonPaintManager.Instance.Paint(verts.ToArray(), areaColor);
        // DropPoint��S�ď���
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        //tipTail.GetComponent<Player1DropControl>().ClearTrail();
        gameObject.GetComponent<Player1DropControl>().ClearTrail();
    }

    protected override void Awake()
    {
        base.Awake();
        //rootTail.GetComponent<TailControl>().SetTailsTag("Player1Tail");
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
