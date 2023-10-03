using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        // 自分のDropPointに当たったら
        if (other.gameObject.CompareTag("DropPoint1") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        // 金の網に当たったら
        if(other.gameObject.CompareTag("GoldenSilk"))
        {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            IsGotSilk = true;
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // ゴールに当たったら
        if(other.gameObject.CompareTag("Goal"))
        {
            // 自分が金の網を持っていたら
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
        // DropPointを全て消す
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
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
        List<Vector3> verts = DropPointManager.Instance.GetPlayerOnePaintablePointVector3(ob.gameObject);
        verts.Add(transform.position);
        // 領域を描画する　※１はプレイヤー１を指す
        PolygonPaintManager.Instance.Paint(verts.ToArray(),1,GetAreaColor());
        // DropPointを全て消す
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
