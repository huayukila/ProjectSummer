using System.Linq;
using Unity.Mathematics;
using UnityEngine;

interface IItemSystem
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="spawnPos_"></param>
    void SpawnItem(Vector3 spawnPos_);
}

public class ItemSystem : SingletonBase<ItemSystem>,IItemSystem
{
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
        itemPrefab = Resources.Load("Prefabs/Item/pfRandomItem") as GameObject;
        TypeEventSystem.Instance.Register<PlayerGetItem>(e => { e.player.GetItem(LotteryItem(e.player)); })
            .UnregisterWhenGameObjectDestroyed(GameManager.Instance.gameObject);
    }

    //道具生成
    public void SpawnItem(Vector3 spawnPos_)
    {
        GameObject.Instantiate(itemPrefab, spawnPos_, quaternion.identity);
    }

    #region 内部用

    /// <summary>
    /// 各ItemPool初期化
    /// </summary>
    void InitItemArray()
    {
        ItemTable itemTable = (ItemTable)Resources.Load("ItemTable");

        powerItemArray = itemTable.powerItemList.ToArray();
        normalItemArray = itemTable.normalItemList.ToArray();
        weakItemArray = itemTable.weakItemList.ToArray();

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

        //if (!player.HadSilk)
        //{

        //}

        //普通の場合獲得できるのアイテム
        return normalItemArray[rand.Next(0, normalItemArray.Count())];
    }

    #endregion
}