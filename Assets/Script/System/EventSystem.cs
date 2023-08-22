using System;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : SingletonBase<EventSystem>
{
    Dictionary<Event, Action> eventDic = new Dictionary<Event, Action>();
    public void Register(Event eventName,Action e)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            eventDic.Add(eventName, e);
        }
        else
        {
            Debug.Log(eventName + "‚ÍŠù‚É“o˜^‚³‚ê‚Ü‚µ‚½");
        }
    }
    public void Unregister(Event eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            eventDic.Remove(eventName);
        }
    }
    public void SendEvent(Event eventName)
    {
        eventDic[eventName]?.Invoke();
    }
}