using System.Collections;
using System.Collections.Generic;
using Mirror;
using NaughtyAttributes.Test;
using UnityEngine;

public enum AnimType
{
    None = 0,
    Respawn,
    Dead,
}
public interface INetworkAnimationProcess
{
    void RpcUpdateAnimation();
    void SetAnimationType(AnimType type);
    public bool IsStopped { get; }
}
public abstract class CharacterAnim : NetworkBehaviour, INetworkAnimationProcess
{

    protected AnimType _animationType = AnimType.None;
    protected bool _bIsAnimationStopping = true;
    public bool IsStopped => _bIsAnimationStopping;

    [ClientRpc]
    public virtual void RpcUpdateAnimation() {}

    [ClientRpc]
    protected virtual void RpcResetAnimation() {}
    
    public virtual void SetAnimationType(AnimType type)
    {
        if(_animationType == type)
            return;

        _animationType = type;
        //RpcResetAnimation();
    }
}
