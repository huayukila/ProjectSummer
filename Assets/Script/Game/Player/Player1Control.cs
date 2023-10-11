using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player1DropControl))]
public class Player1Control : Player
{
    private Player1DropControl p1dc;

    /// <summary>
    /// 当たったときの処理諸々
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // 別のプレイヤーのDropPointに当たったら
        if(other.gameObject.CompareTag("DropPoint2"))
        {
            
            if(IsGotSilk)
            {
                dropSilkEvent.pos = transform.position;
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
            SetDeadStatus();
        }
        // 金の網に当たったら
        else if (other.gameObject.CompareTag("GoldenSilk"))
        {
            playerImage.color = Color.yellow;
            IsGotSilk = true;
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // ゴールに当たったら
        else if (other.gameObject.CompareTag("Goal"))
        {
            // 自分が金の網を持っていたら
            if(IsGotSilk)
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = 1
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
        // DropPointを全て消す
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
        p1dc.ResetTrail();
        transform.position = Global.PLAYER1_START_POSITION * 100.0f;
    }

    protected override void GroundColorCheck()
    {
        // 自分の領域にいたら
        if(colorCheck.isTargetColor(Global.PLAYER_ONE_TRACE_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // 別のプレイヤーの領域にいたら
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
        // 描画すべき領域の頂点を取得する
        List<Vector3> verts = DropPointManager.Instance.GetPlayerOnePaintablePointVector3(ob);
        verts.Add(transform.position);
        // 領域を描画する　※１はプレイヤー１を指す
        PolygonPaintManager.Instance.Paint(verts.ToArray(),1, Global.PLAYER_ONE_TRACE_COLOR);
        // DropPointを全て消す
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
        p1dc.ResetTrail();
    }

    protected override void CheckCanPaint()
    {
        Vector3[] dropPoints = DropPointManager.Instance.GetPlayerOneDropPoints();
        if(dropPoints.Length >= 4)
        {
            //todo プレイヤーの大きさによってオフセットが変わる
            Vector3 endPoint1 = transform.position + transform.forward * offset;
            Vector3 endPoint2 = dropPoints[dropPoints.Length - 1];
            int endIndex = dropPoints.Length - 2;
            if(endPoint1 == endPoint2)
            {
                endPoint2 = dropPoints[endIndex];
                endIndex--;
            }
            for (int i = 0; i < endIndex; ++i)
            {
                if (VectorMath.IsParallel(dropPoints[i], dropPoints[i+1],endPoint2,endPoint1))
                {
                    continue;
                }

                float pointPos1 = VectorMath.PointOfLine(dropPoints[i], endPoint2, endPoint1);
                float pointPos2 = VectorMath.PointOfLine(dropPoints[i + 1], endPoint2, endPoint1);
                float pointPos3 = VectorMath.PointOfLine(endPoint2, dropPoints[i], dropPoints[i + 1]);
                float pointPos4 = VectorMath.PointOfLine(endPoint1, dropPoints[i], dropPoints[i + 1]);

                if (pointPos1 * pointPos2 < 0 && pointPos3 * pointPos4 < 0)
                {
                    isPainting = true;
                    Vector3 crossPoint = VectorMath.GetCrossPoint(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1);
                    List<Vector3> verts = new List<Vector3>();
                    for (int j = i + 1; j < dropPoints.Length; j++)
                    {
                        verts.Add(dropPoints[j]);
                    }
                    verts.Add(crossPoint);
                    PolygonPaintManager.Instance.Paint(verts.ToArray(), 1, Global.PLAYER_ONE_TRACE_COLOR);
                    DropPointManager.Instance.ClearPlayerOneDropPoints();
                    p1dc.ClearTrail();
                    p1dc.ResetTrail();
                    break;
                }
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.playerOne = gameObject;
        p1dc = GetComponent<Player1DropControl>();
    }

    
}
