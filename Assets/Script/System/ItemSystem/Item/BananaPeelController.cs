using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class BananaPeelController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IItemAffectable itemAffectable))
        {           
            itemAffectable.OnAffect(this);
            Destroy(gameObject);
        }
    }
}
