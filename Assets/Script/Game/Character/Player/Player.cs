using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Gaming.PowerUp;
using Math;

namespace Character
{
    [RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
    public class Player : Character, IPlayer2ItemSystem
    {
        private enum State
        {
            None = 0,
            Fine,
            Dead,
            Invincible,
            Uncontrollable,
            Stun,
        }

        public bool HadSilk => mSilkData.SilkCount > 0;

        public IItem item { get; set; }

        private struct PlayerSilkData
        {
            public int SilkCount;                   // プレイヤーが持っている金の糸の数
            public GameObject SilkRenderer;         // プレイヤーが持っている金の糸を画面に表示するGameObject
        }
        private ColorCheck mColorCheck;                     // カラーチェックコンポネント
        //TODO 二つを一つにする
        private InputAction mBoostAction;                   // プレイヤーのブースト入力
        private InputAction mRotateAction;                  // プレイヤーの回転入力
        private InputAction _itemAction;
        private PlayerInput mPlayerInput;                   // playerInputAsset

        [field:SerializeField]
        private State mState;                               // プレイヤーのステータス

        private float mColliderOffset;                      // プレイヤーコライダーの長さ（正方形）
        private float mCurrentMoveSpeed;                    // プレイヤーの現在速度
        private float mMoveSpeedCoefficient;                // プレイヤーの移動速度の係数
        private Rigidbody mRigidbody;                       // プレイヤーのRigidbody
        private Vector3 mRotateDirection;                   // プレイヤーの回転方向
        private bool _canBoost = false;                     // ブーストできるかのフラグ
        private SpriteRenderer mImageSpriteRenderer;        // プレイヤー画像のSpriteRenderer
        private PlayerAnim mAnim;
        private DropPointControl mDropPointControl;         // プレイヤーのDropPointControl
        private PlayerParticleSystemControl mParticleSystemControl;

        //TODO refactorying
        private int mID = -1;                               // プレイヤーID
        private Color mColor;                               // プレイヤーの領域の色                          
        private PlayerSilkData mSilkData;
        private float mBoostCoefficient;

        private float _returnToFineTimer = 0f;
        //TODO テスト用
        public Sprite[] silkCountSprites;

        private void Awake()
        {
            // 初期化処理
            Init();
        }
        private void Start()
        {
            mCurrentMoveSpeed = 0.0f;

            GetComponent<DropPointControl>()?.Init();
            transform.forward = Global.PLAYER_DEFAULT_FORWARD[mID - 1];
            mParticleSystemControl.Play();
        }
        private void Update()
        {
            if (mState == State.Dead)
                return;
            // プレイヤーが「通常」状態じゃないと後ほどの処理を実行しない
            else if(mState != State.Fine)
            {
                ReturnToFineCountDown();
                return;
            }
            UpdateFine();

        }

        private void FixedUpdate()
        {
            switch(mState)
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
                    mCurrentMoveSpeed = 0f;
                    break;
            }

        }

        private void LateUpdate()
        {
            switch(mState)
            {
                case State.Fine:
                    return;
                case State.Uncontrollable:
                    PlaySlipAnimation();
                    return;
            }
        }


        /// <summary>
        /// 衝突があったとき処理する
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            // 死亡したプレイヤーは金の網を持っていたら
            if (mSilkData.SilkCount > 0)
            {
                DropSilkEvent dropSilkEvent = new DropSilkEvent()
                {
                    pos = transform.position,
                };
                // 金の糸のドロップ場所を設定する
                TypeEventSystem.Instance.Send(dropSilkEvent);
            }
            // 衝突したら死亡状態に設定する
            SetDeadStatus();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Contains("DropPoint"))
            {
                // 自分のDropPoint以外のDropPointに当たったら
                if (other.gameObject.tag.Contains(mID.ToString()) == false)
                {
                    if (mSilkData.SilkCount > 0)
                    {
                        DropSilkEvent dropSilkEvent = new DropSilkEvent()
                        {
                            pos = transform.position,
                        };
                        TypeEventSystem.Instance.Send(dropSilkEvent);
                    }
                    SetDeadStatus();
                }
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
            Vector2 rotateInput = mRotateAction.ReadValue<Vector2>();
            // 回転方向を決める
            mRotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);
            // プレイヤーがいるところの地面の色をチェックする
            CheckGroundColor();
            // 領域を描画してみる
            TryPaintArea();

        }

        /// <summary>
        /// プレイヤーのプロパティを初期化する
        /// </summary>
        protected override void Init()
        {
            mRigidbody = GetComponent<Rigidbody>();
            mColorCheck = GetComponent<ColorCheck>();
            mPlayerInput = GetComponent<PlayerInput>();
            // プレイヤー自分の画像のレンダラーを取得する
            mImageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            mParticleSystemControl = gameObject.GetComponent<PlayerParticleSystemControl>();
            // DropPointControlコンポネントを追加する
            mDropPointControl = gameObject.AddComponent<DropPointControl>();
            // PlayerAnimコンポネントを追加する
            mAnim = gameObject.AddComponent<PlayerAnim>();


            mColorCheck.layerMask = LayerMask.GetMask("Ground");
            mMoveSpeedCoefficient = 1.0f;
            mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            mStatus.mRotationSpeed = Global.PLAYER_ROTATION_SPEED;
            mState = State.Fine;
            mColliderOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
            // 表示順位を変換する
            mImageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
            mSilkData.SilkCount = 0;
            mSilkData.SilkRenderer = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
            mSilkData.SilkRenderer.transform.parent = mImageSpriteRenderer.transform;
            mSilkData.SilkRenderer.SetActive(false);
            mBoostCoefficient = 1f;

        }

        /// <summary>
        /// プレイヤーの移動を制御する
        /// </summary>
        private void PlayerMovement(in float acceleration)
        {
            // 速度を計算し、範囲内に制限する
            mCurrentMoveSpeed =  mCurrentMoveSpeed + acceleration * Time.fixedDeltaTime;
            mCurrentMoveSpeed = Mathf.Clamp(mCurrentMoveSpeed, 0f, mStatus.mMaxMoveSpeed);

            // 前向きの移動をする
            Vector3 moveDirection = transform.forward * mCurrentMoveSpeed * mMoveSpeedCoefficient * mBoostCoefficient;
            mRigidbody.velocity = moveDirection;
        }

        /// <summary>
        /// プレイヤーの回転を制御する
        /// </summary>
        private void PlayerRotation()
        {
            // 方向入力がないと終了
            if (mRotateDirection == Vector3.zero)
                return;

            // 入力された方向へ回転する
            {
                Quaternion rotation = Quaternion.LookRotation(mRotateDirection, Vector3.up);
                mRigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, mStatus.mRotationSpeed * Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// キャラクターの画像を反転する関数
        /// </summary>
        private void FlipCharacterImage()
        {
            if (transform.forward.x < 0.0f)
            {
                mImageSpriteRenderer.flipX = false;
            }
            else
            {
                mImageSpriteRenderer.flipX = true;
            }
        }

        /// <summary>
        /// プレイヤーの死亡状態を設定する
        /// </summary>
        private void SetDeadStatus()
        {
            mAnim.StartExplosionAnim();
            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                dropCount = mSilkData.SilkCount,
                pos = transform.position
            };
            TypeEventSystem.Instance.Send(dropSilkEvent);
            PlayerRespawnEvent playerRespawnEvent = new PlayerRespawnEvent()
            {
                ID = mID
            };
            TypeEventSystem.Instance.Send(playerRespawnEvent);

            // プレイヤーの状態をリセットする
            ResetStatus();
            // プレイヤーの向きをリセットする
            FlipCharacterImage();
            // コンポネントを無効化にする
            GetComponent<DropPointControl>().enabled = false;
            GetComponentInChildren<TrailRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            // プレイヤー復活イベントを喚起する
            mAnim.StartRespawnAnim();
            mParticleSystemControl.Stop();
            SetPowerUpLevel();
        }

        /// <summary>
        /// プレイヤーのステイタスをリセットする
        /// </summary>
        private void ResetStatus()
        {
            mRigidbody.velocity = Vector3.zero;
            mRigidbody.angularVelocity = Vector3.zero;

            mState = State.Dead;

            transform.localScale = Vector3.one;

            mCurrentMoveSpeed = 0.0f;

            _canBoost = false;

            mSilkData.SilkCount = 0;

            transform.forward = Global.PLAYER_DEFAULT_FORWARD[(mID - 1)];

            DropPointSystem.Instance.ClearDropPoints(mID);

            mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;

            mDropPointControl.ResetTrail();

            mSilkData.SilkRenderer.SetActive(false);

            _returnToFineTimer = 0f;
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
            else if (mColorCheck.isTargetColor(mColor))
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
        private void TryPaintArea()
        {
            Vector3[] dropPoints = DropPointSystem.Instance.GetPlayerDropPoints(mID);
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
                        PolygonPaintManager.Instance.Paint(verts.ToArray(), mID, mColor);
                        TryCaptureObject(verts.ToArray());
                        // 全てのDropPointを消す
                        DropPointSystem.Instance.ClearDropPoints(mID);
                        // 尻尾のTrailRendererの状態をリセットする
                        mDropPointControl.ResetTrail();
                        break;
                    }
                }
            }
        }

        private void TryCaptureObject(Vector3[] verts)
        {
            #region Pick Silk
            Vector3[] silkPos = ItemManager.Instance.GetOnFieldSilkPos();
            List<Vector3> caputuredSilk = new List<Vector3>();
            bool isPickedNew = false;
            foreach (Vector3 pos in silkPos)
            {
                if (VectorMath.InPolygon(pos, verts))
                {
                    mSilkData.SilkCount++;
                    caputuredSilk.Add(pos);
                    isPickedNew = true;

                }
            }
            // 金の糸の画像を表示
            if (isPickedNew)
            {
                SilkCapturedEvent silkCapturedEvent = new SilkCapturedEvent()
                {
                    ID = mID,
                    positions = caputuredSilk.ToArray()
                };
                TypeEventSystem.Instance.Send(silkCapturedEvent);
                AudioManager.Instance.PlayFX("SpawnFX", 0.7f);
                // キャラクター画像の縦の大きさを取得して画像の上で表示する
                mSilkData.SilkRenderer.transform.localPosition = new Vector3(-mImageSpriteRenderer.bounds.size.x / 4f, mImageSpriteRenderer.bounds.size.z * 1.2f, 0);
                mSilkData.SilkRenderer.SetActive(true);
                Transform silkCount = mSilkData.SilkRenderer.transform.GetChild(0);
                if (silkCount != null)
                {
                    silkCount.GetComponent<SpriteRenderer>().sprite = silkCountSprites[mSilkData.SilkCount];
                    silkCount.transform.localPosition = new Vector3(mImageSpriteRenderer.bounds.size.x, 0, 0);
                }
                SetPowerUpLevel();
            }
            #endregion
            #region Pick Item
            Vector3[] itemBoxPos = ItemManager.Instance.GetOnFieldItemBoxPos();
            List<Vector3> capturedItemBoxPos = new List<Vector3>();
            isPickedNew = false;
            foreach(var pos in itemBoxPos)
            {
                if(VectorMath.InPolygon(pos,verts))
                {
                    capturedItemBoxPos.Add(pos);
                    isPickedNew = true;
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
                    ItemBoxsPos = capturedItemBoxPos.ToArray()
                });
            }
            #endregion
        }

        private void SetPlayerInputProperties()
        {
            mPlayerInput.defaultActionMap = name;
            mPlayerInput.neverAutoSwitchControlSchemes = true;
            mPlayerInput.SwitchCurrentActionMap(name);
            mRotateAction = mPlayerInput.actions["Rotate"];
            mBoostAction = mPlayerInput.actions["Boost"];
            mBoostAction.performed += OnBoost;
            _itemAction = mPlayerInput.actions["Item"];
            _itemAction.performed += OnUseItem;
        }

        private void SetPowerUpLevel()
        {
            if (mSilkData.SilkCount > 0)
            {
                mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED + Global.POWER_UP_PARAMETER[mSilkData.SilkCount - 1].SpeedUp;
                mStatus.mRotationSpeed = Global.PLAYER_ROTATION_SPEED + Global.POWER_UP_PARAMETER[mSilkData.SilkCount - 1].RotateUp;
            }
            else
            {
                mStatus.mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
                mStatus.mRotationSpeed = Global.PLAYER_ROTATION_SPEED;
            }
        }

        private void OnUseItem(InputAction.CallbackContext ctx)
        {
            if(mState == State.Fine && ctx.performed)
            {
                this.UseItem(this);
                
            }


        }

        private void ReturnToFineCountDown()
        {
            _returnToFineTimer -= Time.deltaTime;
            if(_returnToFineTimer <= 0f)
            {
                mImageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, transform.forward);
                mState = State.Fine;
            }
        }

        private float GetOnSlipDeceleration()
        {
            if(mCurrentMoveSpeed >= mStatus.mMaxMoveSpeed / 4f)
            {
                return -(mCurrentMoveSpeed - mStatus.mMaxMoveSpeed / 4f) / (_returnToFineTimer - Global.ON_SLIP_TIME / 2f);  
            }
            if(mCurrentMoveSpeed <= Global.ON_SLIP_MIN_SPEED)
            {
                return 0f;
            }
            else
            {
                return -mCurrentMoveSpeed / _returnToFineTimer;
            }
        }

        private void PlaySlipAnimation()
        {
            //float rotationAngle = (-720f / Global.ON_SLIP_TIME * _returnToFineTimer + 720f) * Time.deltaTime;
            float rotationAngle = 720f / Global.ON_SLIP_TIME * Time.deltaTime;
            mImageSpriteRenderer.transform.Rotate(Vector3.back * rotationAngle);
        }
        private void OnEnable()
        {
            mPlayerInput?.ActivateInput();
        }
        private void OnDisable()
        {
            mPlayerInput?.DeactivateInput();
        }
        private void OnDestroy()
        {
            mBoostAction.performed -= OnBoost;
            _itemAction.performed -= OnUseItem;
        }
        // ブースト
        private void OnBoost(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (mState == State.Fine && _canBoost == false)
                {
                    mBoostCoefficient = 1.5f;
                    mCurrentMoveSpeed = mStatus.mMaxMoveSpeed;
                    _canBoost = true;
                    TypeEventSystem.Instance.Send(new BoostStart 
                    { 
                        Number = mID
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
                            _canBoost = false;
                        }
                        );
                    stopBoostTimer.StartTimer(this);
                    boostCoolDownTimer.StartTimer(this);
                }

            }
        }

        #endregion
        #region interface
        public bool IsDead() => mState == State.Dead;
        public int GetID() => mID;
        public Color GetColor() => mColor;
        public void SetProperties(int ID, Color color)
        {
            if (mID == -1)
            {
                mID = ID;
                mColor = color;
                name = "Player" + mID.ToString();
            }
            if(mID != -1)
            {
                SetPlayerInputProperties();
            }
        }

        public void StartRespawn()
        {
            if(mState == State.Dead)
            {
                transform.position = Global.PLAYER_START_POSITIONS[mID - 1];
                transform.forward = Global.PLAYER_DEFAULT_FORWARD[mID - 1];
                mState = State.Fine;
                GetComponentInChildren<TrailRenderer>().enabled = true;
                GetComponent<DropPointControl>().enabled = true;
                GetComponent<Collider>().enabled = true;
                GameObject smoke = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Smoke"), transform.position, Quaternion.identity);
                smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
                mParticleSystemControl.Play();
            }
        }

        public void OnSlip()
        {
            mState = State.Uncontrollable;
            _returnToFineTimer = Global.ON_SLIP_TIME;
        }

        public void OnStun()
        {
            mState = State.Stun;
            mCurrentMoveSpeed = 0f;
            mRigidbody.velocity = Vector3.zero;
            _returnToFineTimer = Global.ON_STUN_TIME;
        }
        public float ColliderOffset => mColliderOffset;
        #endregion


    }

}
