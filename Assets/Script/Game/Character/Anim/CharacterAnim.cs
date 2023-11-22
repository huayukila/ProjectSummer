using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimType
{
    None,
    Explode,
    Respawn
}

public interface IAnim
{
    public static bool isStopped { get; set; }
    public void Play();

    public void Pause();
    public void SwitchAnimState(AnimType type);

}

public abstract class CharacterAnim : MonoBehaviour, IAnim
{


    protected AnimType mType = AnimType.None;
    public bool isStopped { get; set; } = true;
    public abstract void Play();
    public abstract void Pause();

    public void SwitchAnimState(AnimType type)
    {
        if(isStopped)
        {
            mType = type;
        }
    }

}
