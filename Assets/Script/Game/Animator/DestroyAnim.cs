using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnim: MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
