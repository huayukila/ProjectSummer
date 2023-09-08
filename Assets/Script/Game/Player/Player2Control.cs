using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Control : Player
{
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player1Tail"))
        {
            SetDeadStatus();
        }
        // DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint2") && !isPainting)
        {
            isPainting = true;
            // �`�悷�ׂ��̈�̒��_���擾����
            List<Vector3> verts = DropPointManager.Instance.GetPlayerTwoPaintablePointVector3(other.gameObject);
            if (verts != null)
            {
                TailControl tc = rootTail.GetComponent<TailControl>();
                GameObject[] tails = tc?.GetTails();
                for (int i = 1; i < Global.iMAX_TAIL_COUNT + 1; ++i)
                {
                    verts.Add(tails[^i].transform.position);
                }
            }
            verts.Add(transform.position);

            // �̈��`�悷��
            PolygonPaintManager.Instance.Paint(verts.ToArray(), Global.PLAYER_TWO_AREA_COLOR);
            // DropPoint������
            DropPointManager.Instance.ClearPlayerTwoDropPoints();
            tipTail.GetComponent<Player2DropControl>().ClearTrail();
        }

    }

    protected override void Awake()
    {
        base.Awake();
        PlayerManager.Instance.player2 = gameObject;
        rootTail.GetComponent<TailControl>().SetTailsTag("Player2Tail");
        tipTail.AddComponent<Player2DropControl>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerRotation();
    }

    protected override void ResetPlayerTransform()
    {
        gameObject.transform.position = Global.PLAYER2_START_POSITION;
        gameObject.transform.localEulerAngles = new Vector3(0.0f,-1.0f,0.0f);
        transform.forward = Vector3.back;
    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        tipTail.GetComponent<Player2DropControl>().ClearTrail();
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
    }

    protected override void GroundColorCheck()
    {
        if (colorCheck.IsSameColor(Global.PLAYER_TWO_AREA_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        else if (colorCheck.IsSameColor(Global.PLAYER_ONE_AREA_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_DOWN_COEFFICIENT);
        }
        else
        {
            SetMoveSpeedCoefficient(1.0f);
        }
    }

}
