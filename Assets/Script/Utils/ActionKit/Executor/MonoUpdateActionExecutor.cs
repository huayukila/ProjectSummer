using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// MonoBehaviour���g�p����Update���[�v���ŃA�N�V���������s����N���X�ł��B
internal class MonoUpdateActionExecutor : MonoBehaviour, IActionExecutor
{
    // ���s�������̃A�N�V�����Ƃ����̏I�����̃R�[���o�b�N��ێ����郊�X�g�B
    List<KeyValuePair<IAction, Action<IActionController>>> mPrepareExecutionActions =
            new List<KeyValuePair<IAction, Action<IActionController>>>();

    // ���s���̃A�N�V�����Ƃ����̃R���g���[���ɑ΂���A�N�V������Dic(ID�ɂ���ĒT���₷������)�B
    Dictionary<IAction, Action<IActionController>> mExecutingActions =
        new Dictionary<IAction, Action<IActionController>>();

    // �������A�N�V�����̃��X�g�B
    private List<IAction> mToActionRemove = new List<IAction>();

    // �A�N�V���������s���܂��B�A�N�V�����̏�Ԃ�Finished�̏ꍇ�̓��Z�b�g���܂��B
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

    // Unity��Update���\�b�h�B�A�N�V�������X�V���A�K�v�ɉ����č폜���܂��B
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

// MonoUpdateActionExecutor���g������ÓI�N���X�ł��B
public static class MonoUpdateActionExecutorExtension
{
    // �w�肵��MonoBehaviour�ɃA�N�V���������s������g�����\�b�h�B
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
