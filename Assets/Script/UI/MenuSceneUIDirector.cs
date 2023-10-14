using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSceneUIDirector : MonoBehaviour
{
    private InputAction _anyKeyAction;
    public InputActionAsset _anyValueAction;

    public GameObject sceneSwitchCurtain;//scene切り替えのカーテン
    private float sceneSwitchCurtainSpeed;//scene切り替えのカーテンを黒くなるスピード
    private float sceneSwitchCurtainAlpha;//シーン切り替えカーテンの初期値
    bool isCurtainTurnBlack;
    private void Awake()
    {
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
        _anyKeyAction.performed += OnSwitchScene;
    }
    private void Start()
    {
        sceneSwitchCurtainAlpha = 1f;//scene切り替えのカーテンの透明度初期値（完全透明）
        sceneSwitchCurtainSpeed = 0.1f;//scene切り替えのカーテンを透明になるスピード
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
