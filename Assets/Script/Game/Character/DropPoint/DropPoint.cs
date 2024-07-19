using System;
using Mirror;
using UnityEngine;

public class DropPoint : NetworkBehaviour
{
    private Timer _lifeTimeTimer;            // DropPointのタイマー
    private Action<GameObject> _destroyCallback;

    void Awake()
    {
        _lifeTimeTimer = new Timer(Time.time,Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                _destroyCallback?.Invoke(gameObject);

            });
        _lifeTimeTimer.StartTimer(this);
    }
    public void SetDestroyCallback(Action<GameObject> callback)
    {
        _destroyCallback = callback;
    }
}
