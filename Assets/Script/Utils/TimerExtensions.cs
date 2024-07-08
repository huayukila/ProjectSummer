using UnityEngine;
using Unity.VisualScripting;

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
            timer.OnTimerReset();
        }
        timer.OnTimerStart();
        self.GetOrAddComponent<TimerExecutor>().AddTimer(timer);
    }
}
