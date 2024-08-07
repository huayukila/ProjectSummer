using System;
using UnityEngine;
using Character;

public interface IItem
{
    //IPlayer2ItemSystem用関数
    void Use(Player player);
}

[Serializable]
public class ItemBase : ScriptableObject, IItem
{
    public string itemName;
    public int id;
    public Sprite ItemImage;
    void IItem.Use(Player player)
    {
        OnUse(player);
    }

    //Item実際のロジックコードはここに書いてください
    public virtual void OnUse(Player player)
    {
        if(player == null)
            return;
        Debug.Log(itemName); //ひとまずデバッグ
    }

    protected ItemBase()
    {
        itemName = GetType().Name;
    }
}