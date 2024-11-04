using Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunSilkController : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private Animator _animator;
    private void Awake()
    {
        if(!gameObject.TryGetComponent(out _rigidBody))
        {
            _rigidBody = gameObject.AddComponent<Rigidbody>();
        }

        _rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidBody.useGravity = false;
        _rigidBody.velocity = transform.forward * Global.STUN_SILK_SPEED;

        _animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isHit = false;
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StartCoroutine(OnHit());
            isHit = true;
        }

        if (other.TryGetComponent(out IItemAffectable itemAffectable))
        {
            itemAffectable.OnAffect(this);
            StartCoroutine(OnHit(2f));
            isHit = true;
        }

        if (isHit)
        {
            _rigidBody.velocity = Vector3.zero;
            var collider = GetComponent<Collider>();
            collider.enabled = false;
        }
    }

    private IEnumerator OnHit(float time = 0f)
    {
        _animator.Play("Hit");

        yield return new WaitForSeconds(0.1f);

        if (time <= 0f)
        {
            while(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(time);
        }

        Destroy(gameObject);
        yield break;
    }
}
