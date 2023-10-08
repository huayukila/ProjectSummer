using System;

public interface IActionExecutor
{
    void Execute(IAction action,Action<IActionController> onFinish=null);
}


public static class IActionExecutorExtensions
{
    public static bool UpdateAction(this IActionExecutor self,IAction action,
        float deltaTime,Action<IActionController> onFinish = null)
    {
        if (!action.Deinited && action.Execute(deltaTime))
        {
            onFinish?.Invoke(new ActionController()
            {
                Action=action,
                ActionID=action.ActionID,
            });

            action.Deinit();
        }
        return action.Deinited;
    }
}