using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;
using TMPro.EditorUtilities;

namespace Gaming.PowerUp
{
    public interface ISilkEvent
    {
        GameObject DropNewSilk();
        void RecycleSilk(GameObject silk);
    }

    public class GoldenSilkSystem : SingletonBase<GoldenSilkSystem>, ISilkEvent
    {
        private GameObject mGoldenSilkPrefab;            // ���̎��̃v���n�u

        private Stack<GameObject> mGoldenSilkPool = new Stack<GameObject>();        // ���̎���ۑ�����X�^�b�N
        private IGameObjectFactory mFactory;                                         // GameObject���쐬����t�@�N�g���[

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
        private Vector3 GetInSpaceRandomPosition()
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
        /// �C�x���g��o�^����֐�
        /// </summary>
        private void EventRegister()
        {
            TypeEventSystem.Instance.Register<PickSilkEvent>(e =>
            {
                AudioManager.Instance.PlayFX("SpawnFX", 0.7f);

            });
        }

        /// <summary>
        /// ���̎���z������
        /// </summary>
        /// <returns>���̎�</returns>
        public GameObject DropNewSilk()
        {
            // �v�[���ɂȂ��Ƃ��͐V�����̂�����ĕԂ�
            GameObject newSilk = Allocate();

            //��������GoldenSilk�̃Z�b�g�A�b�v
            GoldenSilkControl ctrl = newSilk.GetComponent<GoldenSilkControl>();
            ctrl.StartDrop(GetInSpaceRandomPosition());

            return newSilk;
        }

        /// <summary>
        /// �g���؂������̎����������
        /// </summary>
        /// <param name="obj">���̎�</param>
        public void RecycleSilk(GameObject obj)
        {
            if (mGoldenSilkPool.Count < Global.MAX_SILK_COUNT && obj != null)
            {
                if (obj.GetComponent<GoldenSilkControl>() != null)
                {
                    obj.transform.position = Global.GAMEOBJECT_STACK_POS;
                    Recycle(obj);

                }
            }
        }

        private GameObject Allocate()
        {
            return mGoldenSilkPool.Count == 0 ? mFactory.CreateObject() : mGoldenSilkPool.Pop();
        }

        private void Recycle(GameObject obj)
        {
            if(mGoldenSilkPool.Count < Global.MAX_SILK_COUNT)
            {
                mGoldenSilkPool.Push(obj);
            }
        }
    }

}
