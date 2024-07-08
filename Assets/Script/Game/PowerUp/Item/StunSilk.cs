using Character;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemSystem/Item/StunSilk", fileName = "StunSilk")]
public class StunSilk: ThrowItem
{
    public override void OnUse(Player player)
    {
        base.OnUse(player);
        BoxCollider stunSilkCol = ThrowObj.GetComponent<BoxCollider>();
        if (stunSilkCol != null)
        {
            Vector3 throwObjDropPos = player.transform.position + player.transform.forward * (player.ItemPlaceOffset * 2f + stunSilkCol.size.x * 0.5f);
            GameObject stunSilk = Instantiate(ThrowObj, throwObjDropPos, player.transform.rotation);
            NetworkServer.Spawn(stunSilk);
            //player.GetComponent<GamePlayer>().CmdOnItemSpawn(stunSilk);
        }
    }

}
