using System.Data;
using UnityEngine;

public class DropPoint : MonoBehaviour
{
    private Timer _mTimer;        // DropPointのタイマー


    void Awake()
    {
        // タイマーを初期化する
        _mTimer = new Timer();
        _mTimer.SetTimer(Global.DROP_POINT_ALIVE_TIME,
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
        if(_mTimer.IsTimerFinished())
        {
            int i = -1;
            string numString = null;
            if(tag.Length > 9)
            {
                numString = tag.Substring(9);
            }
            if(int.TryParse(numString,out int num) == true)
            {
                i = num;
            }
            DropPointSystem.Instance.RemovePoint(i, gameObject);
            // どのプレイヤーが落としたDropPointをチェック

        }
    }

}
