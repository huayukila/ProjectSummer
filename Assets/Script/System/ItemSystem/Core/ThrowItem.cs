using Character;
using Unity.Mathematics;
using UnityEngine;

public class ThrowItem : ItemBase
{
    public GameObject ThrowObj;

    public override void OnUse(Player player)
    {
        base.OnUse(player);
        Instantiate(ThrowObj, player.transform.position, quaternion.identity);
    }
}