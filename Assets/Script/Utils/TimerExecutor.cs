using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimerExecutor
{
    void AddTimer(ITimer timer);
}
public class TimerExecutor : MonoBehaviour,ITimerExecutor
{
    private Stack<ITimer> m_PrepareUpdateTimers;
    private List<ITimer> m_UpdatingTimers;
    // Start is called before the first frame update
    void Awake()
    {
        m_PrepareUpdateTimers = new Stack<ITimer>();
        m_UpdatingTimers = new List<ITimer>();
    }
    // Update is called once per frame
    void Update()
    {
        AddPrepareTimerToUpdate();
        UpdateTimers();
        CheckFinishedTimers();
    }

    private void AddPrepareTimerToUpdate()
    {
        while (m_PrepareUpdateTimers.Count > 0)
        {
            var timer = m_PrepareUpdateTimers.Pop();
            m_UpdatingTimers.Add(timer);
        }
    }
    private void UpdateTimers()
    {
        if (m_UpdatingTimers.Count == 0)
            return;
        foreach (var timer in m_UpdatingTimers)
        {
            timer.onUpdate(Time.deltaTime);
        }
    }

    private void CheckFinishedTimers()
    {
        if (m_UpdatingTimers.Count == 0)
            return;
        int cnt = 0;
        while (cnt < m_UpdatingTimers.Count)
        {
            if (m_UpdatingTimers[cnt].IsFinished())
            {
                if (m_UpdatingTimers[cnt].IsWaitForRepeat())
                {
                    m_UpdatingTimers[cnt].onReset();
                    m_UpdatingTimers[cnt].onStart();
                }
                else
                {
                    m_UpdatingTimers.RemoveAt(cnt);
                    continue;
                }
            }
            cnt++;
        }

    }
    public void AddTimer(ITimer timer)
    {
        m_PrepareUpdateTimers.Push(timer);
    }

    private void OnDestroy()
    {
        m_PrepareUpdateTimers.Clear();
        m_UpdatingTimers.Clear();
    }
}
