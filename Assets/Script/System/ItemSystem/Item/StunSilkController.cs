using Character;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunSilkController : NetworkBehaviour
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

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().OnEffect("Stun");
            NetworkServer.Destroy(gameObject);
        }
        if(other.CompareTag("Wall"))
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}

public static class StringExtensions
{
    public static string ToTitleCast(this string s)
    {
        if(String.IsNullOrEmpty(s))
        {
            throw new ArgumentException("String is null or empty");
        }
        string titleCastStr = s[0].ToString().ToUpper() + s.Substring(1).ToLower();

        return titleCastStr;
    }
}
