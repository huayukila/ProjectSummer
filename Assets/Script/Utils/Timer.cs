using System;
public interface ITimer
{
    void OnTimerStart();
    void OnTimerUpdate(float deltaTime);
    void OnTimerPause();
    void OnTimerReset();
    void OnTimerStop();
    bool IsFinished();
    bool IsRepeatable();
    void SetRepeatable(bool value);

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
    private Clock _clock;
    private Action _callback;
    private bool _isRepeatable;
    public Timer()
    {
        _clock = new Clock()
        {
            StartTime = 0,
            Duration = 0,
            Interval = 0,
            State = Clock.ClockState.NotStart,
        };
        _callback = null;
        _isRepeatable = false;
    }

    public Timer(float startTime,float interval,Action callback = null)
    {
        _clock = new Clock()
        {
            StartTime = startTime,
            Duration = interval,
            Interval = interval,
            State = Clock.ClockState.NotStart,
        };
        _callback = callback;
        _isRepeatable = false;
    }
    public void OnTimerStart() => _clock.State = Clock.ClockState.Run;
    public void OnTimerPause() => _clock.State = Clock.ClockState.Pause;
    public void OnTimerStop() => _clock.State = Clock.ClockState.Finish;
    public void SetRepeatable(bool value) => _isRepeatable = value;

    public void OnTimerUpdate(float deltaTime)
    {
        if (!IsRunning())
            return;

        _clock.Duration -= deltaTime;
        if (_clock.Duration <= 0.0f)
        {
            _clock.State = Clock.ClockState.Finish;
            _callback?.Invoke();
        }
    }
    public void OnTimerReset()
    {
        _clock.Duration = _clock.Interval;

        #if UNITY_EDITOR
            _clock.StartTime = UnityEngine.Time.time;
        #else
            _clock.StartTime = 0f;
        #endif

    }

    private bool IsRunning() => _clock.State == Clock.ClockState.Run;
    public bool IsFinished() => _clock.State == Clock.ClockState.Finish;
    public bool IsRepeatable() => _isRepeatable;

}

