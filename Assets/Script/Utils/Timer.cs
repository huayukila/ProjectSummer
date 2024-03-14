using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public interface ITimer
{
    void onStart();
    void onUpdate(float deltaTime);
    void onPause();
    void onReset();
    bool IsFinished();
    void SetRepeatable(bool value);
    bool IsWaitForRepeat();

}
public class Timer : ITimer
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
    private bool IsRepeatable;
    public Timer()
    {
        m_Clock = new Clock()
        {
            StartTime = 0,
            Duration = 0,
            Interval = 0,
            State = Clock.ClockState.NotStart,
        };
        m_Callback = null;
        IsRepeatable = false;
    }

    public Timer(float startTime,float interval,Action callback = null)
    {
        m_Clock = new Clock()
        {
            StartTime = startTime,
            Duration = interval,
            Interval = interval,
            State = Clock.ClockState.NotStart,
        };
        m_Callback = callback;
        IsRepeatable = false;
    }
    public void onStart() => m_Clock.State = Clock.ClockState.Run;
    public void onPause() => m_Clock.State = Clock.ClockState.Pause;
    public void onStop() => m_Clock.State = Clock.ClockState.Finish;
    public void SetRepeatable(bool value) => IsRepeatable = value;

    public void onUpdate(float deltaTime)
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
    public void onReset()
    {
        m_Clock.Duration = m_Clock.Interval;
        m_Clock.StartTime = Time.time;
    }

    private bool IsRunning() => m_Clock.State == Clock.ClockState.Run;
    public bool IsFinished() => m_Clock.State == Clock.ClockState.Finish;
    public bool IsWaitForRepeat() => IsRepeatable;

}

public static class TimerExtension
{

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
            timer.onReset();
        }
        timer.onStart();
        self.GetOrAddComponent<TimerExecutor>().AddTimer(timer);
    }
}
