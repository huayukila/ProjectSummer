using System;

// IActionExecutor �C���^�[�t�F�[�X�́A�A�N�V���������s���邽�߂̃��\�b�h���`���܂��B
public interface IActionExecutor
{
    // Execute ���\�b�h�́A�w�肳�ꂽ�A�N�V���������s���A�I�v�V�����Ŋ������̃R�[���o�b�N��񋟂��܂��B
    void Execute(IAction action, Action<IActionController> onFinish = null);
}

// IActionExecutorExtensions �N���X�́AIActionExecutor �C���^�[�t�F�[�X�̊g�����\�b�h��񋟂��܂��B
public static class IActionExecutorExtensions
{
    // UpdateAction ���\�b�h�́A�A�N�V�����̏�Ԃ��X�V���A�K�v�ɉ����ăA�N�V�������I�����܂��B
    public static bool UpdateAction(this IActionExecutor self, IAction action,
        float deltaTime, Action<IActionController> onFinish = null)
    {
        // �A�N�V�������܂��I�����Ă��Ȃ��ꍇ�A�A�N�V���������s���܂��B
        if (!action.Deinited && action.Execute(deltaTime))
        {
            // �A�N�V���������������ꍇ�A�����R�[���o�b�N���Ăяo���܂��B
            onFinish?.Invoke(new ActionController()
            {
                Action = action,
                ActionID = action.ActionID,
            });

            // �A�N�V�������I���������܂��B
            action.Deinit();
        }
        // �A�N�V�����̏I����Ԃ�Ԃ��܂��B
        return action.Deinited;
    }
}