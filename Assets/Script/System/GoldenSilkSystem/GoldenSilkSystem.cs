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
        private GameObject mGoldenSilkPrefab;            // 金の糸のプレハブ

        private Stack<GameObject> mGoldenSilkPool = new Stack<GameObject>();        // 金の糸を保存するスタック
        private IGameObjectFactory mFactory;                                         // GameObjectを作成するファクトリー

        // 生成されてた金の糸の数
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
        /// 金の網の生成位置を決める関数
        /// 未完成のため、固定位置に生成している
        /// </summary>
        /// <returns></returns>
        private Vector3 GetInSpaceRandomPosition()
        {
            // ステージの一定範囲内にインスタンス化する
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
        /// 金の網が落ちたときのステータスを設定する
        /// </summary>
        private void SetDropSilkStatus()
        {
            mGoldenSilkPrefab.SetActive(true);
        }

        /// <summary>
        /// イベントを登録する関数
        /// </summary>
        private void EventRegister()
        {
            TypeEventSystem.Instance.Register<PickSilkEvent>(e =>
            {
                AudioManager.Instance.PlayFX("SpawnFX", 0.7f);

            });
        }

        /// <summary>
        /// 金の糸を配給する
        /// </summary>
        /// <returns>金の糸</returns>
        public GameObject DropNewSilk()
        {
            // プールにないときは新しいのを作って返す
            GameObject newSilk = Allocate();

            //生成したGoldenSilkのセットアップ
            GoldenSilkControl ctrl = newSilk.GetComponent<GoldenSilkControl>();
            ctrl.StartDrop(GetInSpaceRandomPosition());

            return newSilk;
        }

        /// <summary>
        /// 使い切った金の糸を回収する
        /// </summary>
        /// <param name="obj">金の糸</param>
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
