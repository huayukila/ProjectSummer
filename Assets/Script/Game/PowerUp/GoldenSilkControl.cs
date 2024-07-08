using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

namespace Gaming.PowerUp
{
    public interface IGoldenSilk
    {
        void StartSpawn(Vector3 position);
        void StartDrop(Vector3 startPos,Vector3 endPos);
        
        void SetActiveCallBack(Action<GameObject> callback); 
    }

    public class GoldenSilkControl : NetworkBehaviour, IGoldenSilk
    {
        private enum State
        {       
            Inactive = 0,   // 生成されていない
            Active,         // 場にある
            Spawning,       // 落下中
            Drop,
        }

        private Action<GameObject> mActiveCallBack = null;
        private GameObject mSilkShadow;     // 金の糸の影
        //TODO 壁際にあるときに使う変数（未完成）
        private Vector3 mDropStartPos = Vector3.zero;
        private Vector3 mDropEndPos = Vector3.zero;
        [SerializeField]
        private State mCurrentState = State.Inactive;

        // Start is called before the first frame update
        void Awake()
        {
            mSilkShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("SilkShadow"), transform.position, Quaternion.identity);
            ResetAnimationStatus();
        }

        // Update is called once per frame
        void Update()
        {
            switch (mCurrentState)
            {
                // 生成されていないときは処理しない
                case State.Inactive:
                    break;
                // 落下状態ときの処理
                case State.Spawning:
                    // 落下アニメーションを更新する
                    UpdateSpawnAnimation();
                    break;
                case State.Drop:
                    UpdateDropAnimation();
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
            //AudioManager.Instance.PlayFX("FallFX", 0.7f);
            Timer spawnTimer = new Timer(Time.time,Global.SILK_SPAWN_TIME / 2.0f,
                () =>
                {
                    ResetAnimationStatus();
                    GameObject smoke = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Smoke"), transform.position, Quaternion.identity);
                    smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                    smoke.transform.position -= new Vector3(0.0f, 0.2f, 0.0f);
                    mCurrentState = State.Active;
                    mActiveCallBack?.Invoke(gameObject);
                    TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
                });
            spawnTimer.StartTimer(this);

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

        private void UpdateDropAnimation()
        {
            transform.position = Vector3.Lerp(mDropStartPos, mDropEndPos, Time.deltaTime * 2);
            mDropStartPos = transform.position;
            if((mDropStartPos - mDropEndPos).magnitude < 0.1f)
            {
                transform.position = mDropEndPos;
                mDropStartPos = Vector3.zero;
                mDropEndPos = Vector3.zero;
                OnSetState(State.Active);
                TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
            }
        }

        private void SetAnimationStartPosition(Vector3 position)
        {
            mSilkShadow.transform.position = position - new Vector3(0, 0.2f, 0);
            transform.position = mSilkShadow.transform.position + Vector3.forward * 150 + new Vector3(0, 0.2f, 0);
        }

        private void OnSetState(State state) => mCurrentState = state;

        public void StartSpawn(Vector3 position)
        {
            SetAnimationStartPosition(position);
            OnSetState(State.Spawning);

            // 落下アニメーションを初期化する
            InitSpawnAnimation();
        }

        public void SetInactive()
        {
            mSilkShadow.transform.position = Global.GAMEOBJECT_STACK_POS;
            transform.position = Global.GAMEOBJECT_STACK_POS;
            OnSetState(State.Inactive);
        }

        public void StartDrop(Vector3 startPos,Vector3 endPos)
        {
            mDropStartPos = startPos;
            mDropEndPos = endPos;
            OnSetState(State.Drop);
        }

        public void SetActiveCallBack(Action<GameObject> callback)
        {
            mActiveCallBack = callback;
        }
    }

}
