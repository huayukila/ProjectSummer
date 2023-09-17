using UnityEngine;

public class DropPoint : MonoBehaviour
{
    Timer timer;        // DropPointのタイマー
    Timer _collidoractivetimer;
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
        _collidoractivetimer = new Timer();
        _collidoractivetimer.SetTimer(0.5f,
            () =>
            {
                gameObject.GetComponent<Collider>().enabled = true;
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
        if(_collidoractivetimer?.IsTimerFinished() == true)
        {
            _collidoractivetimer = null;
        }
    }


}
