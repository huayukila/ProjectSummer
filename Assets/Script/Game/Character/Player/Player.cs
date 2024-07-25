using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Gaming.PowerUp;
using Math;
using Mirror;
using Character;

public struct PaintAreaEvent
{
    public Vector3[] Verts;
    public int PlayerID;
    public Color32 PlayerAreaColor;
}

public struct PlayerItemContainer
{
    public IPlayer2ItemSystem Player2ItemSystem;
    public Player Player;
}

namespace Character
{
    [RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
    public class Player : Character, IPlayer2ItemSystem, IItemAffectable, IPlayerCommand, IPlayerInfo, IPlayerState, IPlayerInterfaceContainer,IPlayerBoost
    {
        // プレイヤーの状態
        private enum State
        {
            None = 0,
            Fine,
            Dead,
            Invincible,
            Uncontrollable,
            Stun,
        }
        // プレイヤーの情報
        private struct PlayerInfo
        {
            public int ID;          //プレイヤーのID
            public Color AreaColor; //プレイヤーの領域の色
        }
        private struct PlayerSilkData
        {
            public int SilkCount;                   // プレイヤーが持っている金の糸の数
            public GameObject SilkRenderer;         // プレイヤーが持っている金の糸を画面に表示するGameObject
        }
        public bool HadSilk => _silkData.SilkCount > 0;
        public IItem item { get; set; }
        private ColorCheck _colorCheck;                     // カラーチェックコンポネント
        //TODO 二つを一つにする
        private InputAction _boostAction;                   // プレイヤーのブースト入力
        private InputAction _rotateAction;                  // プレイヤーの回転入力
        private InputAction _itemAction;
        private PlayerInput _input;                         // playerInputAsset

        [field:SerializeField]
        private State _playerState;                         // プレイヤーのステータス

        private float _itemPlaceOffset;                     // アイテムの放置場所とプレイヤー座標の間の距離（プレイヤーコライダーの辺の長さ（正方形））
        private float _currentMoveSpeed;                    // プレイヤーの現在速度
        private float _moveSpeedCoefficient;                // プレイヤーの移動速度の係数
        private Rigidbody _rigidbody;                       // プレイヤーのRigidbody
        private Vector3 _rotateDirection;                   // プレイヤーの回転方向
        private bool _isBoostCooldown = false;              // ブーストクールダウンしているかのフラグ
        private SpriteRenderer _imageSpriteRenderer;        // プレイヤー画像のSpriteRenderer
        private PlayerAnim _playerAnim;
        private DropPointControl _dropPointCtrl;            // プレイヤーのDropPointControl
        private PlayerParticleSystemControl _particleSystemCtrl; // プレイヤー自身にくっつけているパーティクルシステムを管理するコントローラー
        //TODO refactoring           
        private PlayerSilkData _silkData;
        private float mBoostCoefficient;
        [SerializeField]
        private PlayerInfo _playerInfo = default;           // プレイヤーの情報
        private float _returnToFineTimer = 0f;
        //TODO テスト用
        public Sprite[] silkCountSprites;
        private Dictionary<EItemEffect, Action> _itemAffectActions;
        private PlayerInterfaceContainer _playerInterface;

        private Vector3 _spawnPos;

        //TODO need refactorying(player should not know the existence of system)
        private IItemSystem _itemSystem;

        private GamePlayer _gamePlayer;

        private float _boostChargeTimeCnt;

        public Vector3 SpawnPos
        {
            get
            {
                return _spawnPos;
            }
            
            set
            {
                _spawnPos = value;
            }
        }
        private void Awake()
        {
            #region fuck mirror
            TypeEventSystem.Instance.Register<SendInitializedPlayerEvent>
            (
                eve =>
                {
                    IPlayerInfo info = eve.Player.GetComponent<IPlayerInfo>();
                    if(info != null)
                    {
                        eve.Player.GetComponent<DropPointControl>().Init();
                        eve.Player.transform.forward = Global.PLAYER_DEFAULT_FORWARD[info.ID - 1];
                        eve.Player.SpawnPos = (NetWorkRoomManagerExt.singleton as IRoomManager).GetRespawnPosition(info.ID - 1).position;
                    }
                    
                }
            );
            #endregion

            _playerInterface = new PlayerInterfaceContainer(this);
            // 初期化処理
            Init();
            // Item affectable actions init
            InitItemAffect();

            {
                _boostChargeTimeCnt = Global.BOOST_COOLDOWN_TIME;
            }
        }
        private void Start()
        {
            _currentMoveSpeed = 0.0f;


            _particleSystemCtrl.Play();

            {
                TypeEventSystem.Instance.Register<PaintAreaEvent>
                (
                    e =>
                    {
                        IPaintSystem paintSystem = (NetWorkRoomManagerExt.singleton as NetWorkRoomManagerExt).GetFramework().GetSystem<IPaintSystem>();
                        if(paintSystem != null)
                        {
                            paintSystem.Paint(e.Verts,e.PlayerID,e.PlayerAreaColor);
                        }
                    }
                ).UnregisterWhenGameObjectDestroyed(gameObject);

            }

            {
                _itemSystem = (NetWorkRoomManagerExt.singleton as NetWorkRoomManagerExt).GetFramework().GetSystem<IItemSystem>();
            }
        }
        private void Update()
        {
            if (_playerState == State.Dead)
                return;

            // プレイヤーが「通常」状態じゃないと後ほどの処理を実行しない
            else if(_playerState != State.Fine)
            {
                ReturnToFineCountDown();
                return;
            }
            UpdateFine();

        }

        private void FixedUpdate()
        {
            switch(_playerState)
            {
                case State.Fine:
                    // プレイヤーの動き
                    PlayerMovement(Global.PLAYER_ACCELERATION);
                    // プレイヤーの回転
                    PlayerRotation();
                    break;
                case State.Uncontrollable:
                    float deceleration = GetOnSlipDeceleration();
                    PlayerMovement(deceleration);
                    break;
                case State.Stun:
                    _currentMoveSpeed = 0f;
                    break;
            }

        }

        private void LateUpdate()
        {
            switch(_playerState)
            {
                case State.Fine:
                    return;
                case State.Uncontrollable:
                    PlaySlipAnimation();
                    return;
            }
            
        }

        #region InternalLogic

        private void UpdateFine()
        {
            // プレイヤー画像をずっと同じ方向に向くことにする
            //mImageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

            // プレイヤー画像の向きを変える
            FlipCharacterImage();
            // プレイヤーインプットを取得する
            Vector2 rotateInput = _rotateAction.ReadValue<Vector2>();
            // 回転方向を決める
            _rotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);
            // プレイヤーがいるところの地面の色をチェックする
            CheckGroundColor();
            // 領域を描画してみる
            TryPaintArea();

            if(_isBoostCooldown)
            {
                _boostChargeTimeCnt += Time.deltaTime;
            }

        }

        /// <summary>
        /// プレイヤーのプロパティを初期化する
        /// </summary>
        private void Init()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _colorCheck = GetComponent<ColorCheck>();
            // プレイヤー自分の画像のレンダラーを取得する
            _imageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _particleSystemCtrl = gameObject.GetComponent<PlayerParticleSystemControl>();
            // DropPointControlコンポネントを追加する
            _dropPointCtrl = gameObject.GetComponent<DropPointControl>();
            // PlayerAnimコンポネントを追加する
            _playerAnim = gameObject.GetComponent<PlayerAnim>();


            _colorCheck.layerMask = LayerMask.GetMask("Ground");
            _moveSpeedCoefficient = 1.0f;
            _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            _status.RotationSpeed = Global.PLAYER_ROTATION_SPEED;
            _playerState = State.Fine;
            _itemPlaceOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
            // 表示順位を変換する
            _imageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
            _silkData.SilkCount = 0;
            _silkData.SilkRenderer = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
            _silkData.SilkRenderer.transform.parent = _imageSpriteRenderer.transform;
            _silkData.SilkRenderer.SetActive(false);
            mBoostCoefficient = 1f;

            SetPlayerInputProperties();

            _gamePlayer = GetComponent<GamePlayer>();


        }

