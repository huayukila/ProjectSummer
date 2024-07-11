using System;
using System.Collections.Generic;

public interface ITimerUnregister
{
    void Unregister(Action TimerCallback);
}

public struct TimerUnregisterContainer
{
    public ITimerUnregister UnregisterTimer;
    public List<Action> TimerCallbacks;

    public void OnUnregister()
    {
        foreach(var action in TimerCallbacks)
        {
            UnregisterTimer.Unregister(action);
        }

        UnregisterTimer = null;
        TimerCallbacks.Clear(); 
    }
}
