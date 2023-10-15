using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSceneUIDirector : MonoBehaviour
{
    private InputAction _anyKeyAction;
    public InputActionAsset _anyValueAction;

    public GameObject sceneSwitchCurtain;//scene�؂�ւ��̃J�[�e��
    private float sceneSwitchCurtainSpeed;//scene�؂�ւ��̃J�[�e���������Ȃ�X�s�[�h
    private float sceneSwitchCurtainAlpha;//�V�[���؂�ւ��J�[�e���̏����l
    bool isCurtainTurnBlack;
    private void Awake()
    {
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
        _anyKeyAction.performed += OnSwitchScene;
    }
    private void Start()
    {
        sceneSwitchCurtainAlpha = 1f;//scene�؂�ւ��̃J�[�e���̓����x�����l�i���S�����j
        sceneSwitchCurtainSpeed = 0.1f;//scene�؂�ւ��̃J�[�e���𓧖��ɂȂ�X�s�[�h
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);
    }
    private void FixedUpdate()
    {
        if (isCurtainTurnBlack==false)
        {
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);
        }
        else
        {
            CurtainTurnBlackAndSceneSwitch();
        }
    }
    private void CurtainTurnBlackAndSceneSwitch()
    {
        sceneSwitchCurtainAlpha += sceneSwitchCurtainSpeed;
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);
        if (sceneSwitchCurtainAlpha >= 1f)
        {
            isCurtainTurnBlack = false;
            TypeEventSystem.Instance.Send<GamingSceneSwitch>();
        }
    }
    private void OnEnable() => _anyKeyAction.Enable();
    private void OnDisable() => _anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

    private void OnSwitchScene(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isCurtainTurnBlack = true;
        }

    }
}