        private void InitItemAffect()
        {
            _itemAffectActions = new Dictionary<EItemEffect, Action>
            {
                // Item Effect     Action
                { EItemEffect.Stun, OnStun },
                { EItemEffect.Slip, OnSlip }
            };
        }

        #region Player Move
        /// <summary>
        /// プレイヤーの移動を制御する
        /// </summary>
        private void PlayerMovement(in float acceleration)
        {
            // 速度を計算し、範囲内に制限する
            _currentMoveSpeed =  _currentMoveSpeed + acceleration * Time.fixedDeltaTime;
            _currentMoveSpeed = Mathf.Clamp(_currentMoveSpeed, 0f, _status.MaxMoveSpeed);

            // 前向きの移動をする
            Vector3 moveDirection = transform.forward * _currentMoveSpeed * _moveSpeedCoefficient * mBoostCoefficient;
            _rigidbody.velocity = moveDirection;
        }
        #endregion


        #region Player Rotate
        /// <summary>
        /// プレイヤーの回転を制御する
        /// </summary>
        private void PlayerRotation()
        {
            // 方向入力がないと終了
            if (_rotateDirection == Vector3.zero)
                return;

            // 入力された方向へ回転する
            {
                Quaternion rotation = Quaternion.LookRotation(_rotateDirection, Vector3.up);
                _rigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, _status.RotationSpeed * Time.fixedDeltaTime);
            }
        }
        #endregion

        /// <summary>
        /// キャラクターの画像を反転する関数
        /// </summary>
        private void FlipCharacterImage()
        {
            if (transform.forward.x < 0.0f)
            {
                _imageSpriteRenderer.flipX = false;
            }
            else
            {
                _imageSpriteRenderer.flipX = true;
            }
        }

        /// <summary>
        /// プレイヤーの死亡状態を設定する
        /// </summary>
        private void StartDead()
        {
            _playerAnim.StartExplosionAnim();

            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                dropCount = _silkData.SilkCount,
                pos = transform.position
            };
            TypeEventSystem.Instance.Send(dropSilkEvent);
            TypeEventSystem.Instance.Send(new PlayerRespawnEvent(){ Player = gameObject});

            // プレイヤーの状態をリセットする
            ResetStatus();
            // プレイヤーの向きをリセットする
            FlipCharacterImage();
            // コンポネントを無効化にする
            GetComponent<DropPointControl>().enabled = false;
            GetComponentInChildren<TrailRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            // プレイヤー復活イベントを喚起する
            _particleSystemCtrl.Stop();
            SetPowerUpLevel();
        }

        /// <summary>
        /// プレイヤーのステイタスをリセットする
        /// </summary>
        private void ResetStatus()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _playerState = State.Dead;

            transform.localScale = Vector3.one;

            _currentMoveSpeed = 0.0f;

            _isBoostCooldown = false;

            _silkData.SilkCount = 0;

            transform.forward = Global.PLAYER_DEFAULT_FORWARD[(_playerInfo.ID - 1)];

            _gamePlayer.CmdOnClearAllDropPoints();

            _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;

            _dropPointCtrl.ResetTrail();

            _silkData.SilkRenderer.SetActive(false);

            _returnToFineTimer = 0f;
        }
        /// <summary>
        /// 地面の色をチェックする
        /// </summary>
        private void CheckGroundColor()
        {
            // 自分の領域にいたら
            if (_colorCheck.isTargetColor(Color.clear))
            {
                _moveSpeedCoefficient = 1.0f;
            }
            // 別のプレイヤーの領域にいたら
            else if (_colorCheck.isTargetColor(_playerInfo.AreaColor))
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
        private void TryPaintArea()
        {
            Vector3[] dropPoints = _dropPointCtrl.GetPlayerDropPoints();
            // DropPointは4個以上あれば描画できる
            if (dropPoints.Length >= 4)
            {
                // プレイヤーの先頭座標
                Vector3 endPoint1 = transform.position + transform.forward * _itemPlaceOffset;
                // プレイヤーが直前にインスタンス化したDropPointの座標
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

                        #region Paint Area
                        PaintAreaEvent paintEvent = new PaintAreaEvent
                        {
                            Verts = verts.ToArray(),
                            PlayerID = _playerInfo.ID,
                            PlayerAreaColor = _playerInfo.AreaColor
                        };
                        TypeEventSystem.Instance.Send(paintEvent);
                        #endregion

                        TryCaptureObject(verts.ToArray());
                        // 全てのDropPointを消す
                        _gamePlayer.CmdOnClearAllDropPoints();
                        
                        // 尻尾のTrailRendererの状態をリセットする
                        _dropPointCtrl.ResetTrail();
                        break;
                    }
                }
            }
        }

        private void TryCaptureObject(Vector3[] verts)
        {
            #region Pick Silk
            bool isPickedNew = false;
            List<Vector3> caputuredSilk = null;
            if(_itemSystem != null)
            {
                Vector3[] silkPos = _itemSystem.GetOnFieldSilkPos();
                caputuredSilk = new List<Vector3>();
                
                foreach (Vector3 pos in silkPos)
                {
                    if (VectorMath.InPolygon(pos, verts))
                    {
                        _silkData.SilkCount++;
                        caputuredSilk.Add(pos);
                        isPickedNew = true;
                    }
                }
            }
            
            // 金の糸の画像を表示
            if (isPickedNew)
            {
                SilkCapturedEvent silkCapturedEvent = new SilkCapturedEvent()
                {
                    ID = _playerInfo.ID,
                    positions = caputuredSilk?.ToArray()
                };
                TypeEventSystem.Instance.Send(silkCapturedEvent);
                //AudioManager.Instance.PlayFX("SpawnFX", 0.7f);
                // キャラクター画像の縦の大きさを取得して画像の上で表示する
                _silkData.SilkRenderer.transform.localPosition = new Vector3(-_imageSpriteRenderer.bounds.size.x / 4f, _imageSpriteRenderer.bounds.size.z * 1.2f, 0);
                _silkData.SilkRenderer.SetActive(true);
                Transform silkCount = _silkData.SilkRenderer.transform.GetChild(0);
                if (silkCount != null)
                {
                    silkCount.GetComponent<SpriteRenderer>().sprite = silkCountSprites[_silkData.SilkCount];
                    silkCount.transform.localPosition = new Vector3(_imageSpriteRenderer.bounds.size.x, 0, 0);
                }
                SetPowerUpLevel();
            }
            #endregion
            #region Pick Item
            List<Vector3> capturedItemBoxPos = null;
            if(_itemSystem != null)
            {
                Vector3[] itemBoxPos = _itemSystem.GetOnFieldItemBoxPos();
                capturedItemBoxPos = new List<Vector3>();
                isPickedNew = false;
                foreach(var pos in itemBoxPos)
                {
                    if(VectorMath.InPolygon(pos,verts))
                    {
                        capturedItemBoxPos.Add(pos);
                        isPickedNew = true;
                    }
                }
            }
            
            if(isPickedNew)
            {
                TypeEventSystem.Instance.Send(new PlayerGetItem
                {
                    player = this,
                });
                TypeEventSystem.Instance.Send(new GetNewItem
                {
                    ItemBoxsPos = capturedItemBoxPos?.ToArray()
                });
            }
            #endregion
        }

        private void SetPlayerInputProperties()
        {
            _input = GetComponent<PlayerInput>();
            _input.defaultActionMap = "Player";
            _input.neverAutoSwitchControlSchemes = true;
            _input.SwitchCurrentActionMap("Player");
            _rotateAction = _input.actions["Rotate"];
            _boostAction = _input.actions["Boost"];
            _boostAction.performed += OnBoost;
            _itemAction = _input.actions["Item"];
            _itemAction.performed += OnUseItem;
        }

        private void SetPowerUpLevel()
        {
            if (_silkData.SilkCount > 0)
            {
                _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED + Global.POWER_UP_PARAMETER[_silkData.SilkCount - 1].SpeedUp;
                _status.RotationSpeed = Global.PLAYER_ROTATION_SPEED + Global.POWER_UP_PARAMETER[_silkData.SilkCount - 1].RotateUp;
            }
            else
            {
                _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
                _status.RotationSpeed = Global.PLAYER_ROTATION_SPEED;
            }
        }

        private void OnUseItem(InputAction.CallbackContext ctx)
        {
            if(_playerState == State.Fine && ctx.performed)
            {
                if(_gamePlayer != null)
                {
                    _gamePlayer.CmdOnItemSpawn(gameObject);
                    //this.UseItem(this);
                }
            }
        }

        private void ReturnToFineCountDown()
        {
            _returnToFineTimer -= Time.deltaTime;
            if(_returnToFineTimer <= 0f)
            {
                _imageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, transform.forward);
                _playerState = State.Fine;
            }
        }

        private float GetOnSlipDeceleration()
        {
            if(_currentMoveSpeed >= _status.MaxMoveSpeed / 4f)
            {
                return -(_currentMoveSpeed - _status.MaxMoveSpeed / 4f) / (_returnToFineTimer - Global.ON_SLIP_TIME / 2f);  
            }
            if(_currentMoveSpeed <= Global.ON_SLIP_MIN_SPEED)
            {
                return 0f;
            }
            else
            {
                return -_currentMoveSpeed / _returnToFineTimer;
            }
        }

        private void PlaySlipAnimation()
        {
            //float rotationAngle = (-720f / Global.ON_SLIP_TIME * _returnToFineTimer + 720f) * Time.deltaTime;
            float rotationAngle = 720f / Global.ON_SLIP_TIME * Time.deltaTime;
            _imageSpriteRenderer.transform.Rotate(Vector3.back * rotationAngle);
        }
        private void OnEnable()
        {
            _input?.ActivateInput();
        }
        private void OnDisable()
        {
            _input?.DeactivateInput();
        }
        private void OnDestroy()
        {
            _boostAction.performed -= OnBoost;
            _itemAction.performed -= OnUseItem;
        }

        #region Boost Method
        // ブースト
        private void OnBoost(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (_playerState == State.Fine && _isBoostCooldown == false)
                {
                    _boostChargeTimeCnt = 0f;
                    mBoostCoefficient = 1.5f;
                    _currentMoveSpeed = _status.MaxMoveSpeed;
                    _isBoostCooldown = true;
                    TypeEventSystem.Instance.Send(new BoostStart 
                    { 
                        Number = _playerInfo.ID
                    });
                    Timer stopBoostTimer = new Timer(Time.time, Global.BOOST_DURATION_TIME,
                        () =>
                        {
                            mBoostCoefficient = 1.0f;
                        }
                        );
                    Timer boostCoolDownTimer = new Timer(Time.time,Global.BOOST_COOLDOWN_TIME,
                        () =>
                        {
                            _isBoostCooldown = false;
                            _boostChargeTimeCnt = Global.BOOST_COOLDOWN_TIME;
                        }
                        );
                    stopBoostTimer.StartTimer(this);
                    boostCoolDownTimer.StartTimer(this);
                }

            }
        }
        #endregion

        #endregion // InternalLogic

        #region Item Effect

        private void OnSlip()
        {
            _playerState = State.Uncontrollable;
            _returnToFineTimer = Global.ON_SLIP_TIME;
        }

        private void OnStun()
        {
            _playerState = State.Stun;
            _currentMoveSpeed = 0f;
            _rigidbody.velocity = Vector3.zero;
            _returnToFineTimer = Global.ON_STUN_TIME;
        }

        #endregion //Item Effect

        #region Interface
        public int ID => _playerInfo.ID;
        public int SilkCount => _silkData.SilkCount;
        public Color AreaColor => _playerInfo.AreaColor;
        public void SetInfo(int ID, Color color)
        {
            if (_playerInfo.ID != 0)
                return;
            
            {
                _playerInfo.ID = ID;
                _playerInfo.AreaColor = color;
                name = "Player" + _playerInfo.ID.ToString();
            }
        }
        public bool IsDead => _playerState == State.Dead;
        public bool IsFine => _playerState == State.Fine;
        public bool IsInvincible => _playerState == State.Invincible;
        public bool IsUncontrollable => _playerState == State.Uncontrollable;
        public bool IsStuning => _playerState == State.Stun;

        public PlayerInterfaceContainer GetContainer()
        {
            return _playerInterface;
        }

        public void CallPlayerCommand(EPlayerCommand cmd)
        {
            Debug.Log("Command:" + cmd.ToString() + " get called");
            switch(cmd)
            {
                case EPlayerCommand.Respawn:
                    StartRespawn();
                    break;
                case EPlayerCommand.Dead:
                    StartDead();
                    break;
                default:
                    break;
            }
        }

        private void StartRespawn()
        {
            // 死亡以外は再生処理しない
            if (_playerState != State.Dead)
                return;

            // 再生処理
            {
                transform.position = _spawnPos;
                transform.forward = Global.PLAYER_DEFAULT_FORWARD[_playerInfo.ID - 1];
                _playerState = State.Fine;

                GetComponentInChildren<TrailRenderer>().enabled = true;
                GetComponent<DropPointControl>().enabled = true;
                GetComponent<Collider>().enabled = true;

                // 落下後の煙エフェクトの作成
                GameObject smoke = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Smoke"), transform.position, Quaternion.identity);
                smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);

                // パーティクルシステムの再開
                _particleSystemCtrl.Play();
            }

        }

        public void OnEffect(string effectName)
        {
            effectName.ToTitleCast();

            object receivedEffect;

            if(Enum.TryParse(typeof(EItemEffect), effectName,out receivedEffect))
            {
                _itemAffectActions[(EItemEffect)receivedEffect].Invoke();
            }
            else
            {
                Debug.LogWarning("Can't find action of " + effectName + " effect." + "(In class " + GetType().Name + " )");
            }
        }
        public float ItemPlaceOffset => _itemPlaceOffset;

        //
        public float ChargeBarPercentage => Mathf.Clamp(_boostChargeTimeCnt / Global.BOOST_COOLDOWN_TIME, 0f, 1f);
        #endregion // Interface

        #region Obsolete Code
            /*
            /// <summary>
            /// 衝突があったとき処理する
            /// </summary>
            /// <param name="collision"></param>
            private void OnCollisionEnter(Collision collision)
            {
                // 死亡したプレイヤーは金の網を持っていたら
                if (_silkData.SilkCount > 0)
                {
                    DropSilkEvent dropSilkEvent = new DropSilkEvent()
                    {
                        pos = transform.position,
                    };
                    // 金の糸のドロップ場所を設定する
                    //HACK EventSystem temporary invalid
                    //TypeEventSystem.Instance.Send(dropSilkEvent);
                }
                // 衝突したら死亡状態に設定する
                StartDead();
            }

            private void OnTriggerEnter(Collider other)
            {

                if (other.gameObject.tag.Contains("DropPoint"))
                {
                    // 自分のDropPoint以外のDropPointに当たったら
                    if (other.gameObject.tag.Contains(_playerInfo.ID.ToString()) == false)
                    {
                        if (_silkData.SilkCount > 0)
                        {
                            DropSilkEvent dropSilkEvent = new DropSilkEvent()
                            {
                                pos = transform.position,
                            };
                            TypeEventSystem.Instance.Send(dropSilkEvent);
                        }
                        // 死亡状態に設定する
                        StartDead();
                    }
                }
            }

                    */
        #endregion //Obsolete Code

    }

}
