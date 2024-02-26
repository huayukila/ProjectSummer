using System;
using Unity.VisualScripting;
using UnityEngine;

// ActionStatus �񋓌^�́A�A�N�V�����̎��s��Ԃ�\���܂��B
public enum ActionStatus
{
    NotStart,// ���J�n
    Started,// �J�n�ς�
    Finished,// ����
}

// IActionController �C���^�[�t�F�[�X�́A�A�N�V�����̐���ƊǗ��̂��߂̃��\�b�h���`���܂��B
public interface IActionController
{
    ulong ActionID { get; set; }
    IAction Action { get; set; }
    bool Paused { get; set; }
    void Rest();
    void Deinit();
}

// IAction<TStatus> �C���^�[�t�F�[�X�́A�W�F�l���b�N�ȃA�N�V�����̊�{�I�ȍ\�����`���܂��B
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

// IAction �C���^�[�t�F�[�X�́AActionStatus ���g�p�����W�F�l���b�N�ȃA�N�V�����̃C���^�[�t�F�[�X�ł��B
public interface IAction : IAction<ActionStatus>
{
}

// ActionController �\���̂́AIActionController �C���^�[�t�F�[�X�̎�����񋟂��܂��B
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

// IActionExtensions �N���X�́AIAction �C���^�[�t�F�[�X�̊g�����\�b�h��񋟂��܂��B
public static class IActionExtensions
{
    // Start ���\�b�h�́A�A�N�V�������J�n���A�A�N�V�����R���g���[����Ԃ��܂��B
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

    // ���̃I�[�o�[���[�h�́A�������̃R�[���o�b�N��P���ȃA�N�V�����Œ񋟂��܂��B
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

    // Finish ���\�b�h�́A�A�N�V�����̏�Ԃ������ɐݒ肵�܂��B
    public static void Finish(this IAction self)
    {
        self.Status = ActionStatus.Finished;
    }

    // Execute ���\�b�h�́A�A�N�V�����̏�Ԃɉ����ĈقȂ鏈�����s���܂��B
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