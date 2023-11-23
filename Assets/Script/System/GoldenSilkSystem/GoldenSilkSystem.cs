using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;
using TMPro.EditorUtilities;

namespace Gaming.PowerUp
{

    public class GoldenSilkSystem : SingletonBase<GoldenSilkSystem>
    {
        private GameObject mGoldenSilkPrefab;            // ���̎��̃v���n�u

        private Stack<GameObject> mGoldenSilkPool = new Stack<GameObject>();        // ���̎���ۑ�����X�^�b�N
        private GameObjectFactory mFactory;                                         // GameObject���쐬����t�@�N�g���[

        // ��������Ă����̎��̐�
        public int CurrentSilkCount
        {
            get
            {
                return Global.MAX_SILK_COUNT - mGoldenSilkPool.Count;
            }
        }

        public void Init()
        {
            mGoldenSilkPrefab = GameResourceSystem.Instance.GetPrefabResource("GoldenSilk");
            mFactory = new GameObjectFactory(() => Object.Instantiate(mGoldenSilkPrefab, Global.GAMEOBJECT_STACK_POS,Quaternion.identity));
            while(mGoldenSilkPool.Count < Global.MAX_SILK_COUNT)
            {
                mGoldenSilkPool.Push(mFactory.CreateObject());
            }
            Random.InitState((int)System.DateTime.Now.Ticks);
            EventRegister();
        }

        /// <summary>
        /// ���̖Ԃ̐����ʒu�����߂�֐�
        /// �������̂��߁A�Œ�ʒu�ɐ������Ă���
        /// </summary>
        /// <returns></returns>
        public Vector3 GetInSpaceRandomPosition()
        {
            // �X�e�[�W�̈��͈͓��ɃC���X�^���X������
            float spawnAreaLength = Global.STAGE_LENGTH / 2.5f;
            float spawnAreaWidth = Global.STAGE_WIDTH / 2.5f;
            float posX = 0.0f;
            float posZ = 0.0f;
            while (posX == 0.0f || posZ == 0.0f)
            {
                posX = Random.Range(-spawnAreaLength, spawnAreaLength);
                posZ = Random.Range(-spawnAreaWidth, spawnAreaWidth);
            }
            return new Vector3(posX, 0.54f, posZ);
        }
        /// <summary>
        /// ���̖Ԃ��������Ƃ��̃X�e�[�^�X��ݒ肷��
        /// </summary>
        private void SetDropSilkStatus()
        {
            mGoldenSilkPrefab.SetActive(true);
        }

        /// <summary>
        /// ���̖Ԃ������Ă����v���C���[�����񂾂���̖Ԃ��h���b�v����
        /// </summary>
        private void DropGoldenSilk(DropMode mode, Vector3 pos)
        {
            switch (mode)
            {
                case DropMode.Standard:
                    mGoldenSilkPrefab.transform.position = pos;
                    break;
                case DropMode.Edge:
                    mGoldenSilkPrefab.transform.position = pos;
                    /*
                    _awayFromEdgeStartPos = pos;
                    _awayFromEdgeEndPos = (pos - new Vector3(0.0f, 0.64f, 0.0f)) * 0.7f + new Vector3(0.0f, 0.64f, 0.0f) * 0.3f;
                    _isStartAwayFromEdge = true;
                    */
                    break;
            }
            SetDropSilkStatus();
        }

        /// <summary>
        /// �C�x���g��o�^����֐�
        /// </summary>
        private void EventRegister()
        {
            TypeEventSystem.Instance.Register<DropSilkEvent>(e =>
            {
                DropGoldenSilk(e.dropMode, e.pos);

            });
            TypeEventSystem.Instance.Register<PickSilkEvent>(e =>
            {
                AudioManager.Instance.PlayFX("SpawnFX", 0.7f);

            });
        }

        /// <summary>
        /// ���̎���z������
        /// </summary>
        /// <returns>���̎�</returns>
        public GameObject Allocate()
        {
            // �v�[���ɂȂ��Ƃ��͐V�����̂�����ĕԂ�
            return mGoldenSilkPool.Count == 0 ? mFactory.CreateObject() : mGoldenSilkPool.Pop();
        }
        /// <summary>
        /// �g���؂����I�u�W�F�N�g���������
        /// </summary>
        /// <param name="obj">�I�u�W�F�N�g</param>
        public void Recycle(GameObject obj)
        {
            if (mGoldenSilkPool.Count < Global.MAX_SILK_COUNT && obj != null)
            {
                obj.transform.position = Global.GAMEOBJECT_STACK_POS;
                mGoldenSilkPool.Push(obj);
            }
        }

    }

}
