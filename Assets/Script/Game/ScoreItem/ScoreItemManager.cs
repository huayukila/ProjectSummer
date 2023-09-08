using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItemManager : Singleton<ScoreItemManager>
{
    GameObject _goldenSilkPrefab;
    GameObject _inSpaceSilk;
    Timer goldenSilkSpawnTimer;

    protected override void Awake()
    {
        base.Awake();

        goldenSilkSpawnTimer = new Timer();
        goldenSilkSpawnTimer.SetTimer(10.0f, () => { });

        _goldenSilkPrefab = (GameObject)Resources.Load("Prefabs/GoldenSilk");
    }


    private void Update()
    {
        if(_inSpaceSilk == null && goldenSilkSpawnTimer.IsTimerFinished())
        {
            _inSpaceSilk = Instantiate(_goldenSilkPrefab,GetInSpaceRandomPosition(), Quaternion.identity);
        }
    }

    private Vector3 GetInSpaceRandomPosition()
    {
        return Vector3.zero;
    }
}
