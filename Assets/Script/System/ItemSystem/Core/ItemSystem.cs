using System.Linq;
using UnityEngine;

public class ItemSystem : SingletonBase<ItemSystem>
{
    GameObject itemObj;
    GameObject itemPrefab;
    //通常で獲得できる
    ItemBase[] normalItemArray;
    //糸持ってないやつ獲得できる
    ItemBase[] powerItemArray;
    //持っているやつ
    ItemBase[] weakItemArray;

    System.Random rand;

    public void Init()
    {
        rand = new System.Random((int)Time.time);
        InitItemArray();
        itemPrefab = (GameObject)Resources.Load("Prefabs/ItemPrefab");
        itemObj = Object.Instantiate(itemPrefab,
            new Vector3(100, 100, 100), Quaternion.identity);
        itemObj.SetActive(false);
        TypeEventSystem.Instance.Register<PlayerGetItem>(e =>
        {
            e.player.GetItem(LotteryItem(e.player));
        }).UnregisterWhenGameObjectDestroyed(GameManager.Instance.gameObject);
    }

    //道具生成
    public void SpawnItem()
    {
        if (itemObj != null)
        {
            itemObj.SetActive(true);
        }
        else
        {
            itemObj = Object.Instantiate(itemPrefab,
            new Vector3(100, 100, 100), Quaternion.identity);
        }
        itemObj.transform.position = new Vector3(980, 500, 10);
    }

    #region 内部用
    /// <summary>
    /// 各ItemPool初期化
    /// </summary>
    void InitItemArray()
    {
        ItemTable itemTable = (ItemTable)Resources.Load("ItemTable");

        powerItemArray = itemTable.powrItemList.ToArray();
        normalItemArray = itemTable.normalItemList.ToArray();
        weakItemArray = itemTable.weakitemList.ToArray();

        Resources.UnloadAsset(itemTable);

        if (normalItemArray != null && powerItemArray != null && weakItemArray != null)
        {
            Debug.Log("アイテムロード成功");
        }
    }


    //道具抽選
    private ItemBase LotteryItem(IPlayer2ItemSystem player)
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
}
