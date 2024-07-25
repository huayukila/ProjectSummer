using System.Collections;
using System.Collections.Generic;
using Mirror;
using NaughtyAttributes.Test;
using UnityEngine;


public interface INetworkAnimationProcess
{
    void RpcUpdateAnimation();
    void RpcSetAnimationType(AnimType type);
    public bool IsStopped { get; }
}
public abstract class CharacterAnim : NetworkBehaviour, INetworkAnimationProcess
{

    protected enum AnimType
    {
        None = 0,
        Respawn
    }
    protected AnimType _animationType = AnimType.None;
    protected bool _bIsAnimationStopping = true;
    public bool IsStopped => _bIsAnimationStopping;

    [ClientRpc]
    public virtual void RpcUpdateAnimation() {}

    [ClientRpc]
    protected virtual void RpcResetAnimation() {}

    [ClientRpc]
    public virtual void RpcSetAnimationType(AnimType type)
    {
        if(_animationType == type)
            return;

        _animationType = type;
        RpcResetAnimation();
    }

}
