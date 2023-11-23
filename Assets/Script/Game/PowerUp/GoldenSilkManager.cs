using Gaming.PowerUp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenSilkManager : Singleton<GoldenSilkManager>
{

    private Timer mDropSilkTimer;               // 金の糸を落下させるタイマー               
    // Start is called before the first frame update
    protected override void Awake()
    {
        
    }
    void Start()
    {
        // イベントを登録する
        TypeEventSystem.Instance.Register<SpawnSilkEvent>(e =>
        {
            e.silk.GetComponent<GoldenSilkControl>()?.SetState(State.Droping);

        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if ( mDropSilkTimer != null )
        {
            if (mDropSilkTimer.IsTimerFinished())
            {
                mDropSilkTimer = null;
            }
        }
        else if(GoldenSilkSystem.Instance.CurrentSilkCount < Global.MAX_SILK_COUNT)
        {
            mDropSilkTimer = new Timer();
            mDropSilkTimer.SetTimer(Global.SILK_SPAWN_TIME,
                () =>
                {
                    GameObject obj = GoldenSilkSystem.Instance.Allocate();
                    if(obj != null)
                    {
                        SpawnSilkEvent spawnSilkEvent = new SpawnSilkEvent
                        {
                            silk = obj,
                        };
                        TypeEventSystem.Instance.Send<SpawnSilkEvent>(spawnSilkEvent);

                    }
                }
                );
        }
    }
}
