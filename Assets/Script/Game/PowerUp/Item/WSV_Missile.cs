using Character;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ItemSystem/Item/Missile" , fileName = "Missile")]
public class Missile : ThrowItem
{
    public override void OnUse(Player player)
    {
        base.OnUse(player);
        CapsuleCollider missileCapsule = ThrowObj.GetComponent<CapsuleCollider>();
        if (missileCapsule != null)
        {
            Vector3 throwObjDropPos = player.transform.position - player.transform.forward * (player.ItemPlaceOffset * 2f + missileCapsule.height * 0.5f);
            GameObject missile = Instantiate(ThrowObj, throwObjDropPos, player.transform.rotation);
            NetworkServer.Spawn(missile);
            //player.GetComponent<GamePlayer>().CmdOnItemSpawn(bananaPeel);


            if(missile.TryGetComponent<IExplodable>(out IExplodable explodable))
            {
                // TODO player���g��Ȃ�
                PlayerInterfaceContainer container = player.GetContainer();
                IPlayerInfo playerInfo = container.GetInterface<IPlayerInfo>();
                if(playerInfo != null)
                {
                    explodable.SetupExplode(playerInfo.ID,playerInfo.AreaColor);
                }
            }
        }

    }
}
