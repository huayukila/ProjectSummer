using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintOnObject 
{
    public abstract class Painter : MonoBehaviour
    {
        public Color color;
        [Min(0.01f)] public float radius = 1;
        public float strength = 1;
        public float hardness = 1;
    }

}
