using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public float maxMoveSpeed;                  // プレイヤーの最大速度
    [Min(0.0f)] public float acceleration;      // プレイヤーの加速度
    [Min(0.0f)] public float rotationSpeed;     // プレイヤーの回転速度
    [SerializeField]Color32 _areaColor;         // 領域または移動した跡の色

    protected bool isPainting;                  // 地面に描けるかどうかの信号 
    private Rigidbody _rigidbody;               // プレイヤーのRigidbody
    protected ColorCheck colorCheck;            // カラーチェックコンポネント
    protected DropSilkEvent dropSilkEvent;
    protected PickSilkEvent pickSilkEvent;

    private Timer _paintableTimer;              // 領域を描く間隔を管理するタイマー
    private float _currentMoveSpeed;            // プレイヤーの現在速度
    private float _moveSpeedCoefficient;        // プレイヤーの移動速度の係数

    /// <summary>
    /// 衝突があったとき処理する
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // 衝突したら死亡状態に設定する
        SetDeadStatus();
        // 死亡したプレイヤーは金の網を持っていたら
        if (ScoreItemManager.Instance.IsGotSilk(gameObject))
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                dropSilkEvent.dropMode = DropMode.Edge;
            }
            TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);

        }

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
            dropMode = DropMode.Standard
        };
        pickSilkEvent = new PickSilkEvent()
        {
            player = gameObject
        };
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

    }

    // プレイヤーを復活させる
    public void Respawn()
    {
        if(!gameObject.activeSelf)
        {
            _currentMoveSpeed = 0.0f;
            ResetPlayerTransform();
            gameObject.SetActive(true);
            gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
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
