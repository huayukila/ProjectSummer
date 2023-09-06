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
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player2Tail"))
        {
            SetDeadStatus();
        }
        // DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint1") && !_isPainting)
        {
            _isPainting = true;
            // �`�悷�ׂ��̈�̒��_���擾����
            List<Vector3> verts = DropPointManager.Instance.GetPlayerOnePaintablePointVector3(other.gameObject);
            if (verts != null)
            {
                TailControl tc = _rootTail.GetComponent<TailControl>();
                GameObject[] tails = tc?.GetTails();
                for (int i = 1; i < Global.iMAX_TAIL_COUNT + 1; ++i)
                {
                    verts.Add(tails[^i].transform.position);
                }
            }
            verts.Add(transform.position);

            // �̈��`�悷��
            PolygonPaintManager.Instance.Paint(verts.ToArray(), _areaColor);
            // DropPoint������
            DropPointManager.Instance.ClearPlayerOneDropPoints();
            _tipTail.GetComponent<Player1DropControl>().ClearTrail();
        }

    }

    protected override void Awake()
    {
        base.Awake();
        PlayerManager.Instance.player1 = gameObject;
        _rootTail.GetComponent<TailControl>().SetTailsTag("Player1Tail") ;
        _tipTail.AddComponent<Player1DropControl>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerRotation();
    }

    public override void Respawn()
    {
        base.Respawn();
        _tipTail.GetComponent<Player1DropControl>().ClearTrail();
        DropPointManager.Instance.ClearPlayerOneDropPoints();

    }

    protected override void ResetPlayerTransform()
    {
        transform.position = Global.PLAYER1_START_POSITION;
        transform.localEulerAngles = new Vector3(0.0f,0.0f,0.0f);
        transform.forward = Vector3.forward;
    }

}
