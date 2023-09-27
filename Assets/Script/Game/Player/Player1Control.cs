using System.Collections.Generic;
using UnityEngine;

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
            SetDeadStatus();
            if(ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
            }
        }
        // 自分のDropPointに当たったら
        if (other.gameObject.CompareTag("DropPoint1") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        // 金の網に当たったら
        if(other.gameObject.CompareTag("GoldenSilk"))
        {
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
        }
        // ゴールに当たったら
        if(other.gameObject.CompareTag("Goal"))
        {
            // 自分が金の網を持っていたら
            if(ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = 1
                };
                TypeEventSystem.Instance.Send<AddScoreEvent>(AddScoreEvent);
                gameObject.GetComponent<Renderer>().material.color = Color.black;
            }
        }
    }

    protected override void PlayerRotation()
    {   
        
        float horizontal = 0.0f;
        float vertical = 0.0f;
        var controller = Input.GetJoystickNames()[0];
        if (!string.IsNullOrEmpty(controller))
        { 
            horizontal = Input.GetAxis("1L_Joystick_H");
            vertical = Input.GetAxis("1L_Joystick_V");
        }
        else
        {
            horizontal = Input.GetAxis("Player1_Horizontal");
            vertical = Input.GetAxis("Player1_Vertical");

        }
        // 方向入力を取得する

        Vector3 rotateDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotateDirection != Vector3.zero)
        {
            // 入力された方向へ回転する
            Quaternion rotation = Quaternion.LookRotation(rotateDirection, Vector3.up);
            RotateRigidbody(rotation);
        }

    }

    protected override void SetDeadStatus()
    {
        base.SetDeadStatus();
        // DropPointを全て消す
        DropPointManager.Instance.ClearPlayerOneDropPoints();
        p1dc.ClearTrail();
    }

    protected override void GroundColorCheck()
    {
        // 自分の領域にいたら
        if(colorCheck.isTargetColor(GetAreaColor()))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        // 別のプレイヤーの領域にいたら
        else if(colorCheck.isTargetColor(GameManager.Instance.playerTwo.GetComponent<Player2Control>().GetAreaColor()))
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
