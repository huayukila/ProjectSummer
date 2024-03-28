using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ItemSystem/Item/BananaPeel" , fileName = "BananaPeel")]
public class BananaPeel : ThrowItem
{
    public override void OnUse(Player player)
    {
        base.OnUse(player);
        BoxCollider bananaBox = ThrowObj.GetComponent<BoxCollider>();
        if (bananaBox != null)
        {
            Vector3 throwObjDropPos = player.transform.position - player.transform.forward * (player.ColliderOffset * 2f + bananaBox.size.x / 2f);
            Debug.Log("throwObjDropPos");
            Instantiate(ThrowObj, throwObjDropPos, player.transform.rotation);
            Debug.LogError("Stop");
        }

    }
}
