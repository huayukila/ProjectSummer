using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class TitleSceneUIDirector : MonoBehaviour
{
    //press any button関連
    public GameObject pressBtn;　　　　　　　//PRESS ANY BUTTONのオブジェクト
    public GameObject pressBtn02;
    public float blinkSpeed;         //ボタンの点滅変化の速度
    private float blinkTimer;　　　　　//点滅のタイマー
    //シーン切り替え関連
    public InputActionAsset _anyValueAction;
    private InputAction _anyKeyAction;
    public GameObject sceneSwitchCurtain;//scene切り替えのカーテン
    private float sceneSwitchCurtainSpeed;//scene切り替えのカーテンを黒くなるスピード
    private float sceneSwitchCurtainAlpha;//シーン切り替えカーテンの初期値

    public GameObject InstructionScene;
    public GameObject loadingScene;
    public GameObject LoadingSpider01;
    public GameObject LoadingSpider02;
    private bool isShiningOn = true;          //press any button点滅のスイッチ
    private bool isCurtainTurnBlackSwitchOn;
    private bool isCurtainTurnOn;
    private bool isLoadingAnimationOn;
    private bool isSceneSwitchOn;
    private bool isClicked;
    private float sceneTimer;//ロードシーンの継続時間のタイマー
    private float loadingSceneTime;//ロードシーンの継続時間
    private float loadingSpider01x;
    private float loadingSpider02x;
    private float loadingSpider01MoveSpeed;
    private float loadingSpider02MoveSpeed;
    private float INTSTRUCTON_SCENE_TIME;
    private int clickTimes;

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
        UISystem.DisplayOff(InstructionScene);//ゲーム操作方法紹介画面を隠す
        UISystem.DisplayOff(loadingScene);//ロードシーンを隠す
        loadingSpider01x = UISystem.GetPositionX(LoadingSpider01);//ロードシーンのスパイダーのx座標を獲得
        loadingSpider02x = UISystem.GetPositionX(LoadingSpider02);
        loadingSpider01MoveSpeed = 40f;//ロードシーンのスパイダーのスピードをセット
        loadingSpider02MoveSpeed = 40f;
        UISystem.SetPos(LoadingSpider01, -1200f, 0f);//ロードシーンのスパイダーの位置を初期化
        UISystem.SetPos(LoadingSpider02, 1200f, 0f);
        sceneTimer = 0f;//ロードシーンの継続時間のタイマー
        loadingSceneTime = 2f;//ロードシーンの継続時間
        INTSTRUCTON_SCENE_TIME = Global.INTSTRUCTON_SCENE_TIME;//ゲーム操作方法紹介画面 PRESS無効の時間
        clickTimes = 0;//ボタンの押されて回数コントロラー

        //press any button関連
        UISystem.DisplayOff(pressBtn02); //ゲーム操作方法紹介画面のpress any buttonを隠す
        blinkTimer = 0f;//点滅のタイマー
        blinkSpeed = 2.2f;//ボタンの点滅変化の速度
    }
    private void FixedUpdate()
    {
        if (isShiningOn==true)//press any button点滅のスイッチがonなら
        {
            Blink(pressBtn);//press any buttonを点滅
            Blink(pressBtn02);
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
                isClicked = false;//クリックされていない状態に戻る・クリック状態をリセット
                clickTimes = 0;//クリック回数のコントロラーをリセット
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
        if (clickTimes == 1)//クリックが第一回目のばい
        {
            UISystem.DisplayOn(InstructionScene);//ゲーム操作方法紹介画面を現す
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//カーテンを戻る
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態
            if (sceneSwitchCurtainAlpha <= 0f)//カーテンを戻たら
            {
                sceneSwitchCurtainAlpha = 0f;//カーテンのAlphaを固定
                sceneTimer += Time.fixedDeltaTime;//タイマーを起動
                if(sceneTimer >= INTSTRUCTON_SCENE_TIME)//タイマーが一定の時間を満たす場合
                {
                    sceneTimer = 0f;//タイマーをリセット
                    isClicked = false;//クリックを可能な状態になる
                    isCurtainTurnOn = false;//カーテンを戻るスイッチoff
                    UISystem.DisplayOn(pressBtn02);//ゲーム操作方法紹介画面のpress any buttonを現す
                    isShiningOn = true;////press ant buttonの点滅状態をon
                }
            }
        }
        if (clickTimes == 2)//クリックが第２回目のばい
        {
            UISystem.DisplayOn(loadingScene);//ロードシーンを現す
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//カーテンを戻る
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態
            if (sceneSwitchCurtainAlpha <= 0f)//カーテンを戻たら
            {
                sceneSwitchCurtainAlpha = 0f;//カーテンのAlphaを固定
                isCurtainTurnOn = false;//カーテンを戻るスイッチoff
                isLoadingAnimationOn = true;//ロードシーンのアニメションon！
            }
        } 
    }
    private void LoadingAnimationOn()//ロードシーンのアニメション
    {
        sceneTimer += Time.fixedDeltaTime;//ロードシーンの継続時間をカウント初める
        UISystem.MoveToRight(LoadingSpider01, loadingSpider01x, loadingSpider01MoveSpeed);//スパイダーuiの行動処理
        UISystem.MoveToLeft(LoadingSpider02, loadingSpider02x, loadingSpider02MoveSpeed);//スパイダーuiの行動処理
        if (sceneTimer > loadingSceneTime)//ロードシーンの継続時間に到達
        {
            sceneTimer = 0;//タイマーをリセット
            isLoadingAnimationOn = false;//ロードシーンのアニメションをoff
            isSceneSwitchOn = true;//シーン切り替えのスイッチon!
            isCurtainTurnBlackSwitchOn = true;//カーテンを黒くするスイッチon!
        }
    }
    private void Blink(GameObject pressBtn)
    {
        blinkTimer += Time.fixedDeltaTime;

        Color newColor = pressBtn.GetComponent<TextMeshProUGUI>().color;

        newColor.a = (math.cos(blinkTimer * blinkSpeed) *0.2f)+0.8f;//newColor.aのあたいを0.6~1.0の間繰り返す。

        pressBtn.GetComponent<TextMeshProUGUI>().color = newColor;
    }

    private void OnEnable()=>_anyKeyAction.Enable();
    private void OnDisable()=>_anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

    private void OnSwitchScene(InputAction.CallbackContext context)
    { 
        if (context.performed)
        {
            if (Keyboard.current[Key.Escape].wasPressedThisFrame)
            {
                Application.Quit();
            }
            else
            {
                if (isClicked == false)//クリックされていない状態なら
                {
                    switch (clickTimes)
                    {
                        case 0:
                            clickTimes += 1;
                            AudioManager.Instance.PlayFX("ClickFX", 0.5f);//クリックの音を鳴らす
                            isCurtainTurnBlackSwitchOn = true;//カーテンを黒くするスイッチon!
                            isClicked = true;//クリック済みの状態にする
                            isShiningOn = false;//press ant buttonの点滅状態をoff
                            break;
                        case 1:
                            clickTimes += 1;
                            AudioManager.Instance.PlayFX("ClickFX", 0.5f);//クリックの音を鳴らす
                            isCurtainTurnBlackSwitchOn = true;//カーテンを黒くするスイッチon!
                            isClicked = true;//クリック済みの状態にする
                            break;
                    }
                }

            }
        }
    }
}
