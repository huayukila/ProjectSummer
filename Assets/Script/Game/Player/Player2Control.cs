using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

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
        // ���̖Ԃɓ���������
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            playerImage.color = Color.yellow;
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
                playerImage.color = Color.white;
                IsGotSilk = false;
            }
        }
        
    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();
        p2dc.ResetTrail();
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
        List<Vector3> verts = DropPointManager.Instance.GetPlayerTwoPaintablePointVector3(ob);
        verts.Add(transform.position);
        // �̈��`�悷��@���Q�̓v���C���[�Q���w��
        PolygonPaintManager.Instance.Paint(verts.ToArray(),2, Global.PLAYER_TWO_TRACE_COLOR);
        // DropPoint��S�ď���
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();
        p2dc.ResetTrail();

    }

    protected override void CheckCanPaint()
    {
        Vector3[] dropPoints = DropPointManager.Instance.GetPlayerTwoDropPoints();
        if (dropPoints.Length >= 4)
        {
            //todo �v���C���[�̑傫���ɂ���ăI�t�Z�b�g���ς��
            Vector3 endPoint1 = transform.position + transform.forward * offset;
            Vector3 endPoint2 = dropPoints[dropPoints.Length - 1];
            int endIndex = dropPoints.Length - 2;
            if (transform.position == dropPoints[dropPoints.Length - 1])
            {
                endPoint2 = dropPoints[dropPoints.Length - 2];
                endIndex--;
            }
            for (int i = 0; i < endIndex; ++i)
            {
                if (VectorMath.IsParallel(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1))
                {
                    continue;
                }
                float pointPos1 = VectorMath.PointOfLine(dropPoints[i], endPoint2, endPoint1);
                float pointPos2 = VectorMath.PointOfLine(dropPoints[i + 1], endPoint2, endPoint1);
                float pointPos3 = VectorMath.PointOfLine(endPoint2, dropPoints[i], dropPoints[i + 1]);
                float pointPos4 = VectorMath.PointOfLine(endPoint1, dropPoints[i], dropPoints[i + 1]);

                if (pointPos1 * pointPos2 < 0 && pointPos3 * pointPos4 < 0)
                {
                    Debug.Log("�s���S����");
                    Vector3 crossPoint = VectorMath.GetCrossPoint(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1);
                    List<Vector3> verts = new List<Vector3>();
                    for(int j = i+1;j< dropPoints.Length;j++)
                    {
                        verts.Add(dropPoints[j]);
                    }
                    verts.Add(crossPoint);
                    PolygonPaintManager.Instance.Paint(verts.ToArray(), 2, Global.PLAYER_TWO_TRACE_COLOR);
                    isPainting = true;
                    DropPointManager.Instance.ClearPlayerTwoDropPoints();
                    p2dc.ClearTrail();
                    p2dc.ResetTrail();
                    break;
                }
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.playerTwo = gameObject;
        p2dc = GetComponent<Player2DropControl>();
    }

}
