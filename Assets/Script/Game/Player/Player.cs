using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public enum Status
    {
        Fine,
        Dead,
        Invincible
    }

    [SerializeField]
    private float mMaxMoveSpeed;                         // プレイヤーの最大速度
    [Min(0.0f)][SerializeField]
    private float mAcceleration;                         // プレイヤーの加速度
    [Min(0.0f)][SerializeField]
    private float mRotationSpeed;                        // プレイヤーの回転速度

    private ColorCheck mColorCheck;                      // カラーチェックコンポネント
    //TODO 二つを一つにする
    private InputAction mBoostAction;                    // プレイヤーのブースト入力
    private InputAction mRotateAction;                   // プレイヤーの回転入力
    private PlayerInput mPlayerInput;                    // playerInputAsset
    private Status mStatus;                            // プレイヤーのステータス
    private float mColliderOffset;                     // プレイヤーコライダーの長さ（正方形）
    private float mCurrentMoveSpeed;                    // プレイヤーの現在速度
    private float mMoveSpeedCoefficient;                // プレイヤーの移動速度の係数
    private Rigidbody mRigidbody;                       // プレイヤーのRigidbody
    private Vector3 mRotateDirection;                   // プレイヤーの回転方向

    private Timer _mBoostCoolDown;                      // ブーストタイマー（隠れ仕様）
    private float _boostDurationTime;                   // ブースト持続時間（隠れ仕様）
    private bool _isBoosting = false;                   // ブーストしているかのフラグ
    private SpriteRenderer _mImageSpriteRenderer;       // プレイヤー画像のSpriteRenderer
    private PlayerAnim _mAnim;
    private CharacterParticleSystem mParticleSystem;
    //TODO
    private GameObject _mHasSilkImage;                  // 金の糸を持っていることを示す画像

    //TODO refactorying
    private int _mID;                                   // プレイヤーID
    private Color _mColor;                              // プレイヤーの領域の色
    private DropPointControl _mDropPointControl;        // プレイヤーのDropPointControl

    public bool hasSilk { get; private set; }         // プレイヤーが金の糸を持っているかのフラグ
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

        // プレイヤーが「通常」状態じゃないと後ほどの処理を実行しない
        if (mStatus != Status.Fine)
        {
            return;
        }
        // 通常状態の処理
        UpdateFine();
    }

    private void FixedUpdate()
    {
        // プレイヤーが「通常」状態じゃないと動きに関する処理を実行しない
        if (mStatus != Status.Fine)
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
        if (hasSilk == true)
        {
            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                pos = transform.position,
                dropMode = DropMode.Standard
            };
            // 金の糸のドロップ場所を設定する

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
                if (hasSilk == true)
                {
                    DropSilkEvent dropSilkEvent = new DropSilkEvent()
                    {
                        pos = transform.position,
                        dropMode = DropMode.Standard
                    };
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
            hasSilk = true;
            _mHasSilkImage.SetActive(true);
            _mHasSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
            TypeEventSystem.Instance.Send<PickSilkEvent>();
            _mSilkCount++;
        }
        // ゴールに当たったら
        if (other.gameObject.CompareTag("Goal"))
        {
            // 自分が金の糸を持っていたら
            if (hasSilk == true)
            {
                AddScoreEvent addScoreEvent = new AddScoreEvent()
                {
                    playerID = _mID,
                    silkCount = _mSilkCount
                };

                TypeEventSystem.Instance.Send<AddScoreEvent>(addScoreEvent);
                hasSilk = false;
                _mHasSilkImage.SetActive(false);
                _mSilkCount = 0;
            }
        }
    }


    private void UpdateFine()
    {

        // プレイヤー画像の向きを変える
        FlipCharacterImage();
        // 金の網のを持っていれば、プレイヤー画像の上に表示する
        if (hasSilk == true)
        {
            // キャラクター画像の縦の大きさを取得して画像の上で表示する
            _mHasSilkImage.transform.position = transform.position + new Vector3(0, 0, _mImageSpriteRenderer.bounds.size.z);
        }

        // プレイヤーインプットを取得する
        Vector2 rotateInput = mRotateAction.ReadValue<Vector2>();
        // 回転方向を決める
        mRotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);

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
                mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            }
            if(_mBoostCoolDown.IsTimerFinished())
            {
                _mBoostCoolDown = null;
            }
        }

        mParticleSystem.Play();
    }


    /// <summary>
    /// プレイヤーのプロパティを初期化する
    /// </summary>
    private void Init()
    {
        mCurrentMoveSpeed = 0.0f;
        mRigidbody = GetComponent<Rigidbody>();
        mColorCheck = GetComponent<ColorCheck>();
        mColorCheck.layerMask = LayerMask.GetMask("Ground");
        mMoveSpeedCoefficient = 1.0f;
        mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        mAcceleration = Global.PLAYER_ACCELERATION;
        mRotationSpeed = Global.PLAYER_ROTATION_SPEED;

        mPlayerInput = GetComponent<PlayerInput>();
        mRotateAction = mPlayerInput.actions["Rotate"];
        mBoostAction = mPlayerInput.actions["Boost"];
        mBoostAction.performed += OnBoost;
        mStatus = Status.Fine;
        mColliderOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;

        _boostDurationTime = Global.BOOST_DURATION_TIME;

        _mHasSilkImage = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
        _mHasSilkImage.SetActive(false);

        // プレイヤー自分の画像のレンダラーを取得する
        _mImageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // 表示順位を変換する
        _mImageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
        _mSilkCount = 0;
        _mID = -1;

        // DropPointControlコンポネントを追加する
        _mDropPointControl = gameObject.AddComponent<DropPointControl>();
        // PlayerAnimコンポネントを追加する
        _mAnim = gameObject.AddComponent<PlayerAnim>();
        mParticleSystem = gameObject.AddComponent<CharacterParticleSystem>();

    }
    /// <summary>
    /// プレイヤーの移動を制御する
    /// </summary>
    private void PlayerMovement()
    {
        // 加速運動をして、最大速度まで加速する
        mCurrentMoveSpeed = mCurrentMoveSpeed >= mMaxMoveSpeed ? mMaxMoveSpeed : mCurrentMoveSpeed + mAcceleration;
        // 前向きの移動をする
        Vector3 moveDirection = transform.forward * mCurrentMoveSpeed * mMoveSpeedCoefficient;
        mRigidbody.velocity = moveDirection;

    }

    /// <summary>
    /// プレイヤーの回転を制御する
    /// </summary>
    private void PlayerRotation()
    {
        // 方向入力を取得する
        if (mRotateDirection != Vector3.zero)
        {
            // 入力された方向へ回転する
            Quaternion rotation = Quaternion.LookRotation(mRotateDirection, Vector3.up);
            RotateRigidbody(rotation);
        }
    }

    /// <summary>
    /// プレイヤーのリジッドボディを回転させる
    /// </summary>
    /// <param name="quaternion">回転する方向</param>
    private void RotateRigidbody(Quaternion quaternion)
    {
        mRigidbody.rotation = Quaternion.Slerp(transform.rotation, quaternion, mRotationSpeed * Time.fixedDeltaTime);
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
        _mAnim.Play(AnimType.Explode);
        // プレイヤーの状態をリセットする
        ResetStatus();
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

        _mHasSilkImage.SetActive(false);

        _mAnim.Play(AnimType.Respawn);
        mParticleSystem.Stop();

    }

    /// <summary>
    /// プレイヤーのステイタスをリセットする
    /// </summary>
    private void ResetStatus()
    {
        mRigidbody.velocity = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;
        mStatus = Status.Dead;
        transform.localScale = Vector3.one;
        mCurrentMoveSpeed = 0.0f;
        hasSilk = false;
        _isBoosting = false;
        _boostDurationTime = Global.BOOST_DURATION_TIME;
        _mBoostCoolDown = null;
        _mSilkCount = 0;
        transform.forward = Global.PLAYER_DEFAULT_FORWARD[(_mID - 1)];
        DropPointSystem.Instance.ClearDropPoints(_mID);
        mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
        _mDropPointControl.ResetTrail();

    }
    /// <summary>
    /// 地面の色をチェックする
    /// </summary>
    private void CheckGroundColor()
    {
        // 自分の領域にいたら
        if (mColorCheck.isTargetColor(Color.clear))
        {
            mMoveSpeedCoefficient = 1.0f;
        }
        // 別のプレイヤーの領域にいたら
        else if (mColorCheck.isTargetColor(_mColor))
        {
            mMoveSpeedCoefficient = Global.SPEED_UP_COEFFICIENT;
        }
        // 塗られていない地面にいたら
        else
        {
            mMoveSpeedCoefficient = Global.SPEED_DOWN_COEFFICIENT;
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
            Vector3 endPoint1 = transform.position + transform.forward * mColliderOffset;
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

    public int GetID() => _mID;
    public Color GetColor() => _mColor;
    public float GetCurrentMoveSpeed() => mCurrentMoveSpeed;

    //todo アクセス修飾子の変更予定
    public void SetStatus(Status status)
    {
        mStatus = status;
    }

    public void SetProperties(int ID, Color color)
    {
        if (_mID == -1)
        {
            _mID = ID;
            _mColor = color;
            if (_mID <= Global.PLAYER_START_POSITIONS.Length)
            {
                _mAnim.Init();
            }
            name = "Player" + _mID.ToString();
        }
    }


    private void OnEnable()
    {
        mRotateAction.Enable();
        mBoostAction.Enable();
    }
    private void OnDisable()
    {
        mRotateAction.Disable();
        mBoostAction.Disable();
    }
    private void OnDestroy()
    {
        mBoostAction.performed -= OnBoost;
    }

    // 隠れ仕様
    private void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(mStatus == Status.Fine && _isBoosting == false)
            {
                mMaxMoveSpeed *= 1.5f;
                mCurrentMoveSpeed = mMaxMoveSpeed;
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
