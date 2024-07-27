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
            public float MaxMoveSpeed;                        // �ő呬�x
            [Min(0.0f)]
            [SerializeField]
            public float RotationSpeed;                       // ��]���x

        }
        [Min(0.0f)]
        protected float _acceleration;                        // �����x
        protected CharaStatus _status;

    }

}
