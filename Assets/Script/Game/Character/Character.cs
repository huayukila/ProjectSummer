using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected float mMaxMoveSpeed;                        // �ő呬�x
    [Min(0.0f)]
    [SerializeField]
    protected float mAcceleration;                        // �����x
    [Min(0.0f)]
    [SerializeField]
    protected float mRotationSpeed;                       // ��]���x

    protected abstract void Init();
}
