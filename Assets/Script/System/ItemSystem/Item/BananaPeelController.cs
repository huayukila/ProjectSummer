using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class BananaPeelController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IItemAffectable itemAffectable))
        {           
            itemAffectable.OnAffect(this);
            var collider = GetComponent<Collider>();
            collider.enabled = false;

            StartCoroutine(OnHit());
        }
    }

    private IEnumerator OnHit()
    {
        _animator.Play("Used");

        yield return new WaitForSecondsRealtime(0.1f);

        while(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}
