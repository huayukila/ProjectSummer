using System;
using Unity.VisualScripting;
using UnityEngine;

public enum ActionStatus
{
    NotStart,
    Started,
    Finished,
}

public interface IActionController
{
    ulong ActionID { get; set; }
    IAction Action { get; set; }
    bool Paused { get; set; }
    void Rest();
    void Deinit();
}

public interface IAction<TStatus>
{
    ulong ActionID { get; set; }
    TStatus Status { get; set; }
    void OnStart();
    void OnExecute(float deltaTime);
    void OnFinish();

    bool Deinited { get; set; }

    bool Paused { get; set; }
    void Reset();
    void Deinit();
}


public interface IAction : IAction<ActionStatus>
{
}

public struct ActionController : IActionController
{
    public ulong ActionID { get; set; }
    public IAction Action { get; set; }
    public bool Paused { get; set; }

    public void Deinit()
    {
        if(Action.ActionID==ActionID)
        {
            Action.Reset();
        }
    }

    public void Rest()
    {
        if(Action.ActionID == ActionID)
        {
            Action.Deinit();
        }
    }
}
public static class IActionExtensions
{
    public static IActionController Start(this IAction self,
        MonoBehaviour monoBehaviour, Action<IActionController> onFinish = null)
    {
        monoBehaviour.ExecuteByUpdate(self, onFinish);
        return new ActionController()
        {
            ActionID = self.ActionID,
            Action = self,
        };
    }

    public static IActionController Start(this IAction self,
        MonoBehaviour monoBehaviour,Action onFinish)
    {
        monoBehaviour.ExecuteByUpdate(self, _ => onFinish());
        return new ActionController()
        {
            Action = self,
            ActionID = self.ActionID,
        };
    }

    public static void Finish(this IAction self)
    {
        self.Status= ActionStatus.Finished;
    }
    public static bool Execute(this IAction self,float deltaTime)
    {
        if (self.Status == ActionStatus.NotStart)
        {
            self.OnStart();

            if(self.Status== ActionStatus.Finished)
            {
                self.OnFinish();
                return true;
            }

            self.Status = ActionStatus.Started;
        }
        else if(self.Status == ActionStatus.Started)
        {
            if(self.Paused)
            {
                return false;
            }
            self.OnExecute(deltaTime);

            if(self.Status==ActionStatus.Finished)
            {
                self.OnFinish();
                return true;
            }
        }
        else if(self.Status == ActionStatus.Finished)
        {
            self.OnFinish();
            return true;
        }
        return false;
    }
}