using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected float mMaxMoveSpeed;                        // 最大速度
    [Min(0.0f)]
    [SerializeField]
    protected float mAcceleration;                        // 加速度
    [Min(0.0f)]
    [SerializeField]
    protected float mRotationSpeed;                       // 回転速度

    protected abstract void Init();
}
