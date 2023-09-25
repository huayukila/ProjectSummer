using System.Collections.Generic;
using UnityEngine;

public class Player2Control : Player
{
    private Player2DropControl p2dc;
    protected override void PlayerRotation()
    {
        // 方向入力を取得する
        float horizontal = Input.GetAxis("Player2_Horizontal");
        float vertical = Input.GetAxis("Player2_Vertical");

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
        // 別のプレイヤーのDropPointに当たったら
        if (other.gameObject.CompareTag("DropPoint1"))
        {
            SetDeadStatus();
            if (ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
        }
        // DropPointに当たったら
        if (other.gameObject.CompareTag("DropPoint2") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        // 金の網に当たったら
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // ゴールに当たったら
        if (other.gameObject.CompareTag("Goal"))
        {
            // 自分が金の網を持っていたら
            if (ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = 2
                };
                TypeEventSystem.Instance.Send<AddScoreEvent>(AddScoreEvent);
                gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }

    protected override void ResetPlayerTransform()
    {
        transform.position = Global.PLAYER2_START_POSITION;
        transform.forward = Vector3.back;
    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();
    }

    protected override void GroundColorCheck()
    {
        // 自分の領域にいたら
        if (colorCheck.isTargetColor(GetAreaColor()))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // 別のプレイヤーの領域にいたら
        else if (colorCheck.isTargetColor(GameManager.Instance.playerOne.GetComponent<Player1Control>().GetAreaColor()))
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
        List<Vector3> verts = DropPointManager.Instance.GetPlayerTwoPaintablePointVector3(ob.gameObject);
        verts.Add(transform.position);
        // 領域を描画する
        PolygonPaintManager.Instance.Paint(verts.ToArray(), GetAreaColor());
        // DropPointを全て消す
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        p2dc.ClearTrail();

    }

    protected override void Awake()
    {
        base.Awake();
        p2dc = gameObject.AddComponent<Player2DropControl>();
    }

    private void Start()
    {
        GameManager.Instance.playerTwo = gameObject;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerRotation();
    }
}
