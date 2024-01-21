using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemTable")]
public class ItemTable : ScriptableObject
{
    public List<ItemBase> powerItemList;
    public List<ItemBase> normalItemList;
    public List<ItemBase> weakItemList;
}
