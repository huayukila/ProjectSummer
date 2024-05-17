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
            public float mMaxMoveSpeed;                        // ç≈ëÂë¨ìx
            [Min(0.0f)]
            [SerializeField]
            public float mRotationSpeed;                       // âÒì]ë¨ìx

        }
        [Min(0.0f)]
        protected float _acceleration;                        // â¡ë¨ìx
        protected CharaStatus mStatus;

    }

}
