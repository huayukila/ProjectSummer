using System;
using UnityEngine;
using Character;

public interface IItem
{
    void Use(Player player);
}
[Serializable]
public class ItemBase:ScriptableObject,IItem 
{
    public string itemName;
    public int id;
    void IItem.Use(Player player)
    {
        OnUse(player);
    }
    public virtual void OnUse(Player player) {
        Debug.Log(itemName);
    }
}
