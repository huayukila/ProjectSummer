using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public float maxMoveSpeed;                  // プレイヤーの最大速度
    [Min(0.0f)] public float acceleration;      // プレイヤーの加速度
    [Min(0.0f)] public float rotationSpeed;     // プレイヤーの回転速度
    public Color32 areaColor;                   // 領域または移動した跡の色

    protected GameObject rootTail;              // 尻尾の頭
    protected GameObject tipTail;               // 尻尾の尾
    protected bool isPainting;                  // 地面に描けるかどうかの信号 
    private Rigidbody _rigidbody;               // プレイヤーのRigidbody
    protected ColorCheck colorCheck;            // カラーチェックコンポネント

    private Timer _paintableTimer;              // 領域を描く間隔を管理するタイマー
    private float _currentMoveSpeed;            // プレイヤーの現在速度
    private float _moveSpeedCoefficient;        // プレイヤーの移動速度の係数
    private GameObject _tailPrefab;             // 尻尾のプレハブ

    /// <summary>
    /// 尻尾をインスタンス化する
    /// </summary>
    private void SetTail()
    {
        GameObject tail = Instantiate(_tailPrefab);
        tail.name = gameObject.name + "Tail";
        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.position = transform.position;
        tail.AddComponent<TailControl>();
        rootTail = tail;
        TailControl tc = rootTail.GetComponent<TailControl>();
        tipTail = tc.GetTipTail();

    }

    /// <summary>
    /// 衝突があったとき処理する
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // 他のプレイヤー　もしくは　壁と衝突したら
        bool isCollision = collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall");
        if (isCollision)
        {
            SetDeadStatus();
        }

    }

    protected virtual void Awake()
    {
        _tailPrefab = (GameObject)Resources.Load("Prefabs/Tail");
        isPainting = false;
        _currentMoveSpeed = 0.0f;
        SetTail();
        _rigidbody = GetComponent<Rigidbody>();
        colorCheck = gameObject.AddComponent<ColorCheck>();
        colorCheck.layerMask = LayerMask.GetMask("Ground");
        _moveSpeedCoefficient = 1.0f;
    }

    private void Update()
    {
        // 描画を制限する（α版）
        if (isPainting)
        {
            if(_paintableTimer == null)
            {
                _paintableTimer = new Timer();
                _paintableTimer.SetTimer(0.5f,
                    () =>
                    {
                        isPainting = false;
                    }
                    );
            }
            if(_paintableTimer.IsTimerFinished())
            {
                _paintableTimer = null;
            }
        }
        GroundColorCheck();

    }
    protected virtual void FixedUpdate()
    {
        PlayerMovement();
        rootTail.transform.position = transform.position;
    }

    /// <summary>
    /// プレイヤーの移動を制御する
    /// </summary>
    private void PlayerMovement()
    {
        // 加速運動をして、最大速度まで加速する
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.deltaTime;
        Vector3 moveDirection = transform.forward * _currentMoveSpeed * Time.fixedDeltaTime * _moveSpeedCoefficient;
        _rigidbody.velocity = moveDirection;
    }

    /// <summary>
    /// プレイヤーの死亡状態を設定する
    /// </summary>
    protected virtual void SetDeadStatus()
    {
        gameObject.SetActive(false);
        rootTail.GetComponent<TailControl>().SetDeactiveProperties();
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // 死亡したプレイヤーは金の網を持っていたら
        if(ScoreItemManager.Instance.IsGotSilk(gameObject))
        {
            ScoreItemManager.Instance.DropGoldenSilk();
        }

    }

    // プレイヤーを復活させる
    public void Respawn()
    {
        if(!gameObject.activeSelf)
        {
            _currentMoveSpeed = 0.0f;
            ResetPlayerTransform();
            rootTail.GetComponent<TailControl>().SetActiveProperties(transform.position);
            gameObject.SetActive(true);
            gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    /// <summary>
    /// プレイヤーのリジッドボディを回転させる
    /// </summary>
    /// <param name="quaternion">回転する方向</param>
    protected void RotateRigidbody(Quaternion quaternion)
    {
        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotationSpeed * Time.fixedDeltaTime);
    }    
    
    /// <summary>
    /// 移動速度の係数を設定する
    /// </summary>
    /// <param name="coefficient"></param>
    protected void SetMoveSpeedCoefficient(float coefficient)
    {
        _moveSpeedCoefficient = coefficient;
    }
    /// <summary>
    /// プレイヤーの回転を制御する
    /// </summary>
    protected abstract void PlayerRotation();

    /// <summary>
    /// プレイヤーの位置をリセットする
    /// </summary>
    protected abstract void ResetPlayerTransform();

    /// <summary>
    /// 地面の色をチェックする
    /// </summary>
    protected abstract void GroundColorCheck();

    /// <summary>
    /// 領域を描く
    /// </summary>
    /// <param name="object">当たった自分自身が落としたDropPoint</param>
    protected abstract void PaintArea(GameObject @object);
}
