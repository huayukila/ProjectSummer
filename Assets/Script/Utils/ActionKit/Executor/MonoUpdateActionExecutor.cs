using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// MonoBehaviourを使用してUpdateループ内でアクションを実行するクラスです。
internal class MonoUpdateActionExecutor : MonoBehaviour, IActionExecutor
{
    // 実行準備中のアクションとそれらの終了時のコールバックを保持するリスト。
    List<KeyValuePair<IAction, Action<IActionController>>> mPrepareExecutionActions =
            new List<KeyValuePair<IAction, Action<IActionController>>>();

    // 実行中のアクションとそれらのコントローラに対するアクションのDic(IDによって探しやすいため)。
    Dictionary<IAction, Action<IActionController>> mExecutingActions =
        new Dictionary<IAction, Action<IActionController>>();

    // 回収するアクションのリスト。
    private List<IAction> mToActionRemove = new List<IAction>();

    // アクションを実行します。アクションの状態がFinishedの場合はリセットします。
    public void Execute(IAction action, Action<IActionController> onFinish = null)
    {
        if(action.Status == ActionStatus.Finished)
        {
            action.Reset();
        }
        if (this.UpdateAction(action, 0, onFinish))
        {
            return;
        }
        mPrepareExecutionActions.Add(new KeyValuePair<IAction, Action<IActionController>>(action, onFinish));
    }

    // UnityのUpdateメソッド。アクションを更新し、必要に応じて削除します。
    private void Update()
    {
        if(mPrepareExecutionActions.Count > 0)
        {
            foreach(var prepareExecutionAction in mPrepareExecutionActions)
            {
                if (mExecutingActions.ContainsKey(prepareExecutionAction.Key))
                {
                    mExecutingActions[prepareExecutionAction.Key] = prepareExecutionAction.Value;
                }
                else
                {
                    mExecutingActions.Add(prepareExecutionAction.Key, prepareExecutionAction.Value);
                }
            }

            mPrepareExecutionActions.Clear();
        }

        foreach(var actionAndFinishCallback in mExecutingActions)
        {
            if (this.UpdateAction(actionAndFinishCallback.Key, Time.deltaTime, actionAndFinishCallback.Value))
            {
                mToActionRemove.Add(actionAndFinishCallback.Key);
            }
        }

        if(mToActionRemove.Count > 0)
        {
            foreach(var action in mToActionRemove)
            {
                mExecutingActions.Remove(action);
            }
            mToActionRemove.Clear();
        }
    }
}

// MonoUpdateActionExecutorを拡張する静的クラスです。
public static class MonoUpdateActionExecutorExtension
{
    // 指定したMonoBehaviourにアクションを実行させる拡張メソッド。
    public static IAction ExecuteByUpdate<T>(this T self, IAction action,
        Action<IActionController> onFinish = null)
        where T : MonoBehaviour
    {
        if(action.Status == ActionStatus.Finished)
        {
            action.Reset();
        }
        self.gameObject.GetOrAddComponent<MonoUpdateActionExecutor>().Execute(action, onFinish);
        return action;
    }
}
