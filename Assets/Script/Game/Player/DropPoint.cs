using UnityEngine;

public class DropPoint : MonoBehaviour
{
    Timer timer;        // DropPointのタイマー

    void Awake()
    {
        // タイマーを初期化する
        timer = new Timer();
        timer.SetTimer(Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                // 時間が経ったらDropPointを消す
                Destroy(gameObject);
            }
            );
    }

    void Update()
    {
        // タイマーが終わるまでチェック
        if(timer.IsTimerFinished())
        {
            // どのプレイヤーが落としたDropPointをチェック
            if(gameObject.CompareTag("DropPoint1"))
            {
                DropPointManager.Instance.PlayerOneRemovePoint(gameObject);
            }
            else
            {
                DropPointManager.Instance.PlayerTwoRemovePoint(gameObject);
            }
        }
    }


}
