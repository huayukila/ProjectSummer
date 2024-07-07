using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;
using Mirror;

public class BananaPeelController : NetworkBehaviour
{

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {           
            other.gameObject.GetComponent<Player>().OnEffect("Slip");
            NetworkServer.Destroy(gameObject);
        }
    }
}
