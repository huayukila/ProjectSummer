using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public float maxMoveSpeed;
    [Min(0.0f)] public float acceleration;
    [Min(0.0f)] public float rotationSpeed;
}
