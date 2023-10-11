using System.Linq;
using UnityEngine;

public class ItemSystem : SingletonBase<ItemSystem>
{
    GameObject itemObj;
    GameObject itemPrefab;

    //通常で獲得できる
    Item[] normalItemArray;
    //糸持ってないやつ獲得できる
    Item[] powerItemArray;
    //持っているやつ
    Item[] weakItemArray;

    System.Random rand;

    #region 内部用
    /// <summary>
    /// 各ItemPool初期化
    /// </summary>
    void InitItemArray()
    {
        normalItemArray = new Item[]
        {

        };

        powerItemArray = new Item[]
        {

        };

        weakItemArray = new Item[]
        {

        };

        if (normalItemArray != null && powerItemArray != null && weakItemArray != null)
        {
            Debug.Log("アイテムロード成功");
        }
    }


    //道具抽選
    private Item LotteryItem(IPlayer2ItemSystem player)
    {

        if (player.HadSilk)
        {
            return weakItemArray[rand.Next(0, weakItemArray.Count())];
        }

        //silkは生成された、かつプレイヤーがsilkを持ってない場合、強力アイテムをあげる
        //if (!player.HadSilk)
        //{

        //}

        //普通の場合獲得できるのアイテム
        return normalItemArray[rand.Next(0, normalItemArray.Count())];
    }
    #endregion

    public void Init()
    {
        rand = new System.Random((int)Time.time);
        InitItemArray();

        itemObj = Object.Instantiate((GameObject)Resources.Load("Prefabs/ItemPrefab"),
            new Vector3(100, 100, 100), Quaternion.identity);
        if (itemObj != null)
        {
            Debug.Log("itemObjロード成功");
        }
        itemObj.SetActive(false);
        TypeEventSystem.Instance.Register<PlayerGetItem>(e =>
        {
            e.player.GetItem(LotteryItem(e.player));
        }).UnregisterWhenGameObjectDestroyed(GameManager.Instance.gameObject);
    }

    //道具生成
    public void SpawnItem()
    {
        itemObj.SetActive(true);
        itemObj.transform.position = new Vector3(980, 500, 10);
    }
}
