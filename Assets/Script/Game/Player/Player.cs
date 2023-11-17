using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public enum PlayerStatus
{
    Fine,
    Dead,
    Invincible
}
[RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float maxMoveSpeed;                         // プレイヤーの最大速度
    [Min(0.0f)][SerializeField]
    private float acceleration;                         // プレイヤーの加速度
    [Min(0.0f)][SerializeField]
    private float rotationSpeed;                        // プレイヤーの回転速度

    private ColorCheck colorCheck;                      // カラーチェックコンポネント
    private DropSilkEvent dropSilkEvent;                // 金の網を落とすイベント
    private PickSilkEvent pickSilkEvent;                // 金の網を拾うイベント
    //TODO 二つを一つにする
    private InputAction boostAction;                    // プレイヤーのブースト入力
    private InputAction rotateAction;                   // プレイヤーの回転入力
    private PlayerInput playerInput;                    // playerInputAsset
    private PlayerStatus _mStatus;                      // プレイヤーのステータス
    private float _mColliderOffset;                     // プレイヤーコライダーの長さ（正方形）
    private float _currentMoveSpeed;                    // プレイヤーの現在速度
    private float _moveSpeedCoefficient;                // プレイヤーの移動速度の係数
    private Rigidbody _rigidbody;                       // プレイヤーのRigidbody
    private Vector3 _rotateDirection;                   // プレイヤーの回転方向

    //TODO コメント付く
    private GameObject _particleObject;                 // パーティクルシステムが入っているオブジェクト
    private ParticleSystem _mParticleSystem;            // パーティクルシステム
    private ParticleSystem.MainModule _pSMain;          // パーティクルシステムのプロパティを設定するための変数
    private Timer _mBoostCoolDown;                      // ブーストタイマー（隠れ仕様）
    private float _boostDurationTime;                   // ブースト持続時間（隠れ仕様）
    private bool _isBoosting = false;                   // ブーストしているかのフラグ
    private SpriteRenderer _mImageSpriteRenderer;       // プレイヤー画像のSpriteRenderer
    private GameObject _mShadow;                        // プレイヤーが復活する時の影
    private SpriteRenderer _mShadowSpriteRenderer;      // プレイヤーが復活する時の影のSpriteRenderer
    private GameObject _bigSpider;                      // プレイヤーが復活するする時の大きい蜘蛛
    private Timer _respawnAnimationTimer;               // プレイヤー復活用タイマー
    private LineRenderer _mBigSpiderLineRenderer;       // プレイヤー復活する時の空中投下する時に繋がっている糸

    private GameObject _explosionPrefab;                // 爆発アニメーションプレハブ  
    //todo
    private GameObject _mGotSilkImage;                  // 金の糸を持っていることを示す画像

    //todo refactorying
    private Vector3 _mRespawnPos;                       // 復活する場所
    private int _mID;                                   // プレイヤーID
    private Color _mColor;                              // プレイヤーの領域の色
    private DropPointControl _mDropPointControl;        // プレイヤーのDropPointControl

    public bool IsGotSilk { get; private set; }         // プレイヤーが金の糸を持っているかのフラグ
    //TODO
    private int _mSilkCount;                            // プレイヤーが持っている金の糸の数

    private void Awake()
    {
        // 初期化処理
        Init();
    }
    private void Update()
    {
        // プレイヤー画像をずっと同じ方向に向くことにする
        _mImageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // 通常状態じゃないと復活アニメーションを処理する
        if (_mStatus != PlayerStatus.Fine || _respawnAnimationTimer != null)
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

    private void FixedUpdate()
    {
        // プレイヤーが「通常」状態じゃないと動きに関する関数を実行しない
        if (_mStatus != PlayerStatus.Fine)
        {
            return;
        }
        // プレイヤーの動き
        PlayerMovement();
        // プレイヤーの回転
        PlayerRotation();
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
            // 金の糸のドロップ場所を設定する
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Contains("DropPoint"))
        {
            if(other.gameObject.tag.Contains(_mID.ToString()) == false)
            {
                if (IsGotSilk)
                {
                    dropSilkEvent.pos = transform.position;
                    TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
                }
                SetDeadStatus();
            }
        }
        // 金の糸に当たったら
        if (other.gameObject.CompareTag("GoldenSilk"))
        {
            if (_mSilkCount == 3)
                return;
            //TODO (3個まで追加)

            // 金の糸の画像を表示
            IsGotSilk = true;
            _mGotSilkImage.SetActive(true);
            _mGotSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
            TypeEventSystem.Instance.Send<PickSilkEvent>(pickSilkEvent);
            _mSilkCount++;
        }
        // ゴールに当たったら
        if (other.gameObject.CompareTag("Goal"))
        {
            // 自分が金の糸を持っていたら
            if (IsGotSilk)
            {
                AddScoreEvent AddScoreEvent = new AddScoreEvent()
                {
                    playerID = _mID,
                    silkCount = _mSilkCount
                };
                TypeEventSystem.Instance.Send<AddScoreEvent>(AddScoreEvent);
                IsGotSilk = false;
                _mGotSilkImage.SetActive(false);
                _mSilkCount = 0;
            }
        }
    }


    private void UpdateFine()
    {
        // エフェクトスタート
        if (_mParticleSystem.isStopped)
        {
            _mParticleSystem.Play();
        }
        // エフェクトの更新
        {
            _pSMain.startSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 2.0f;
            _pSMain.simulationSpeed = _currentMoveSpeed / Global.PLAYER_MAX_MOVE_SPEED * 4.0f + 1.0f;
            _pSMain.startLifetime = _pSMain.simulationSpeed * 0.5f;

        }

        // プレイヤー画像の向きを変える
        FlipCharacterImage();
        // 金の網のを持っていれば、プレイヤー画像の上に表示する
        if (IsGotSilk == true)
        {
            // キャラクター画像の縦の大きさを取得して画像の上で表示する
            _mGotSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
        }

        // プレイヤーインプットを取得する
        Vector2 rotateInput = rotateAction.ReadValue<Vector2>();
        // 回転方向を決める
        _rotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);

        // プレイヤーがいるところの地面の色をチェックする
        CheckGroundColor();

        // 領域は描画できるかどうかをチェックする
        CheckCanPaint();

        //TODO ブースト（隠れ仕様）
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


    /// <summary>
    /// プレイヤーのプロパティを初期化する
    /// </summary>
    private void Init()
    {
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
        boostAction = playerInput.actions["Boost"];
        boostAction.performed += OnBoost;
        _mStatus = PlayerStatus.Fine;
        _mColliderOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _particleObject = Instantiate(GameResourceSystem.Instance.GetPrefabResource("DustParticlePrefab"), transform);
        _particleObject.transform.localPosition = Vector3.zero;
        _particleObject.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        _mParticleSystem = _particleObject.GetComponent<ParticleSystem>();
        _pSMain = _mParticleSystem.main;
        _pSMain.startSize = 0.4f;
        _pSMain.startColor = Color.gray;
        _boostDurationTime = Global.BOOST_DURATION_TIME;

        _explosionPrefab = GameResourceSystem.Instance.GetPrefabResource("Explosion");
        _bigSpider = Instantiate(GameResourceSystem.Instance.GetPrefabResource("BigSpider"),Vector3.zero,Quaternion.identity);
        _bigSpider.transform.position = new Vector3(0.0f,0.0f,100.0f);
        _bigSpider.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        _mBigSpiderLineRenderer = _bigSpider.GetComponentInChildren<LineRenderer>();
        _mBigSpiderLineRenderer.positionCount = 2;
        _mBigSpiderLineRenderer.startWidth = 0.2f;
        _mBigSpiderLineRenderer.endWidth = 0.2f;

        _mGotSilkImage = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
        _mGotSilkImage.SetActive(false);

        // プレイヤー自分の画像のレンダラーを取得する
        _mImageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // 表示順位を変換する
        _mImageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
        // 復活するとき現れる影のプレハブをインスタンス化する
        _mShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("PlayerShadow"), Vector3.zero, Quaternion.identity);
        _mShadow.transform.localScale = Vector3.zero;
        //TODO 影の方向を変える
        _mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // 影を透明にする
        _mShadowSpriteRenderer = _mShadow.GetComponent<SpriteRenderer>();
        _mShadowSpriteRenderer.color = Color.clear;

        _mSilkCount = 0;
        _mID = -1;

        // DropPointControlコンポネントを追加する
        _mDropPointControl = gameObject.AddComponent<DropPointControl>();

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
    /// プレイヤーのリジッドボディを回転させる
    /// </summary>
    /// <param name="quaternion">回転する方向</param>
    private void RotateRigidbody(Quaternion quaternion)
    {
        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotationSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// キャラクターの画像を反転する関数
    /// </summary>
    private void FlipCharacterImage()
    {
        if (transform.forward.x < 0.0f)
        {
            _mImageSpriteRenderer.flipX = false;
        }
        else
        {
            _mImageSpriteRenderer.flipX = true;
        }

    }
    /// <summary>
    /// プレイヤーの死亡状態を設定する
    /// </summary>
    private void SetDeadStatus()
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
        // プレイヤーの向きをリセットする
        FlipCharacterImage();
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
        if(_mParticleSystem.isPlaying)
        {
            _mParticleSystem.Stop();
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



    /// <summary>
    /// 復活アニメーションをリセットする
    /// </summary>
    private void ResetRespawnAnimation()
    {
        _bigSpider.transform.position = _mRespawnPos + new Vector3(0.0f, 0.0f, 100.0f);
        _mBigSpiderLineRenderer.positionCount = 0;
        _mShadow.transform.localScale = Vector3.zero;
        _mShadowSpriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// 復活アニメーションを更新する関数
    /// </summary>
    //TODO カメラを二つにする時に変更する予定
    private void UpdateRespawnAnimation()
    {
        // 復活アニメーション前半部分の処理
        if (_respawnAnimationTimer.GetTime() >= Global.RESPAWN_TIME / 2.0f)
        {
            _bigSpider.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f * Time.deltaTime), Space.World);
            transform.position = _bigSpider.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }
        // 復活アニメーション後半部分の処理
        else
        {
            //TODO
            transform.Translate(-(_bigSpider.transform.position - _mRespawnPos) * 0.4f * Time.deltaTime, Space.World);
            transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
            _mShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
            _mShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
            Vector3[] temp = new Vector3[2];
            temp[0] = _bigSpider.transform.position;
            temp[1] = transform.position + new Vector3(0.0f, -0.5f, _mColliderOffset);
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
        _mStatus = PlayerStatus.Dead;
        transform.localScale = Vector3.one;
        _currentMoveSpeed = 0.0f;
        IsGotSilk = false;
        _isBoosting = false;
        _boostDurationTime = Global.BOOST_DURATION_TIME;
        _mBoostCoolDown = null;
        _mSilkCount = 0;
        transform.forward = Global.PLAYER_DEFAULT_FORWARD[(_mID - 1)];
        DropPointSystem.Instance.ClearDropPoints(_mID);
        maxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        _mDropPointControl.ResetTrail();

    }
    /// <summary>
    /// 地面の色をチェックする
    /// </summary>
    private void CheckGroundColor()
    {
        // 自分の領域にいたら
        if (colorCheck.isTargetColor(Color.clear))
        {
            _moveSpeedCoefficient = 1.0f;
        }
        // 別のプレイヤーの領域にいたら
        else if (colorCheck.isTargetColor(_mColor))
        {
            _moveSpeedCoefficient = Global.SPEED_UP_COEFFICIENT;
        }
        // 塗られていない地面にいたら
        else
        {
            _moveSpeedCoefficient = Global.SPEED_DOWN_COEFFICIENT;
        }

    }

    /// <summary>
    /// プレイヤー領域を描画してみる関数
    /// </summary>
    private void CheckCanPaint()
    {
        Vector3[] dropPoints = DropPointSystem.Instance.GetPlayerDropPoints(_mID);
        // DropPointは4個以上あれば描画できる
        if (dropPoints.Length >= 4)
        {
            // プレイヤーの先頭座標
            Vector3 endPoint1 = transform.position + transform.forward * _mColliderOffset;
            // プレイヤーが直前にインスタンス化したDropPoint
            Vector3 endPoint2 = dropPoints[dropPoints.Length - 1];

            // endPoint1とendPoint2で作ったベクトルとendPoint2以外のDropPointを先頭から順番で2個ずつで作ったベクトルが交わっているかどうかをチェックする
            for (int i = 0; i < dropPoints.Length - 2; ++i)
            {
                // 二つのベクトルが平行したらcontinue
                if (VectorMath.IsParallel(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1))
                {
                    continue;
                }

                // それぞれの座標点と自分自身以外のベクトルの位置関係を計算する(0より大きいならベクトルの左側、0より小さいならベクトルの右側、0ならベクトルの中)
                float pointPos1 = VectorMath.PointOfLine(dropPoints[i], endPoint2, endPoint1);
                float pointPos2 = VectorMath.PointOfLine(dropPoints[i + 1], endPoint2, endPoint1);
                float pointPos3 = VectorMath.PointOfLine(endPoint2, dropPoints[i], dropPoints[i + 1]);
                float pointPos4 = VectorMath.PointOfLine(endPoint1, dropPoints[i], dropPoints[i + 1]);

                // 二つのベクトルが交わっていたら描画する
                if (pointPos1 * pointPos2 < 0 && pointPos3 * pointPos4 < 0)
                {
                    // 交点を計算する
                    Vector3 crossPoint = VectorMath.GetCrossPoint(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1);
                    // 描画する領域の頂点を取得する
                    List<Vector3> verts = new List<Vector3>();
                    for (int j = i + 1; j < dropPoints.Length; j++)
                    {
                        verts.Add(dropPoints[j]);
                    }
                    verts.Add(crossPoint);
                    // 描画する
                    PolygonPaintManager.Instance.Paint(verts.ToArray(), _mID, _mColor);
                    // 全てのDropPointを消す
                    DropPointSystem.Instance.ClearDropPoints(_mID);
                    // 尻尾のTrailRendererの状態をリセットする
                    _mDropPointControl.ResetTrail();
                    break;
                }
            }
        }

    }


    
    public PlayerStatus GetStatus() => _mStatus;
    public int GetID() => _mID;
    public Color GetColor() => _mColor;

    //todo アクセス修飾子の変更予定
    public void SetStatus(PlayerStatus status)
    {
        _mStatus = status;
    }

    public void SetProperties(int ID, Color color)
    {
        if (_mID == -1)
        {
            _mID = ID;
            _mColor = color;
            if (_mID <= Global.PLAYER_START_POSITIONS.Length)
            {
                _mRespawnPos = Global.PLAYER_START_POSITIONS[(_mID - 1)];
                _bigSpider.transform.position += _mRespawnPos;
                _mShadow.transform.position = _mRespawnPos;
            }
            name = "Player" + _mID.ToString();
        }
    }


    private void OnEnable()
    {
        rotateAction.Enable();
        boostAction.Enable();
    }
    private void OnDisable()
    {
        rotateAction.Disable();
        boostAction.Disable();
    }
    private void OnDestroy()
    {
        boostAction.performed -= OnBoost;
    }

    // 隠れ仕様
    private void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_mStatus == PlayerStatus.Fine && _isBoosting == false)
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
