using Character;
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
    public override void OnUse(Player player)
    {
        base.OnUse(player);
        Instantiate(ThrowObj, player.transform.position, 
            quaternion.identity);
    }
}