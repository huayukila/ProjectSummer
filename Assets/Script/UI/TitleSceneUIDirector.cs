using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject pressBtn;　　　　　　　//PRESS ANY BUTTONのオブジェクト
    public bool isShiningOn = true;          //ブタン点滅のスイッチ
    public float blinkSpeed = 0.02f;         //ボタンの点滅変化の速度
    public float blinkInterval = 1.5f;       //ボタンの点滅一往復の時間
    public InputActionAsset _anyValueAction;

    public GameObject sceneSwitchCurtain;//scene切り替えのカーテン
    private float sceneSwitchCurtainSpeed;//scene切り替えのカーテンを黒くなるスピード
    private float sceneSwitchCurtainAlpha;//シーン切り替えカーテンの初期値

    private float blinkTimer = 0f;　　　　　//点滅のタイマー
    private InputAction _anyKeyAction;

    public GameObject loadingScene;
    public GameObject LoadingSpider01;
    public GameObject LoadingSpider02;
    private bool isCurtainTurnBlackSwitchOn;
    private bool isCurtainTurnOn;
    private bool isLoadingAnimationOn;
    private bool isSceneSwitchOn;
    private bool isClickOnce;
    private float loadingSceneTimer;//ロードシーンの継続時間のタイマー
    private float loadingSceneTime;//ロードシーンの継続時間
    private float loadingSpider01x;
    private float loadingSpider02x;
    private float loadingSpider01MoveSpeed;
    private float loadingSpider02MoveSpeed;

    private void Awake()
    {
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
        _anyKeyAction.performed += OnSwitchScene;
    }
    private void Start()
    {
        sceneSwitchCurtainAlpha = 0f;//カーテンの透明度初期値（完全透明）
        sceneSwitchCurtainSpeed = 0.05f;//カーテンを黒くなるスピード
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態を初期化
        UISystem.DisplayOff(loadingScene);//ロードシーンを隠す
        loadingSpider01x = UISystem.GetPositionX(LoadingSpider01);//ロードシーンのスパイダーのx座標を獲得
        loadingSpider02x = UISystem.GetPositionX(LoadingSpider02);
        loadingSpider01MoveSpeed = 40f;//ロードシーンのスパイダーのスピードをセット
        loadingSpider02MoveSpeed = 40f;
        UISystem.SetPos(LoadingSpider01, -1200f, 0f);//ロードシーンのスパイダーの位置を初期化
        UISystem.SetPos(LoadingSpider02, 1200f, 0f);
        loadingSceneTimer = 0f;//ロードシーンの継続時間のタイマー
        loadingSceneTime = 2f;//ロードシーンの継続時間
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

        if (isCurtainTurnBlackSwitchOn == true)//カーテンを黒くするスイッチon!
        {
            CurtainTurnBlack();//カーテンを黒くする処理開始
        }
        if (isCurtainTurnOn == true)//カーテンを戻るスイッチon！
        {
            CurtainTurnOnAndLoadingSceneOn();//カーテンを戻るとロードシーンを現す処理
        }
        if (isLoadingAnimationOn == true)//ロードシーンのアニメションon！
        {
            LoadingAnimationOn();//ロードシーンのアニメション
        }
    }
    private void CurtainTurnBlack()//カーテンを黒くする処理開始
    {
        sceneSwitchCurtainAlpha += sceneSwitchCurtainSpeed;//カーテンを黒くする
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態
        if (sceneSwitchCurtainAlpha >= 1f)//カーテンが黒になった
        {
            isCurtainTurnBlackSwitchOn = false;////カーテンを黒くするスイッチoff!
            if (isSceneSwitchOn == true)//シーン切り替えのスイッチon!
            {
                isSceneSwitchOn = false;//シーン切り替えのスイッチをoffにする
                isClickOnce = false;//クリックされていない状態に戻る・クリック状態をリセット
                TypeEventSystem.Instance.Send<GamingSceneSwitch>();//シーンを切り替え
            }
            else//シーン切り替えのスイッチがoffの状態なら
            {
                isCurtainTurnOn = true;//カーテンを戻るスイッチon！
            }
        }
    }
    private void CurtainTurnOnAndLoadingSceneOn()//カーテンを戻るとロードシーンを現す処理
    {
        UISystem.DisplayOn(loadingScene);//ロードシーンを現す
        sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//カーテンを戻る
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態
        if (sceneSwitchCurtainAlpha <=0f)//カーテンを戻たら
        {
            sceneSwitchCurtainAlpha = 0f;//カーテンのAlphaを固定
            isCurtainTurnOn = false;//カーテンを戻るスイッチoff
            isLoadingAnimationOn = true;//ロードシーンのアニメションon！
        }
    }
    private void LoadingAnimationOn()//ロードシーンのアニメション
    {
        loadingSceneTimer += Time.fixedDeltaTime;//ロードシーンの継続時間をカウント初める
        UISystem.MoveToRight(LoadingSpider01, loadingSpider01x, loadingSpider01MoveSpeed);//スパイダーuiの行動処理
        UISystem.MoveToLeft(LoadingSpider02, loadingSpider02x, loadingSpider02MoveSpeed);//スパイダーuiの行動処理
        if (loadingSceneTimer > loadingSceneTime)//ロードシーンの継続時間に到達
        {
            loadingSceneTimer = 0;//タイマーをリセット
            isLoadingAnimationOn = false;//ロードシーンのアニメションをoff
            isSceneSwitchOn = true;//シーン切り替えのスイッチon!
            isCurtainTurnBlackSwitchOn = true;//カーテンを黒くするスイッチon!
        }
    }

    private void OnEnable()=>_anyKeyAction.Enable();
    private void OnDisable()=>_anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

    private void OnSwitchScene(InputAction.CallbackContext context)
    { 
        if (context.performed)
        {
            if(isClickOnce==false)//クリックされていない状態なら
            {
                AudioManager.Instance.PlayFX("ClickFX",0.5f);//クリックの音を鳴らす
                isCurtainTurnBlackSwitchOn = true;//カーテンを黒くするスイッチon!
                isClickOnce = true;//クリック済みの状態にする
            }
        }
    }
}
