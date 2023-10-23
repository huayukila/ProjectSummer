using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
public enum PlayerStatus
{
    Fine,
    Dead
}
[RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
public abstract class Player : MonoBehaviour
{
    [SerializeField]
    float maxMoveSpeed;                 // プレイヤーの最大速度
    [Min(0.0f)][SerializeField]
    float acceleration;                 // プレイヤーの加速度
    [Min(0.0f)][SerializeField]
    float rotationSpeed;                // プレイヤーの回転速度

    protected bool isPainting;                          // 地面に描けるかどうかの信号   
    protected ColorCheck colorCheck;                    // カラーチェックコンポネント
    protected DropSilkEvent dropSilkEvent;              // 金の網を落とすイベント
    protected PickSilkEvent pickSilkEvent;              // 金の網を拾うイベント
    protected InputAction playerAction;
    private InputAction rotateAction;
    protected PlayerInput playerInput;
    protected PlayerStatus status;
    protected float offset;

    private Timer _paintableTimer;                      // 領域を描く間隔を管理するタイマー
    private float _currentMoveSpeed;                    // プレイヤーの現在速度
    private float _moveSpeedCoefficient;                // プレイヤーの移動速度の係数
    private Rigidbody _rigidbody;                       // プレイヤーのRigidbody
    private Vector3 _rotateDirection;
    private GameObject _particleObject;
    private GameObject _particlePrefab;
    private ParticleSystem _pS;
    private ParticleSystem.MainModule _pSMain;
    private Timer _mBoostCoolDown;
    private float _boostDurationTime = Global.BOOST_DURATION_TIME;
    private bool _isBoosting = false;
    private SpriteRenderer _mSpriteRenderer;
    private SpriteRenderer _mShadowSpriteRenderer;
    private GameObject _explosionPrefab;
    private GameObject _bigSpider;
    private LineRenderer _mBigSpiderLineRenderer;
    private GameObject _mShadow;
    //todo
    protected GameObject _mGotSilkImage;

    //todo refactorying
    protected Vector3 _mRespawnPos;
    private string _mName;
    private int _mID;
    private Color _mColor;

    private Timer _respawnAnimationTimer;

    public bool IsGotSilk { get; protected set; }
    //todo
    private int _mSilkCount;

    public void SetProperties(int ID , Color color, string name = "Player")
    {
        if(_mName == null)
        {
            _mName = name;
            this.name = _mName;
            _mID = ID;
            _mColor = color;
        }
    }
    /// <summary>
    /// 衝突があったとき処理する
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // 死亡したプレイヤーは金の網を持っていたら
        if (IsGotSilk == true)
        {
            // 金の網のドロップ場所を設定する
            dropSilkEvent.pos = transform.position;
            // 壁にぶつかったら
            if (collision.gameObject.CompareTag("Wall"))
            {
                dropSilkEvent.dropMode = DropMode.Edge; 
            }
            TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
        }
        // 衝突したら死亡状態に設定する
        SetDeadStatus();
    }

    protected virtual void Awake()
    {
        // 初期化処理
        Init();
    }
    private void Update()
    {
        // プレイヤー画像をずっと同じ方向に向くことにする
        _mSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // 通常状態じゃないと復活アニメーションを処理する
        if (status != PlayerStatus.Fine　|| _respawnAnimationTimer != null)
        {
            // 復活アニメーションの処理
            UpdateRespawnAnimation();
            if (_respawnAnimationTimer.IsTimerFinished())
            {
                ResetRespawnAnimation();
            }
            return;
        }
        // 通常状態の処理
        UpdateFine();


    }

    private void UpdateFine()
    {
        // エフェクトスタート
        if (_pS.isStopped)
        {
            _pS.Play();
        }
        // エフェクトの更新
        {
            _pSMain.startSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 2.0f;
            _pSMain.simulationSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 4.0f + 1.0f;
            _pSMain.startLifetime = _pSMain.simulationSpeed * 0.5f;

        }

        // プレイヤー画像の向きを変える
        if (transform.forward.x < 0.0f)
        {
            _mSpriteRenderer.flipX = false;
        }
        else
        {
            _mSpriteRenderer.flipX = true;
        }

        // 金の網のを持っていれば、プレイヤー画像の上に表示する
        if(IsGotSilk == true)
        {
            _mGotSilkImage.transform.position = transform.position + Vector3.forward * 6.5f;
        }

        // プレイヤーインプットを取得する
        Vector2 rotateInput = rotateAction.ReadValue<Vector2>();
        // 回転方向を決める
        _rotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);

        // プレイヤーがいるところの地面の色をチェックする
        GroundColorCheck();
        // 描画を制限する（α版）
        if (!isPainting)
        {
            // 陣取りができるかどうかをチェックする
            CheckCanPaint();
        }
        else
        {
            // 一回の陣取りにつき0.3秒の間隔を設ける
            if (_paintableTimer == null)
            {
                _paintableTimer = new Timer();
                _paintableTimer.SetTimer(0.3f,
                    () =>
                    {
                        isPainting = false;
                    }
                    );
            }
            if (_paintableTimer.IsTimerFinished())
            {
                _paintableTimer = null;
            }
        }

        //TODO ブースト
        if(_mBoostCoolDown != null)
        {
            _boostDurationTime -= Time.deltaTime;
            if(_boostDurationTime <= 0.0f)
            {
                maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            }
            if(_mBoostCoolDown.IsTimerFinished())
            {
                _mBoostCoolDown = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if(status != PlayerStatus.Fine)
        {
            return;
        }
        // プレイヤーの動き
        PlayerMovement();
        // プレイヤーの回転
        PlayerRotation();
    }

    /// <summary>
    /// プレイヤーのプロパティを初期化する
    /// </summary>
    private void Init()
    {
        isPainting = false;
        _currentMoveSpeed = 0.0f;
        _rigidbody = GetComponent<Rigidbody>();
        colorCheck = GetComponent<ColorCheck>();
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

        playerInput = GetComponent<PlayerInput>();
        rotateAction = playerInput.actions["Rotate"];
        playerAction = playerInput.actions["Boost"];
        playerAction.performed += OnBoost;
        status = PlayerStatus.Fine;
        offset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _particlePrefab = Resources.Load("Prefabs/DustParticlePrefab") as GameObject;
        _particleObject = Instantiate(_particlePrefab, transform);
        _particleObject.transform.localPosition = Vector3.zero;
        _particleObject.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        _pS = _particleObject.GetComponent<ParticleSystem>();
        _pSMain = _pS.main;
        _pSMain.startSize = 0.4f;
        _pSMain.startColor = Color.gray;

        _explosionPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
        _bigSpider = Instantiate(GameManager.Instance.bigSpiderPrefab,_mRespawnPos,Quaternion.identity);
        _bigSpider.transform.position = _mRespawnPos + new Vector3(0.0f,0.0f,100.0f);
        _bigSpider.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        _mBigSpiderLineRenderer = _bigSpider.GetComponentInChildren<LineRenderer>();
        _mBigSpiderLineRenderer.positionCount = 2;
        _mBigSpiderLineRenderer.startWidth = 0.2f;
        _mBigSpiderLineRenderer.endWidth = 0.2f;

        _mGotSilkImage = Instantiate(Resources.Load("Prefabs/GoldenSilkImage") as GameObject);
        _mGotSilkImage.SetActive(false);

        // プレイヤー自分の画像のレンダラーを取得する
        _mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // 表示順位を変換する
        _mSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
        // 復活するとき現れる影のプレハブをインスタンス化する
        _mShadow = Instantiate(Resources.Load("Prefabs/PlayerShadow") as GameObject, _mRespawnPos, Quaternion.identity);
        _mShadow.transform.localScale = Vector3.zero;
        //TODO 影の方向を変える
        _mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // 影を透明にする
        _mShadowSpriteRenderer = _mShadow.GetComponent<SpriteRenderer>();
        _mShadowSpriteRenderer.color = Color.clear;

        _mName = null;


    }
    /// <summary>
    /// プレイヤーの移動を制御する
    /// </summary>
    private void PlayerMovement()
    {
        // 加速運動をして、最大速度まで加速する
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.fixedDeltaTime;
        // 前向きの移動をする
        Vector3 moveDirection = transform.forward * _currentMoveSpeed * Time.fixedDeltaTime * _moveSpeedCoefficient;
        _rigidbody.velocity = moveDirection;

    }

    /// <summary>
    /// プレイヤーの死亡状態を設定する
    /// </summary>
    protected virtual void SetDeadStatus()
    {
        // 爆発エフェクト
        {
            // 爆発アニメーションをインスタンス化する
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
            // 爆発の効果音を流す
            AudioManager.Instance.PlayFX("BoomFX", 0.7f);
        }

        // プレイヤーの状態をリセットする
        ResetPlayerStatus();
        // コンポネントを無効化にする
        GetComponent<DropPointControl>().enabled = false;
        GetComponentInChildren<TrailRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // プレイヤー復活イベントを喚起する
        PlayerRespawnEvent playerRespawnEvent = new PlayerRespawnEvent()
        {
            player = gameObject
        };
        TypeEventSystem.Instance.Send<PlayerRespawnEvent>(playerRespawnEvent);

        // エフェクトを止める
        if(_pS.isPlaying)
        {
            _pS.Stop();
        }

        // 復活アニメーションを初期化する
        _mBigSpiderLineRenderer.positionCount = 2;
        _respawnAnimationTimer = new Timer();
        _respawnAnimationTimer.SetTimer(Global.RESPAWN_TIME,
            () =>
            {
                _respawnAnimationTimer = null;
            });
        _mGotSilkImage.SetActive(false);
    }

    //todo アクセス修飾子の変更予定
    public PlayerStatus GetStatus() => status;
    public int GetID() => _mID;
    public Color GetColor() => _mColor;
    public void SetStatus(PlayerStatus status)
    {
        this.status = status;
    }


    /// <summary>
    /// プレイヤーのリジッドボディを回転させる
    /// </summary>
    /// <param name="quaternion">回転する方向</param>
    private void RotateRigidbody(Quaternion quaternion)
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
    private void PlayerRotation()
    {
        // 方向入力を取得する
        if (_rotateDirection != Vector3.zero)
        {
            // 入力された方向へ回転する
            Quaternion rotation = Quaternion.LookRotation(_rotateDirection, Vector3.up);
            RotateRigidbody(rotation);
        }
    }

    /// <summary>
    /// 復活アニメーションをリセットする
    /// </summary>
    private void ResetRespawnAnimation()
    {
        _bigSpider.transform.position = _mRespawnPos + new Vector3(0.0f,0.0f,100.0f);
        _mBigSpiderLineRenderer.positionCount = 0;
        _mShadow.transform.localScale = Vector3.zero;
        _mShadowSpriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// 復活アニメーションを更新する関数
    /// </summary>
    private void UpdateRespawnAnimation()
    {
        // 復活アニメーション前半部分の処理
        if(_respawnAnimationTimer.GetTime() >= Global.RESPAWN_TIME /2.0f)
        {
            _bigSpider.transform.Translate(new Vector3(0.0f, 0.0f,-20.0f * Time.deltaTime),Space.World);
            transform.position = _bigSpider.transform.position + new Vector3(0.0f,0.5f,0.0f);
        }
        // 復活アニメーション後半部分の処理
        else
        {
            //TODO
            transform.Translate(-(_bigSpider.transform.position - _mRespawnPos) * 0.4f * Time.deltaTime,Space.World);
            transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
            _mShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
            _mShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
            Vector3[] temp = new Vector3[2];
            temp[0] = _bigSpider.transform.position;
            temp[1] = transform.position + new Vector3(0.0f,-0.5f,offset);
            _mBigSpiderLineRenderer.SetPositions(temp);
        }
    }

    /// <summary>
    /// プレイヤーのステイタスをリセットする
    /// </summary>
    private void ResetPlayerStatus()
    {
        transform.position = _bigSpider.transform.position;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        status = PlayerStatus.Dead;
        transform.localScale = Vector3.one;
        _currentMoveSpeed = 0.0f;
        IsGotSilk = false;
        _isBoosting = false;
        _boostDurationTime = Global.BOOST_DURATION_TIME;
        _mBoostCoolDown = null;
    }
    /// <summary>
    /// 地面の色をチェックする
    /// </summary>
    protected abstract void GroundColorCheck();

    protected abstract void CheckCanPaint();

    /// <summary>
    /// 領域を描く
    /// </summary>
    /// <param name="object">当たった自分自身が落としたDropPoint</param>
    protected abstract void PaintArea(GameObject @object);

    private void OnEnable()
    {
        rotateAction.Enable();
        playerAction.Enable();
    }
    private void OnDisable()
    {
        rotateAction.Disable();
        playerAction.Disable();
    }
    private void OnDestroy()
    {
        playerAction.performed -= OnBoost;
        Resources.UnloadUnusedAssets();
    }
    private void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_isBoosting == false)
            {
                maxMoveSpeed *= 1.5f;
                _currentMoveSpeed = maxMoveSpeed;
                _isBoosting = true;
                _mBoostCoolDown = new Timer();
                _mBoostCoolDown.SetTimer(Global.BOOST_COOLDOWN_TIME,
                    () =>
                    {
                        _boostDurationTime = Global.BOOST_DURATION_TIME;
                        _isBoosting = false;
                    });
            }
        }
    }

}
