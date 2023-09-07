using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float duration;
    private float startTime;
    private Action callback;

    public Timer()
    {
        duration = 0f;
        startTime = 0f;
        callback = null;
    }

    public void SetTimer(float duration, Action callback) //タイマーの時間とコールバックをセットする
    {
        this.duration = duration;
        this.callback = callback;
        startTime = Time.time;
    }
    public bool IsTimerFinished()　　　　　　　　　　　　//タイマーが終わるかどうかの判断
    {
        if (callback == null)
        {
            return false;
        }
        if (Time.time - startTime >= duration)
        {
            callback.Invoke();
            return true;
        }
        return false;
    }
    public float GetTime()　　　　　　　　　　　　　　　//タイマーのリアルタイムのゲット
    {
        float remainingTime = duration - (Time.time - startTime);
        return remainingTime;
    }
}
