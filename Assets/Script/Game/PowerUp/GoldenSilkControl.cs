using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaming.PowerUp
{
    public interface IGoldenSilk
    {
        void StartDrop(Vector3 position);
        void StartDropSilkStandard();
        void StartDropSilkEdge();
    }

    public class GoldenSilkControl : MonoBehaviour, IGoldenSilk
    {
        private enum State
        {
            Active,         // 場にある
            Inactive,       // 生成されていない
            Droping,        // 落下中
            Captured        // プレイヤーに取られた
        }

        private enum DropMode
        {
            None,
            Standard,       // 普通のとき
            Edge            // 壁際にあるとき
        };

        private Timer mSpawnTimer;          // 金の糸を生成することを管理するタイマー
        private GameObject mSilkShadow;     // 金の糸の影
        //TODO 壁際にあるときに使う変数（未完成）
        private Vector3 _awayFromEdgeStartPos = Vector3.zero;
        private Vector3 _awayFromEdgeEndPos = Vector3.zero;
        [SerializeField]
        private State currentState = State.Inactive;
        private DropMode mode = DropMode.None;

        // Start is called before the first frame update
        void Start()
        {
            mSilkShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("SilkShadow"), transform.position, Quaternion.identity);
            ResetAnimationStatus();
        }

        // Update is called once per frame
        void Update()
        {
            switch (currentState)
            {
                // 生成されていないときは処理しない
                case State.Inactive:
                    break;
                // 落下状態ときの処理
                case State.Droping:
                    // 落下アニメーションを初期化する
                    if (mSpawnTimer == null)
                    {
                        InitSpawnAnimation();
                    }
                    // 落下アニメーションを更新する
                    UpdateSpawnAnimation();
                    // 落下アニメーションが終わったら解放する
                    if (mSpawnTimer.IsTimerFinished())
                    {
                        mSpawnTimer = null;
                    }
                    break;
            }
        }

        /// <summary>
        /// 落下アニメーションを更新する関数
        /// </summary>
        private void UpdateSpawnAnimation()
        {
            transform.Translate(0, 0, -300.0f / Global.SILK_SPAWN_TIME * Time.deltaTime);
            transform.localScale -= Vector3.one * Time.deltaTime * 2.0f / Global.SILK_SPAWN_TIME;
            mSilkShadow.transform.localScale += Vector3.one * Time.deltaTime * 2.0f / Global.SILK_SPAWN_TIME;
        }

        /// <summary>
        /// 落下アニメーションを初期化する関数
        /// </summary>
        private void InitSpawnAnimation()
        {
            transform.localScale = Vector3.one * 1.9f;
            AudioManager.Instance.PlayFX("FallFX", 0.7f);
            mSpawnTimer = new Timer();
            mSpawnTimer.SetTimer(Global.SILK_SPAWN_TIME / 2.0f,
                () =>
                {
                    ResetAnimationStatus();
                    GameObject smoke = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Smoke"), transform.position, Quaternion.identity);
                    smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                    smoke.transform.position -= new Vector3(0.0f, 0.2f, 0.0f);
                    currentState = State.Active;
                }
                );

        }

        /// <summary>
        /// 落下アニメーションの状態をリセットする関数
        /// </summary>
        private void ResetAnimationStatus()
        {
            mSilkShadow.transform.localScale = Vector3.zero;
            mSilkShadow.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            mSilkShadow.transform.position = Global.GAMEOBJECT_STACK_POS;
        }

        private void OnSetState(State state) => currentState = state;
        private void OnSetDropMode(DropMode dropMode) => mode = dropMode;

        public void StartDrop(Vector3 position)
        {
            SetPosition(position);
            OnSetState(State.Droping);
        }

        public void StartDropSilkStandard()
        {
            OnSetDropMode(DropMode.Standard);
        }
        
        public void StartDropSilkEdge()
        {
            OnSetDropMode(DropMode.Edge);
        }

        private void SetPosition(Vector3 position)
        {
            mSilkShadow.transform.position = position - new Vector3(0, 0.2f, 0);
            transform.position = mSilkShadow.transform.position + Vector3.forward * 150 + new Vector3(0, 0.2f, 0);
        }


    }

}
