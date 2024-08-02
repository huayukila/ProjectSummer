using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// 投げるアイテム
/// </summary>
public class ThrowItem: ItemBase
{
    //投げるItemのprefab
    public GameObject ThrowObj;

    //投げるItemには、一般的このOnUseメソッドをoverrideの必要がない
}