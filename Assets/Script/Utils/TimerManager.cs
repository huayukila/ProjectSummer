using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : Singleton<TimerManager>
{
    [SerializeField]
    private List<ITimer> m_TimerUpdateList;
    private Stack<ITimer> m_ReadyToUpdateTimerStack;

    protected override void Awake()
    {
        base.Awake();
        m_TimerUpdateList = new List<ITimer>();
        m_ReadyToUpdateTimerStack = new Stack<ITimer>();
    }

    // Update is called once per frame
    void Update()
    {
        AddToDoTimerToUpdateList();
        UpdateTimer();
        RemoveFinishedTimer();
    }

    public void AddTimer(ITimer timer)
    {
        m_ReadyToUpdateTimerStack.Push(timer);
    }
    private void AddToDoTimerToUpdateList()
    {
        if (m_ReadyToUpdateTimerStack.Count == 0)
            return;
        while (m_ReadyToUpdateTimerStack.Count > 0)
        {
            var timer = m_ReadyToUpdateTimerStack.Pop();
            timer.Start();
            m_TimerUpdateList.Add(timer);
        }
    }

    private void UpdateTimer()
    {
        if (m_TimerUpdateList.Count == 0)
            return;
        foreach(var timer in m_TimerUpdateList)
        {
            timer.Update(Time.deltaTime);
        }

    }

    private void RemoveFinishedTimer()
    {
        if (m_TimerUpdateList.Count == 0)
            return;
        int cnt = 0;
        while(cnt < m_TimerUpdateList.Count)
        {
            if (m_TimerUpdateList[cnt].IsFinished())
            {
                m_TimerUpdateList.RemoveAt(cnt);
                continue;
            }
            cnt++;
        }
    }
}
