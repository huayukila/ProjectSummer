using System;
using UnityEngine;

public class DropPoint : MonoBehaviour
{
    private Timer _lifeTimeTimer;            // DropPointのタイマー
    private Action<GameObject> _destroyCallback;

    void Awake()
    {
        _lifeTimeTimer = new Timer(Time.time,Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                _destroyCallback.Invoke(gameObject);
                Debug.Log("Destroyed");
                Destroy(gameObject);
            });
        _lifeTimeTimer.StartTimer(this);
    }
    public void SetDestroyCallback(Action<GameObject> callback)
    {
        _destroyCallback = callback;
    }
}
