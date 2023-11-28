using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimType
{
    None = 0,
    Respawn
}

public interface IAnim
{
    public static bool isStopped { get; set; }

    public void Pause();

}

public abstract class CharacterAnim : MonoBehaviour, IAnim
{
    protected AnimType mType = AnimType.None;
    public static bool isStopped { get; set; } = true;
    public abstract void Pause();

    protected void SwitchAnimState(AnimType type)
    {
        if(isStopped)
        {
            mType = type;
        }
    }

}
