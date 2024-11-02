using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxController : MonoBehaviour
{
    private Vector3 _defaultPos;
    // Start is called before the first frame update
    void Start()
    {
        _defaultPos = transform.position;
    }

    public void SetInactive()
    {
        transform.position = Global.GAMEOBJECT_STACK_POS;
        SetRespawnTimer();
    }

    private void SetRespawnTimer()
    {
        Timer respawnTimer = new Timer(Time.time,Global.ITEM_BOX_SPAWN_TIME,
        () =>
        {
            transform.position = _defaultPos;
        });
        respawnTimer.StartTimer(this);
    }

    public bool IsInactive => _defaultPos != transform.position;
}
