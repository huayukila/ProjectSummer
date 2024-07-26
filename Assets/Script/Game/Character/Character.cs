using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Character
{
    public abstract class Character : NetworkBehaviour
    {
        protected struct CharaStatus
        {
            [SerializeField]
            public float MaxMoveSpeed;                        // ç≈ëÂë¨ìx
            [Min(0.0f)]
            [SerializeField]
            public float RotationSpeed;                       // âÒì]ë¨ìx

        }
        [Min(0.0f)]
        protected float _acceleration;                        // â¡ë¨ìx
        protected CharaStatus _status;

    }

}
