using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            callback = null;
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

public interface ITimer
{
    void Start();
    void Update(float deltaTime);
    void Pause();
    void Reset();
    bool IsFinished();

}
public class NewTimer:ITimer
{
    private struct Clock
    {
        public enum ClockState
        {
            
            NotStart= 0,
            Run,
            Pause,
            Finish
        }
        public float StartTime;
        public float Duration;
        public float Interval;
        public ClockState State;
    }
    private Clock m_Clock;
    private Action m_Callback;
    public NewTimer()
    {
        m_Clock = new Clock()
        {
            StartTime = 0,
            Duration = 0,
            Interval = 0,
            State = Clock.ClockState.NotStart
        };
        m_Callback = null;
    }

    public NewTimer(float startTime,float interval,Action callback = null)
    {
        m_Clock = new Clock()
        {
            StartTime = startTime,
            Duration = interval,
            Interval = interval,
            State = Clock.ClockState.NotStart
        };
        m_Callback = callback;
    }
    public void Start() => m_Clock.State = Clock.ClockState.Run;
    public void Pause() => m_Clock.State = Clock.ClockState.Pause;
    public void Stop() => m_Clock.State = Clock.ClockState.Finish;

    public void Update(float deltaTime)
    {
        if (!IsRunning())
            return;
        m_Clock.Duration -= deltaTime;
        if (m_Clock.Duration <= 0.0f)
        {
            m_Clock.State = Clock.ClockState.Finish;
            m_Callback?.Invoke();
        }
    }
    public void Reset()
    {
        m_Clock.Duration = m_Clock.Interval;
        m_Clock.StartTime = Time.time;
    }

    private bool IsRunning() => m_Clock.State == Clock.ClockState.Run;
    public bool IsFinished() => m_Clock.State == Clock.ClockState.Finish;

}

public static class TimerExtension
{
    public static void AddTimerToManager(this ITimer self)
    {
        TimerManager.Instance.AddTimer(self);
    }

    public static void StartTimer(this ITimer self,MonoBehaviour monoBehaviour)
    {
        monoBehaviour.GetOrAddTimerExecutor(self);
    }
}

public static class MonoBehaviourTimerExtension
{
    public static void GetOrAddTimerExecutor<T>(this T self,ITimer timer) where T : MonoBehaviour
    {
        if (timer.IsFinished())
        {
            timer.Reset();
        }
        timer.Start();
        self.GetOrAddComponent<TimerExecutor>().AddTimer(timer);
    }
}
