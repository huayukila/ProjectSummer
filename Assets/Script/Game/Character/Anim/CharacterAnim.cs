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

}

public abstract class CharacterAnim : MonoBehaviour, IAnim
{
    protected AnimType mType = AnimType.None;
    public static bool isStopped { get; set; } = true;

}
