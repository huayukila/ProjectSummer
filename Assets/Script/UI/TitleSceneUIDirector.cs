using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject pressBtn;
    public bool isShiningOn = true;         //ブタン点滅のスイッチ
    public float blinkSpeed = 0.02f;       //ボタンの点滅変化の速度
    public float blinkInterval = 1.5f;       //ボタンの点滅一往復の時間
    public InputActionAsset _anyValueAction;

    public GameObject sceneSwitchCurtain;//scene切り替えのカーテン
    private float sceneSwitchCurtainSpeed;//scene切り替えのカーテンを黒くなるスピード
    private float sceneSwitchCurtainAlpha;//シーン切り替えカーテンの初期値

    private float blinkTimer = 0f;
    private InputAction _anyKeyAction;

    bool isCurtainTurnBlack;

    private void Awake()
    {
        ScoreSystem.Instance.ResetScore();
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
        _anyKeyAction.performed += OnSwitchScene;
    }
    private void Start()
    {
        sceneSwitchCurtainAlpha = 0f;//scene切り替えのカーテンの透明度初期値（完全透明）
        sceneSwitchCurtainSpeed = 0.1f;//scene切り替えのカーテンを黒くなるスピード
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);
    }
    private void FixedUpdate()
    {
        if (isShiningOn==true)
        {
            blinkTimer += Time.fixedDeltaTime;

            Color newColor = pressBtn.GetComponent<TextMeshProUGUI>().color;

            if (blinkTimer < blinkInterval * 0.5f)
            {
                newColor.a -= blinkSpeed;
            }
            else
            {
                newColor.a += blinkSpeed;
                if (newColor.a >= 1.0f)
                {
                    newColor.a = 1.0f;
                    blinkTimer = 0f;
                    Mathf.Sin(blinkTimer);
                }
            }
            pressBtn.GetComponent<TextMeshProUGUI>().color = newColor;
        }

        if (isCurtainTurnBlack == true) 
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
            TypeEventSystem.Instance.Send<MenuSceneSwitch>();
        }
    }

    private void OnEnable()=>_anyKeyAction.Enable();
    private void OnDisable()=>_anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

    private void OnSwitchScene(InputAction.CallbackContext context)
    { 
        if (context.performed)
        {
            isCurtainTurnBlack = true;       
        }
    }
}
