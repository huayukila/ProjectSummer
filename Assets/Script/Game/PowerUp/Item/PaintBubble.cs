using Character;
using Mirror;
using Org.BouncyCastle.Asn1.Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ItemSystem/Item/PaintBubble", fileName = "PaintBubble")]
public class PaintBubble : ThrowItem
{
    public override void OnUse(Player player)
    {
        GameObject bubble = Instantiate(ThrowObj,player.gameObject.transform.position,Quaternion.identity);
        NetworkServer.Spawn(bubble);
        
        if(bubble != null)
        {
            IExplodable explodeCtrl;
            if(!bubble.TryGetComponent(out explodeCtrl))
            {
                explodeCtrl = bubble.AddComponent<PaintBubbleController>();
            }

            BoxCollider playerCol = player.GetComponent<BoxCollider>();


            explodeCtrl.SetupExplode(player.ID, player.AreaColor);

        }
        
    }
}

