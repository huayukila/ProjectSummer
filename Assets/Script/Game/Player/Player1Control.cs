using System.Collections.Generic;
using UnityEngine;
using System;

public class Player1Control : Player
{
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player2Tail"))
        {
            SetDeadStatus();
        }
        // DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint1") && !isPainting)
        {
            isPainting = true;
            // �`�悷�ׂ��̈�̒��_���擾����
            List<Vector3> verts = DropPointManager.Instance.GetPlayerOnePaintablePointVector3(other.gameObject);
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
            PolygonPaintManager.Instance.Paint(verts.ToArray(), Global.PLAYER_ONE_AREA_COLOR);
            // DropPoint������
            DropPointManager.Instance.ClearPlayerOneDropPoints();
            tipTail.GetComponent<Player1DropControl>().ClearTrail();
        }

    }

    protected override void Awake()
    {
        base.Awake();
        PlayerManager.Instance.player1 = gameObject;
        rootTail.GetComponent<TailControl>().SetTailsTag("Player1Tail") ;
        tipTail.AddComponent<Player1DropControl>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerRotation();
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
        tipTail.GetComponent<Player1DropControl>().ClearTrail();
        DropPointManager.Instance.ClearPlayerOneDropPoints();

    }

    protected override void GroundColorCheck()
    {
        if(colorCheck.IsSameColor(Global.PLAYER_ONE_AREA_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        else if(colorCheck.IsSameColor(Global.PLAYER_TWO_AREA_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_DOWN_COEFFICIENT);
        }
        else
        {
            SetMoveSpeedCoefficient(1.0f);
        }
    }
}