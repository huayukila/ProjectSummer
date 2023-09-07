using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemDataTable",menuName = "ItemSystem/ItemDataTable")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    public List<Item> PowerList = new List<Item>();
    [SerializeField]
    public List<Item> NormalList = new List<Item>();
}
