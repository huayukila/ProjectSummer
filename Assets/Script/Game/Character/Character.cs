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
            public float MaxMoveSpeed;                        // 最大速度
            [Min(0.0f)]
            [SerializeField]
            public float RotationSpeed;                       // 回転速度

        }
        [Min(0.0f)]
        protected float _acceleration;                        // 加速度
        protected CharaStatus _status;

    }

}
