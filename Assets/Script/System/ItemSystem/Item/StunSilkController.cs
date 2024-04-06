using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunSilkController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().OnStun();
            Destroy(gameObject);
        }
    }
}
