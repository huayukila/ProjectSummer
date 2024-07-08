using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimerExecutor
{
    void AddTimer(ITimer timer);
}
public class TimerExecutor : MonoBehaviour,ITimerExecutor
{
    private List<ITimer> m_PrepareUpdateTimers;
    private List<ITimer> m_UpdatingTimers;
    // Start is called before the first frame update
    void Awake()
    {
        m_PrepareUpdateTimers = new List<ITimer>();
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
            var timer = m_PrepareUpdateTimers[0];
            m_UpdatingTimers.Add(timer);
            m_PrepareUpdateTimers.RemoveAt(0);
        }
    }
    private void UpdateTimers()
    {
        if (m_UpdatingTimers.Count == 0)
            return;
        foreach (var timer in m_UpdatingTimers)
        {
            timer.OnTimerUpdate(Time.deltaTime);
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
                if (m_UpdatingTimers[cnt].IsRepeatable())
                {
                    m_UpdatingTimers[cnt].OnTimerReset();
                    m_UpdatingTimers[cnt].OnTimerStart();
                }
                else
                {
                    m_UpdatingTimers[cnt] = null;
                    m_UpdatingTimers.RemoveAt(cnt);
                    continue;
                }
            }
            cnt++;
        }

    }
    public void AddTimer(ITimer timer)
    {
        m_PrepareUpdateTimers.Add(timer);
    }

    private void OnDestroy()
    {
        m_PrepareUpdateTimers.Clear();
        m_UpdatingTimers.Clear();
    }
}
