using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ParticleObject/Data", fileName = "New ParticleObject Data")]
[Serializable]
public class ParticleObjectData : ScriptableObject
{
    public string particleObjectname;
    public GameObject particleObject;
}
