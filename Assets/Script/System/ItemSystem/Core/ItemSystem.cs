using System.Linq;
using Mirror.Examples.NetworkRoom;
using Unity.Mathematics;
using UnityEngine;

public interface IItemSystem : ISystem
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="spawnPos_"></param>
    GameObject SpawnItem(Vector3 spawnPos_);

    Vector3[] GetOnFieldItemBoxPos();
    Vector3[] GetOnFieldSilkPos();
    void InitItemSystem();
    void DestroyItem(GameObject obj);
}


public class ItemSystem : AbstractSystem, IItemSystem
{
    GameObject itemPrefab;

    //通常で獲得できる
    ItemBase[] normalItemArray;

    //糸持ってないやつ獲得できる
    ItemBase[] powerItemArray;

    //持っているやつ
    ItemBase[] weakItemArray;
    System.Random rand;

    ItemManager _itemManager;

    protected override void OnInit()
    {
        rand = new System.Random((int)Time.time);
        InitItemArray();
        itemPrefab = Resources.Load("Prefabs/Item/pfItemObject") as GameObject;
        TypeEventSystem.Instance.Register<PlayerGetItem>(e => { e.player.GetItem(LotteryItem(e.player)); })
            .UnregisterWhenGameObjectDestroyed(NetWorkRoomManagerExt.singleton.gameObject);
    }

    //道具生成
    public GameObject SpawnItem(Vector3 spawnPos_)
    {
        GameObject temp = GameObject.Instantiate(itemPrefab, spawnPos_, quaternion.identity);
        return temp;
    }

    public void RegisterManager(ItemManager itemManager)
    {
        _itemManager = itemManager;
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

    public Vector3[] GetOnFieldSilkPos()
    {
        return _itemManager.GetOnFieldSilkPos();
    }

    public Vector3[] GetOnFieldItemBoxPos() 
    {
        return _itemManager.GetOnFieldItemBoxPos();
    }

    public void InitItemSystem()
    {
        _itemManager.InitItemBox();
    }

    public void DestroyItem(GameObject obj)
    {
        _itemManager.DestroyItem(obj);
    }

    #endregion
}