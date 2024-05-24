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
            public float mMaxMoveSpeed;                        // �ő呬�x
            [Min(0.0f)]
            [SerializeField]
            public float mRotationSpeed;                       // ��]���x

        }
        [Min(0.0f)]
        protected float _acceleration;                        // �����x
        protected CharaStatus mStatus;

    }

}
