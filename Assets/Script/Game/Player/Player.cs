using UnityEngine;

public abstract class Player : MonoBehaviour
{
    [SerializeField]float maxMoveSpeed;                 // プレイヤーの最大速度
    [Min(0.0f)][SerializeField]float acceleration;      // プレイヤーの加速度
    [Min(0.0f)][SerializeField]float rotationSpeed;     // プレイヤーの回転速度
    [SerializeField]Color32 _areaColor;                 // 領域または移動した跡の色

    protected bool isPainting;                          // 地面に描けるかどうかの信号   
    protected ColorCheck colorCheck;                    // カラーチェックコンポネント
    protected DropSilkEvent dropSilkEvent;              // 金の網を落とすイベント
    protected PickSilkEvent pickSilkEvent;              // 金の網を拾うイベント

    private Timer _paintableTimer;                      // 領域を描く間隔を管理するタイマー
    private float _currentMoveSpeed;                    // プレイヤーの現在速度
    private float _moveSpeedCoefficient;                // プレイヤーの移動速度の係数
    private Rigidbody _rigidbody;                       // プレイヤーのRigidbody
    public bool IsGotSilk { get; protected set; }

    /// <summary>
    /// 衝突があったとき処理する
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // 衝突したら死亡状態に設定する

        // 死亡したプレイヤーは金の網を持っていたら
        if (IsGotSilk)
        {
            dropSilkEvent.pos = transform.position;
            if (collision.gameObject.CompareTag("Wall"))
            {
                dropSilkEvent.dropMode = DropMode.Edge; 
            }
            TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
        }
        SetDeadStatus();
    }

    protected virtual void Awake()
    {
        isPainting = false;
        _currentMoveSpeed = 0.0f;
        _rigidbody = GetComponent<Rigidbody>();
        colorCheck = gameObject.AddComponent<ColorCheck>();
        colorCheck.layerMask = LayerMask.GetMask("Ground");
        _moveSpeedCoefficient = 1.0f;
        maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        acceleration = Global.PLAYER_ACCELERATION;
        rotationSpeed = Global.PLAYER_ROTATION_SPEED;
        dropSilkEvent = new DropSilkEvent()
        {
            dropMode = DropMode.Standard,
        };
        pickSilkEvent = new PickSilkEvent();

        gameObject.GetComponent<Renderer>().material.color = Color.black;
        IsGotSilk = false;
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
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _currentMoveSpeed = 0.0f;
        IsGotSilk = false;
        PlayerRespawnEvent playerRespawnEvent = new PlayerRespawnEvent()
        {
            player = gameObject
        };
        TypeEventSystem.Instance.Send<PlayerRespawnEvent>(playerRespawnEvent);

    }

    public Color32 GetAreaColor() => _areaColor;

    public void SetAreaColor(Color32 color)
    {
        _areaColor = color;
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
    /// 地面の色をチェックする
    /// </summary>
    protected abstract void GroundColorCheck();

    /// <summary>
    /// 領域を描く
    /// </summary>
    /// <param name="object">当たった自分自身が落としたDropPoint</param>
    protected abstract void PaintArea(GameObject @object);
}
