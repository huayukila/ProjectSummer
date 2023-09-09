using System.Collections.Generic;
using UnityEngine;

public class Player2Control : Player
{
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player1Tail"))
        {
            SetDeadStatus();
        }
        // DropPointに当たったら
        if (other.gameObject.CompareTag("DropPoint2") && !isPainting)
        {
            PaintArea(other.gameObject);
        }
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            ScoreItemManager.Instance.SetGotSilkPlayer(gameObject);
        }
        if (other.gameObject.CompareTag("Goal"))
        {
            if (ScoreItemManager.Instance.IsGotSilk(gameObject))
            {
                ScoreItemManager.Instance.SetReachGoalProperties();
                gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        rootTail.GetComponent<TailControl>().SetTailsTag("Player2Tail");
        tipTail.AddComponent<Player2DropControl>();
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
        if (colorCheck.isTargetColor(Global.PLAYER_TWO_COLOR))
        {
            SetMoveSpeedCoefficient(Global.SPEED_UP_COEFFICIENT);
        }
        else if(colorCheck.isTargetColor(Global.PLAYER_ONE_COLOR))
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

        // 領域を描画する
        PolygonPaintManager.Instance.Paint(verts.ToArray(), Global.PLAYER_TWO_COLOR);
        // DropPointを消す
        DropPointManager.Instance.ClearPlayerTwoDropPoints();
        tipTail.GetComponent<Player2DropControl>().ClearTrail();

    }
}
