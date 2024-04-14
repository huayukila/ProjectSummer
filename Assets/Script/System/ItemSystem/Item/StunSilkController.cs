using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunSilkController : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private void Awake()
    {
        if(!gameObject.TryGetComponent(out _rigidBody))
        {
            _rigidBody = gameObject.AddComponent<Rigidbody>();
        }
        _rigidBody.velocity = transform.forward * Global.STUN_SILK_SPEED;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().OnStun();
            Destroy(gameObject);
        }
        if(other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
