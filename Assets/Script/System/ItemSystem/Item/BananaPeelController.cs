using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class BananaPeelController : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {           
            other.gameObject.GetComponent<Player>().OnSlip();
            Destroy(gameObject);
        }
    }
}
