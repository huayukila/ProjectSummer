using System;
using UnityEngine;
[CreateAssetMenu(menuName ="AudioKit/AudioData",fileName ="New AudioData")]
[Serializable]
public class AudioData : ScriptableObject
{
    public string audioName;
    public AudioClip audioClip;
}