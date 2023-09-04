using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class PlayerControl : Player
{
    float _currentMoveSpeed;         // プレイヤーの現在速度
    GameObject _rootTail;            // 尻尾の頭
    GameObject _tipTail;             // 尻尾の尾
    bool _isPainting;                // 地面に描けるかどうかの信号
    float _timer;                    // 前回の描画が終わってからの経過時間
    Rigidbody _rigidBody;   
    
    public GameObject tailPrefab;
    public Paintable p;             // 地面

    /// <summary>
    /// 尻尾をインスタンス化する
    /// </summary>
    private void SetTail()
    {
        GameObject tail = Instantiate(tailPrefab);

        tail.name = gameObject.name + "Tail";
        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.position = transform.position;
        tail.AddComponent<TailControl>();

        _rootTail = tail;

        TailControl tc = _rootTail.GetComponent<TailControl>();
        _tipTail = tc?.GetTipTail();

    }

    private void OnTriggerEnter(Collider other)
    {
        // DropPointに当たったら
        if(other.gameObject.CompareTag("DropPoint") && !_isPainting)
        {
            _isPainting = true;
            // 描画すべき領域の頂点を取得する
            List<Vector3> verts = DropPointManager.Instance.GetPaintablePointVector3(other.gameObject);
            if(verts != null)
            {
                TailControl tc = _rootTail.GetComponent<TailControl>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DropPoint"))
        {
            SphereCollider temp = collision.collider.gameObject.GetComponent<SphereCollider>();
            Debug.Log("DP");
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Wall");
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("BOOM!!!!!!!");
        }
    }
    private void Awake()
    {
        _isPainting = false;
        _timer = 0.0f;
        _currentMoveSpeed = 0.0f;
        SetTail();
        _tipTail?.AddComponent<DropPointControl>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        // 描画を制限する（α版）
        if (_isPainting)
        {
            _timer += Time.deltaTime;
        }
        if (_timer >= 0.5f)
        {
            _isPainting = false;
            _timer = 0.0f;
        }
    }
    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerRotation();
        _rootTail.transform.position = transform.position;
    }


    /// <summary>
    /// プレイヤーの移動を制御する
    /// </summary>
    private void PlayerMovement()
    {
        // 加速運動をして、最大速度まで加速する
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.deltaTime;

        Vector3 movementDirection = transform.forward * _currentMoveSpeed * Time.deltaTime;

        _rigidBody.velocity = movementDirection;

    }

    /// <summary>
    /// プレイヤーの回転を制御する
    /// </summary>
    private void PlayerRotation()
    {
        // 方向入力を取得する
        float horizontal = Input.GetAxis(name + "_Horizontal");
        float vertical = Input.GetAxis(name + "_Vertical");

        Vector3 rotationDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotationDirection != Vector3.zero)
        {
            // 入力された方向へ回転する
            Quaternion rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }

    }
}
