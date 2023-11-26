using Gaming.PowerUp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenSilkManager : Singleton<GoldenSilkManager>
{

    private Timer mDropSilkTimer;               // ���̎��𗎉�������^�C�}�[               
    // Start is called before the first frame update
    protected override void Awake()
    {
        
    }
    void Start()
    {
        // �C�x���g��o�^����
        TypeEventSystem.Instance.Register<SpawnSilkEvent>(e =>
        {
            //e.silk.GetComponent<GoldenSilkControl>()?.SetState(State.Droping);

        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (mDropSilkTimer != null){
            mDropSilkTimer.IsTimerFinished();
        }
        SetSilkSpawnTimer();
    }

    private void SetSilkSpawnTimer()
    {
        if (mDropSilkTimer != null)
            return;
        
        if (GoldenSilkSystem.Instance.CurrentSilkCount >= Global.MAX_SILK_COUNT)
            return;

        mDropSilkTimer = new Timer();
        mDropSilkTimer.SetTimer(
            Global.SILK_SPAWN_TIME,
            () =>
            {
                GameObject obj = GoldenSilkSystem.Instance.DropNewSilk();
                if (obj != null)
                {
                    SpawnSilkEvent spawnSilkEvent = new SpawnSilkEvent{
                        silk = obj,
                    };
                    TypeEventSystem.Instance.Send<SpawnSilkEvent>(spawnSilkEvent);
                }

                mDropSilkTimer = null;
            }
        );
    }
}
