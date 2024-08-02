using WSV.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName ="ItemSystem/Item/BananaPeel" , fileName = "BananaPeel")]
public class BananaPeel : ThrowItem
{
    public override void OnUse(Player player)
    {
        base.OnUse(player);
        BoxCollider bananaBox = ThrowObj.GetComponent<BoxCollider>();
        if (bananaBox != null)
        {
            Vector3 throwObjDropPos = player.transform.position - player.transform.forward * (player.ItemPlaceOffset * 2f + bananaBox.size.x * 0.5f);
            GameObject bananaPeel = Instantiate(ThrowObj, throwObjDropPos, player.transform.rotation);
            NetworkServer.Spawn(bananaPeel);
            //player.GetComponent<GamePlayer>().CmdOnItemSpawn(bananaPeel);
        }

    }
}
