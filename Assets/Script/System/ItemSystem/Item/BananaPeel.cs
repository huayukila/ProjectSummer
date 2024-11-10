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
            Vector3 throwObjDropPos = player.transform.position 
                                    - player.transform.forward 
                                    * (player.ColliderOffset * 2f + new Vector2(bananaBox.size.x, bananaBox.size.z).magnitude / 2f);
            Instantiate(ThrowObj, throwObjDropPos, Quaternion.identity);
        }

    }
}
