using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected float mMaxMoveSpeed;                        // ç≈ëÂë¨ìx
    [Min(0.0f)]
    [SerializeField]
    protected float mAcceleration;                        // â¡ë¨ìx
    [Min(0.0f)]
    [SerializeField]
    protected float mRotationSpeed;                       // âÒì]ë¨ìx

    protected abstract void Init();
}
