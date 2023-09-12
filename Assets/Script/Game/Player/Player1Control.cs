using System.Collections.Generic;
using UnityEngine;

public class Player1Control : Player
{
    private Player1DropControl p1dc;
    protected override void PlayerRotation()
    {
        // 方向入力を取得する
        float horizontal = Input.GetAxis("Player1_Horizontal");
        float vertical = Input.GetAxis("Player1_Vertical");

        Vector3 rotateDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotateDirection != Vector3.zero)
        {
            // 入力された方向へ回転する
            Quaternion rotation = Quaternion.LookRotation(rotateDirection, Vector3.up);
            RotateRigidbody(rotation);          
        }

    }

    /// <summary>
    /// 当たったときの処理諸々
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // 別のプレイヤーの尻尾に当たったら
        if(other.gameObject.CompareTag("Player2Tail"))
        {
            SetDeadStatus();
        }
        // 自分のDropPointに当たったら
        if (other.gameObject.CompareTag("DropPoint1") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        if(other.gameObject.CompareTag("DropPoint2"))
        {
            SetDeadStatus();
        }
        // 金の網に当たったら
        if(other.gameObject.CompareTag("GoldenSilk"))
        {
            ScoreItemManager.Instance.SetGotSilkPlayer(gameObject);
        }
        // ゴールに当たったら
        if(other.gameObject.CompareTag("Goal"))
        {
            // 自分が金の網を持っていたら
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
        // DropPointを全て消す
        //tipTail.GetComponent<Player1DropControl>().ClearTrail();
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
    }

    protected override void GroundColorCheck()
    {
        // 自分の領域にいたら
        if(colorCheck.isTargetColor(areaColor))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // 別のプレイヤーの領域にいたら
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
        // 描画すべき領域の頂点を取得する
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

        // 領域を描画する
        PolygonPaintManager.Instance.Paint(verts.ToArray(), areaColor);
        // DropPointを全て消す
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
