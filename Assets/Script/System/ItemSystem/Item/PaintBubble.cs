using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ItemSystem/Item/PaintBubble", fileName = "PaintBubble")]
public class PaintBubble : ThrowItem
{
    public override void OnUse(Player player)
    {
        GameObject bubble = Instantiate(ThrowObj, player.gameObject.transform.position, Quaternion.identity);

        player.GetComponent<GamePlayer>().CmdOnItemSpawn(bubble);

        IExplodable explodeCtrl;
        if(!bubble.TryGetComponent(out explodeCtrl))
        {
            explodeCtrl = bubble.AddComponent<PaintBubbleController>();
        }

        BoxCollider playerCol = player.GetComponent<BoxCollider>();

        float explodeRadius = Mathf.Sqrt(
            Mathf.Pow(playerCol.size.x * player.transform.localScale.x, 2f) +
            Mathf.Pow(playerCol.size.z * player.transform.localScale.z, 2f)
            ) * 5f;

        explodeCtrl.SetExplodeProperty(player.ID, explodeRadius, player.AreaColor);


    }
}

