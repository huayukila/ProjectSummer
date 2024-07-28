using System;
using Mirror;
using UnityEngine;

public class DropPoint : NetworkBehaviour
{
    private event Action<GameObject> _destroyCallback;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf),Global.DROP_POINT_ALIVE_TIME);
    }
    public void SetDestroyCallback(Action<GameObject> callback)
    {
        _destroyCallback = callback;
    }

    // 点を管理するリストから外して、サーバーから消す
    [Server]
    public void DestroySelf()
    {
        _destroyCallback?.Invoke(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
