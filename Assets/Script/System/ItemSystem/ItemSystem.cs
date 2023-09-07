using System.Collections.Generic;
using UnityEngine;

public class ItemSystem : SingletonBase<ItemSystem>
{
    GameObject itemPrefab;
    GameObject itemObj;
    //糸持ってないやつ獲得できる
    List<Item> attackItemList = new List<Item>();
    //持っているやつ
    List<Item> dfenteItemList = new List<Item>();

    ItemData data;

    System.Random rand = new System.Random((int)Time.time);
    public void Init()
    {
        itemPrefab = (GameObject)Resources.Load("Prefabs/ItemPrefab");
        if(itemPrefab != null)
        {
            Debug.Log("ItemPrefabロード成功");
        }
        data = (ItemData)Resources.Load("ItemDataTable");
        if (data != null)
        {
            attackItemList = data.PowerList;
            dfenteItemList = data.NormalList;
            Debug.Log("Itemデータロード成功");
        }
        itemObj = GameObject.Instantiate(itemPrefab);
        itemObj.SetActive(false);
        TypeEventSystem.Instance.Register<PlayerGetItem>(e =>
        {
            if (!e.player.isHadItem)
            {
                GiveItem(e.player);
                itemObj.SetActive(false);
            }
        }).UnregisterWhenGameObjectDestroyde(GameManager.Instance.gameObject);
    }

    public void SpawnItem()
    {
        //道具生成
    }

    private void GiveItem(IPlayer player)
    {
        Item giveItem;
        if (!player.isHadSilk)
        {
            giveItem = attackItemList[rand.Next(0, attackItemList.Count)];
        }
        else
        {
            giveItem = dfenteItemList[rand.Next(0, dfenteItemList.Count)];
        }
        player.GetItem(giveItem);
    }
}
