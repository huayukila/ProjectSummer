using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITypeEventSystem
{
    void Send<T>() where T : new();
    void Send<T>(T e);

    IUnregister Register<T>(Action<T> onEvent);
    void Unregister<T>(Action<T> onEvent);
}
public interface IUnregister
{
    void Unregister();
}
public struct TypeEventSystemUnregister<T> : IUnregister
{
    public ITypeEventSystem TypeEventSystem;
    public Action<T> OnEvent;
    public void Unregister()
    {
        TypeEventSystem.Unregister<T>(OnEvent);
        TypeEventSystem = null;
        OnEvent = null;
    }
}
/// <summary>
/// MonoBehaviourの生命周期によって、自動的に事件解除
/// </summary>
public class UnregisterOnDestroyTrigger : MonoBehaviour
{
    private HashSet<IUnregister> mUnregistered = new HashSet<IUnregister>();

    public void AddUnregister(IUnregister unregister)
    {
        mUnregistered.Add(unregister);
    }

    private void OnDestroy()
    {
        foreach (var unRegister in mUnregistered)
        {
            unRegister.Unregister();
        }

        mUnregistered.Clear();
    }
}
/// <summary>
/// 事件解除静的エクステンション
/// </summary>
public static class UnregisterExtension
{
    public static void UnregisterWhenGameObjectDestroyde(this IUnregister unRegister, GameObject gameObject)
    {
        var trigger = gameObject.GetComponent<UnregisterOnDestroyTrigger>();
        if (!trigger)
        {
            trigger = gameObject.AddComponent<UnregisterOnDestroyTrigger>();
        }
        trigger.AddUnregister(unRegister);
    }
}
public class TypeEventSystem : SingletonBase<TypeEventSystem>,ITypeEventSystem
{
    public interface IRegistrations
    {

    }
    public class Registrations<T> : IRegistrations
    {
        public Action<T> OnEvent = e => { };
    }
    Dictionary<Type, IRegistrations> mEventRegistration = new Dictionary<Type, IRegistrations>();
    public IUnregister Register<TEvent>(Action<TEvent> onEvent)
    {
        var type = typeof(TEvent);
        IRegistrations registrations;

        if (mEventRegistration.TryGetValue(type, out registrations))
        {

        }
        else
        {
            registrations = new Registrations<TEvent>();
            mEventRegistration.Add(type, registrations);
        }
        (registrations as Registrations<TEvent>).OnEvent += onEvent;
        return new TypeEventSystemUnregister<TEvent>()
        {
            OnEvent = onEvent,
            TypeEventSystem = this
        };
    }
    public void Send<T>() where T : new()
    {
        var e = new T();
        Send<T>(e);
    }
    public void Send<T>(T e)
    {
        var type = typeof(T);
        IRegistrations registrations;

        if (mEventRegistration.TryGetValue(type, out registrations))
        {
            (registrations as Registrations<T>).OnEvent(e);
        }
    }
    public void Unregister<T>(Action<T> onEvent)
    {
        var type = typeof(T);
        IRegistrations registrations;
        if (mEventRegistration.TryGetValue(type, out registrations))
        {
            (registrations as Registrations<T>).OnEvent -= onEvent;
        }
    }
}