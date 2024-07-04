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
            Inactive = 0,   // ��������Ă��Ȃ�
            Active,         // ��ɂ���
            Spawning,       // ������
            Drop,
        }

        private Action<GameObject> mActiveCallBack = null;
        private GameObject mSilkShadow;     // ���̎��̉e
        //TODO �Ǎۂɂ���Ƃ��Ɏg���ϐ��i�������j
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
                // ��������Ă��Ȃ��Ƃ��͏������Ȃ�
                case State.Inactive:
                    break;
                // ������ԂƂ��̏���
                case State.Spawning:
                    // �����A�j���[�V�������X�V����
                    UpdateSpawnAnimation();
                    break;
                case State.Drop:
                    UpdateDropAnimation();
                    break;
            }

        }

        /// <summary>
        /// �����A�j���[�V�������X�V����֐�
        /// </summary>
        
        private void UpdateSpawnAnimation()
        {
            transform.Translate(0, 0, -300.0f / Global.SILK_SPAWN_TIME * Time.deltaTime);
            transform.localScale -= Vector3.one * Time.deltaTime * 2.0f / Global.SILK_SPAWN_TIME;
            mSilkShadow.transform.localScale += Vector3.one * Time.deltaTime * 2.0f / Global.SILK_SPAWN_TIME;
        }

        /// <summary>
        /// �����A�j���[�V����������������֐�
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
        /// �����A�j���[�V�����̏�Ԃ����Z�b�g����֐�
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

            // �����A�j���[�V����������������
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
