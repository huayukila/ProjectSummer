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

        private Action<GameObject> _activeCallBack = null;
        private GameObject _silkShadow;     // ���̎��̉e
        //TODO �Ǎۂɂ���Ƃ��Ɏg���ϐ��i�������j
        private Vector3 _dropStartPos = Vector3.zero;
        private Vector3 _dropEndPos = Vector3.zero;
        [SerializeField]
        private State _currentState = State.Inactive;

        // Start is called before the first frame update
        void Awake()
        {
            _silkShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("SilkShadow"), transform.position, Quaternion.identity);
            ResetAnimationStatus();
        }

        // Update is called once per frame
        void Update()
        {
            switch (_currentState)
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
            _silkShadow.transform.localScale += Vector3.one * Time.deltaTime * 2.0f / Global.SILK_SPAWN_TIME;
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
                    _currentState = State.Active;
                    _activeCallBack?.Invoke(gameObject);
                    TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
                });
            spawnTimer.StartTimer(this);

        }

        /// <summary>
        /// �����A�j���[�V�����̏�Ԃ����Z�b�g����֐�
        /// </summary>
        private void ResetAnimationStatus()
        {
            _silkShadow.transform.localScale = Vector3.zero;
            _silkShadow.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            _silkShadow.transform.position = Global.GAMEOBJECT_STACK_POS;
        }

        private void UpdateDropAnimation()
        {
            transform.position = Vector3.Lerp(_dropStartPos, _dropEndPos, Time.deltaTime * 2);
            _dropStartPos = transform.position;
            if((_dropStartPos - _dropEndPos).magnitude < 0.1f)
            {
                transform.position = _dropEndPos;
                _dropStartPos = Vector3.zero;
                _dropEndPos = Vector3.zero;
                OnSetState(State.Active);
                TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
            }
        }

        private void SetAnimationStartPosition(Vector3 position)
        {
            _silkShadow.transform.position = position - new Vector3(0, 0.2f, 0);
            transform.position = _silkShadow.transform.position + Vector3.forward * 150 + new Vector3(0, 0.2f, 0);
        }

        private void OnSetState(State state) => _currentState = state;

        public void StartSpawn(Vector3 position)
        {
            SetAnimationStartPosition(position);
            OnSetState(State.Spawning);

            // �����A�j���[�V����������������
            InitSpawnAnimation();
        }

        public void SetInactive()
        {
            _silkShadow.transform.position = Global.GAMEOBJECT_STACK_POS;
            transform.position = Global.GAMEOBJECT_STACK_POS;
            OnSetState(State.Inactive);
        }

        public void StartDrop(Vector3 startPos,Vector3 endPos)
        {
            _dropStartPos = startPos;
            _dropEndPos = endPos;
            OnSetState(State.Drop);
        }

        public void SetActiveCallBack(Action<GameObject> callback)
        {
            _activeCallBack = callback;
        }

        private void OnDestroy()
        {
            Destroy(_silkShadow);
        }
    }

}
