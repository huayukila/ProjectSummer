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

    // �_���Ǘ����郊�X�g����O���āA�T�[�o�[�������
    [Server]
    public void DestroySelf()
    {
        _destroyCallback?.Invoke(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
