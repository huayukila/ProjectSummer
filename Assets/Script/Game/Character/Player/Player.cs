using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Gaming.PowerUp;

namespace Character
{
    [RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
    public class Player : Character
    {
        public enum Status
        {
            None = 0,
            Fine,
            Dead,
            Invincible
        }
        private ColorCheck mColorCheck;                     // カラーチェックコンポネント
        //TODO 二つを一つにする
        private InputAction mBoostAction;                   // プレイヤーのブースト入力
        private InputAction mRotateAction;                  // プレイヤーの回転入力
        private PlayerInput mPlayerInput;                   // playerInputAsset
        private Status mStatus;                             // プレイヤーのステータス
        private float mColliderOffset;                      // プレイヤーコライダーの長さ（正方形）
        private float mCurrentMoveSpeed;                    // プレイヤーの現在速度
        private float mMoveSpeedCoefficient;                // プレイヤーの移動速度の係数
        private Rigidbody mRigidbody;                       // プレイヤーのRigidbody
        private Vector3 mRotateDirection;                   // プレイヤーの回転方向
        private Timer mBoostCoolDownTimer = null;           // ブーストタイマー（隠れ仕様）
        private float mBoostDurationTime;                   // ブースト持続時間（隠れ仕様）
        private bool mIsBoosting = false;                    // ブーストしているかのフラグ
        private SpriteRenderer mImageSpriteRenderer;        // プレイヤー画像のSpriteRenderer
        private PlayerAnim mAnim;
        private DropPointControl mDropPointControl;         // プレイヤーのDropPointControl
        private PlayerParticleSystemControl mParticleSystemControl;

        //TODO refactorying
        private int mID;                                    // プレイヤーID
        private Color mColor;                               // プレイヤーの領域の色
        private bool mHasSilk;                               // プレイヤーが金の糸を持っているかのフラグ
        private int mSilkCount;                             // プレイヤーが持っている金の糸の数
        private GameObject mHasSilkImage;                   // 金の糸を持っていることを示す画像

        private void Awake()
        {
            // 初期化処理
            Init();
        }
        private void Update()
        {
            // プレイヤー画像をずっと同じ方向に向くことにする
            mImageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

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
            if (mHasSilk == true)
            {
                DropSilkEvent dropSilkEvent = new DropSilkEvent()
                {
                    pos = transform.position,
                };
                // 金の糸のドロップ場所を設定する

                // 壁にぶつかったら
                if (collision.gameObject.CompareTag("Wall"))
                {

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
                // 自分のDropPoint以外のDropPointに当たったら
                if(other.gameObject.tag.Contains(mID.ToString()) == false)
                {
                    if (mHasSilk == true)
                    {
                        DropSilkEvent dropSilkEvent = new DropSilkEvent()
                        {
                            pos = transform.position,
                        };
                        TypeEventSystem.Instance.Send<DropSilkEvent>(dropSilkEvent);
                    }
                    SetDeadStatus();
                }
            }
            // 金の糸に当たったら
            else if (other.gameObject.CompareTag("GoldenSilk"))
            {
                if (mSilkCount == 3)
                    return;
                //TODO (3個まで追加)

                // 金の糸の画像を表示
                mHasSilk = true;
                mHasSilkImage.SetActive(true);
                mHasSilkImage.transform.position = transform.position + new Vector3(0, 0, mImageSpriteRenderer.bounds.size.z);
                TypeEventSystem.Instance.Send<PickSilkEvent>();
                mSilkCount++;
            }
            // ゴールに当たったら
            else if (other.gameObject.CompareTag("Goal"))
            {
                // 自分が金の糸を持っていたら
                if (mHasSilk == true)
                {
                    AddScoreEvent addScoreEvent = new AddScoreEvent()
                    {
                        playerID = mID,
                        silkCount = mSilkCount
                    };

                    TypeEventSystem.Instance.Send<AddScoreEvent>(addScoreEvent);
                    mHasSilk = false;
                    mHasSilkImage.SetActive(false);
                    mSilkCount = 0;
                }
            }
        }
        #region

        private void UpdateFine()
        {
            mParticleSystemControl.Play();
            // プレイヤー画像の向きを変える
            FlipCharacterImage();
            // 金の網のを持っていれば、プレイヤー画像の上に表示する
            if (mHasSilk == true)
            {
                // キャラクター画像の縦の大きさを取得して画像の上で表示する
                mHasSilkImage.transform.position = transform.position + new Vector3(0, 0, mImageSpriteRenderer.bounds.size.z);
            }
            // プレイヤーインプットを取得する
            Vector2 rotateInput = mRotateAction.ReadValue<Vector2>();
            // 回転方向を決める
            mRotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);
            // プレイヤーがいるところの地面の色をチェックする
            CheckGroundColor();
            // 領域を描画してみる
            TryPaintArea();
            //TODO ブースト（隠れ仕様）
            if(mBoostCoolDownTimer != null)
            {
                mBoostDurationTime -= Time.deltaTime;
                if(mBoostDurationTime <= 0.0f)
                {
                    mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
                }
                if(mBoostCoolDownTimer.IsTimerFinished())
                {
                    mBoostCoolDownTimer = null;
                }
            }
        }

        /// <summary>
        /// プレイヤーのプロパティを初期化する
        /// </summary>
        protected override void Init()
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
            mBoostDurationTime = Global.BOOST_DURATION_TIME;
            mHasSilkImage = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
            mHasSilkImage.SetActive(false);
            // プレイヤー自分の画像のレンダラーを取得する
            mImageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            // 表示順位を変換する
            mImageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
            mSilkCount = 0;
            mID = -1;
            // DropPointControlコンポネントを追加する
            mDropPointControl = gameObject.AddComponent<DropPointControl>();
            // PlayerAnimコンポネントを追加する
            mAnim = gameObject.AddComponent<PlayerAnim>();
            mParticleSystemControl = gameObject.GetComponent<PlayerParticleSystemControl>();
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
                mRigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, mRotationSpeed * Time.fixedDeltaTime);
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
            mAnim.SwitchAnimState(AnimType.Explode);
            mAnim.Play();
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
                ID = mID
            };
            TypeEventSystem.Instance.Send<PlayerRespawnEvent>(playerRespawnEvent);
            mHasSilkImage.SetActive(false);
            mAnim.SwitchAnimState(AnimType.Respawn);
            mAnim.Play();
            mParticleSystemControl.Stop();
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
            mHasSilk = false;
            mIsBoosting = false;
            mBoostDurationTime = Global.BOOST_DURATION_TIME;
            mBoostCoolDownTimer = null;
            mSilkCount = 0;
            transform.forward = Global.PLAYER_DEFAULT_FORWARD[(mID - 1)];
            DropPointSystem.Instance.ClearDropPoints(mID);
            mMaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            mDropPointControl.ResetTrail();
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
                        // 全てのDropPointを消す
                        DropPointSystem.Instance.ClearDropPoints(mID);
                        // 尻尾のTrailRendererの状態をリセットする
                        mDropPointControl.ResetTrail();
                        break;
                    }
                }
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
                if (mStatus == Status.Fine && mIsBoosting == false)
                {
                    mMaxMoveSpeed *= 1.5f;
                    mCurrentMoveSpeed = mMaxMoveSpeed;
                    mIsBoosting = true;
                    mBoostCoolDownTimer = new Timer();
                    mBoostCoolDownTimer.SetTimer(Global.BOOST_COOLDOWN_TIME,
                        () =>
                        {
                            mBoostDurationTime = Global.BOOST_DURATION_TIME;
                            mIsBoosting = false;
                        });
                }
            }
        }

        #endregion
        public int GetID() => mID;
        public Color GetColor() => mColor;
        public float GetCurrentMoveSpeed() => mCurrentMoveSpeed;
        public void SetProperties(int ID, Color color)
        {
            if (mID == -1)
            {
                mID = ID;
                mColor = color;
                if (mID <= Global.PLAYER_START_POSITIONS.Length)
                {
                    mAnim.Init();
                }
                name = "Player" + mID.ToString();
            }
        }

        public void StartRespawn()
        {
            if(mStatus == Status.Dead)
            {
                transform.position = Global.PLAYER_START_POSITIONS[mID - 1];
                transform.forward = Global.PLAYER_DEFAULT_FORWARD[mID - 1];
                mStatus = Status.Fine;
                GetComponentInChildren<TrailRenderer>().enabled = true;
                GetComponent<DropPointControl>().enabled = true;
                GetComponent<Collider>().enabled = true;
                GameObject smoke = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Smoke"), transform.position, Quaternion.identity);
                smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
            }
        }
    }

}
