using System;

// IActionExecutor インターフェースは、アクションを実行するためのメソッドを定義します。
public interface IActionExecutor
{
    // Execute メソッドは、指定されたアクションを実行し、オプションで完了時のコールバックを提供します。
    void Execute(IAction action, Action<IActionController> onFinish = null);
}

// IActionExecutorExtensions クラスは、IActionExecutor インターフェースの拡張メソッドを提供します。
public static class IActionExecutorExtensions
{
    // UpdateAction メソッドは、アクションの状態を更新し、必要に応じてアクションを終了します。
    public static bool UpdateAction(this IActionExecutor self, IAction action,
        float deltaTime, Action<IActionController> onFinish = null)
    {
        // アクションがまだ終了していない場合、アクションを実行します。
        if (!action.Deinited && action.Execute(deltaTime))
        {
            // アクションが完了した場合、完了コールバックを呼び出します。
            onFinish?.Invoke(new ActionController()
            {
                Action = action,
                ActionID = action.ActionID,
            });

            // アクションを終了処理します。
            action.Deinit();
        }
        // アクションの終了状態を返します。
        return action.Deinited;
    }
}