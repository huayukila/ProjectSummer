using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Character
{
    public abstract class Character : MonoBehaviour
    {
        protected struct CharaStatus
        {
            [SerializeField]
            public float mMaxMoveSpeed;                        // 最大速度
            [Min(0.0f)]
            [SerializeField]
            public float mRotationSpeed;                       // 回転速度

        }
        [Min(0.0f)]
        protected float _acceleration;                        // 加速度
        protected CharaStatus mStatus;

    }

}
