using System;
using Unity.VisualScripting;
using UnityEngine;

// ActionStatus 列挙型は、アクションの実行状態を表します。
public enum ActionStatus
{
    NotStart,// 未開始
    Started,// 開始済み
    Finished,// 完了
}

// IActionController インターフェースは、アクションの制御と管理のためのメソッドを定義します。
public interface IActionController
{
    ulong ActionID { get; set; }
    IAction Action { get; set; }
    bool Paused { get; set; }
    void Rest();
    void Deinit();
}

// IAction<TStatus> インターフェースは、ジェネリックなアクションの基本的な構造を定義します。
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

// IAction インターフェースは、ActionStatus を使用する非ジェネリックなアクションのインターフェースです。
public interface IAction : IAction<ActionStatus>
{
}

// ActionController 構造体は、IActionController インターフェースの実装を提供します。
public struct ActionController : IActionController
{
    public ulong ActionID { get; set; }
    public IAction Action { get; set; }
    public bool Paused { get; set; }

    public void Deinit()
    {
        if (Action.ActionID == ActionID)
        {
            Action.Reset();
        }
    }

    public void Rest()
    {
        if (Action.ActionID == ActionID)
        {
            Action.Deinit();
        }
    }
}

// IActionExtensions クラスは、IAction インターフェースの拡張メソッドを提供します。
public static class IActionExtensions
{
    // Start メソッドは、アクションを開始し、アクションコントローラを返します。
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

    // このオーバーロードは、完了時のコールバックを単純なアクションで提供します。
    public static IActionController Start(this IAction self,
        MonoBehaviour monoBehaviour, Action onFinish)
    {
        monoBehaviour.ExecuteByUpdate(self, _ => onFinish());
        return new ActionController()
        {
            Action = self,
            ActionID = self.ActionID,
        };
    }

    // Finish メソッドは、アクションの状態を完了に設定します。
    public static void Finish(this IAction self)
    {
        self.Status = ActionStatus.Finished;
    }

    // Execute メソッドは、アクションの状態に応じて異なる処理を行います。
    public static bool Execute(this IAction self, float deltaTime)
    {
        if (self.Status == ActionStatus.NotStart)
        {
            self.OnStart();

            if (self.Status == ActionStatus.Finished)
            {
                self.OnFinish();
                return true;
            }

            self.Status = ActionStatus.Started;
        }
        else if (self.Status == ActionStatus.Started)
        {
            if (self.Paused)
            {
                return false;
            }

            self.OnExecute(deltaTime);

            if (self.Status == ActionStatus.Finished)
            {
                self.OnFinish();
                return true;
            }
        }
        else if (self.Status == ActionStatus.Finished)
        {
            self.OnFinish();
            return true;
        }

        return false;
    }
}