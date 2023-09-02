using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Player
{
    float currentMoveSpeed;         // プレイヤーの現在速度
    GameObject rootTail;            // 尻尾の頭
    GameObject tipTail;             // 尻尾の尾

    public GameObject tailPrefab;
    public Paintable p;             // 地面

    bool isPainting;                // 地面に描けるかどうかの信号
    float timer;                    // 前回の描画が終わってからの経過時間

    private void FixedUpdate()
    {

        PlayerMovement();
    }

    /// <summary>
    /// 尻尾をインスタンス化する
    /// </summary>
    private void SetTail()
    {
        GameObject tail = Instantiate(tailPrefab, transform);

        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.parent = transform;
        tail.transform.localPosition = new Vector3(0.0f, 0.0f, -0.5f);
        tail.AddComponent<TailControl>();

        rootTail = tail;

        TailControl temp = rootTail.GetComponent<TailControl>();
        tipTail = temp?.GetTipTail();

    }

    private void OnTriggerEnter(Collider other)
    {
        // DropPointに当たったら
        if(other.gameObject.CompareTag("DropPoint") && !isPainting)
        {
            isPainting = true;

            // 描画すべき領域の頂点を取得する
            List<Vector3> verts = DropPointManager.Instance.GetPaintablePointVector3(other.gameObject);
            if(verts != null)
            {
                TailControl tc = rootTail.GetComponent<TailControl>();
                GameObject[] tails = tc?.GetTails();
                for (int i = 1; i < Global.iMAX_TAIL_COUNT + 1;++i)
                {
                    verts.Add(tails[^i].transform.position);
                }
            }
            verts.Add(transform.position);

            // 領域を描画する
            PolygonPaintManager.Instance.Paint(p, verts?.ToArray());
            // DropPointを消す
            DropPointManager.Instance.Clear();
        }
    }

    private void Awake()
    {
        isPainting = false;
        timer = 0.0f;
        currentMoveSpeed = 0.0f;
        SetTail();
        tipTail?.AddComponent<DropPointControl>();
    }

    // Update is called once per frame
    private void Update()
    {
        // 描画を制限する（α版）
        if (isPainting)
        {
            timer += Time.deltaTime;
        }
        if (timer >= 2.0f)
        {
            isPainting = false;
            timer = 0.0f;
        }
    }

    /// <summary>
    /// プレイヤーの移動を制御する
    /// </summary>
    private void PlayerMovement()
    {
        // 加速運動をして、最大速度まで加速する
        currentMoveSpeed = currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : currentMoveSpeed + acceleration * Time.deltaTime;

        Vector3 movementDirection = Vector3.forward * currentMoveSpeed;
        transform.Translate(movementDirection * Time.fixedDeltaTime);

    }

    /// <summary>
    /// プレイヤーの回転を制御する
    /// </summary>
    private void PlayerRotation()
    {
        // 方向入力を取得する
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 rotationDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotationDirection != Vector3.zero)
        {
            // 入力された方向へ回転する
            Quaternion rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }

    }
}
