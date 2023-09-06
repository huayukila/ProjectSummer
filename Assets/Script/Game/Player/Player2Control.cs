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
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player1Tail"))
        {
            SetDeadStatus();
        }
        // DropPoint�ɓ���������
        if (other.gameObject.CompareTag("DropPoint2") && !_isPainting)
        {
            _isPainting = true;
            // �`�悷�ׂ��̈�̒��_���擾����
            List<Vector3> verts = DropPointManager.Instance.GetPlayerTwoPaintablePointVector3(other.gameObject);
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
            DropPointManager.Instance.ClearPlayerTwoDropPoints();
            _tipTail.GetComponent<Player2DropControl>().ClearTrail();
        }

    }

    protected override void Awake()
    {
        base.Awake();
        PlayerManager.Instance.player2 = gameObject;
        _rootTail.GetComponent<TailControl>().SetTailsTag("Player2Tail");
        _tipTail.AddComponent<Player2DropControl>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerRotation();
    }
    public override void Respawn()
    {
        base.Respawn();
        _tipTail.GetComponent<Player2DropControl>().ClearTrail();
        DropPointManager.Instance.ClearPlayerTwoDropPoints();

    }

    protected override void ResetPlayerTransform()
    {
        gameObject.transform.position = Global.PLAYER2_START_POSITION;
        gameObject.transform.localEulerAngles = new Vector3(0.0f,-1.0f,0.0f);
        transform.forward = Vector3.back;

    }


}
