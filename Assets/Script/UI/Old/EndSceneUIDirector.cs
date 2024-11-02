using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class EndSceneUIDirector : MonoBehaviour
{
    public GameObject redScore;
    public GameObject yellowScore;
    public GameObject winner;
    public GameObject draw;
    public GameObject winYellow;
    public GameObject winRed;
    public GameObject pressAnyBtn;
    //public GameObject pressAnyBtn02;
    public GameObject creditsScene;

    private float timer;
    private float timerCon;
    private bool isTimerOn;
    
    private float alphaSetTMP;
    private float alphachangeTMP;
    private float localScaleSetTMP;
    private float localScaleChangeTMP;
    private float alphaSet;
    private float alphachange;
    private float localScaleSet;
    private float localScaleChange;

    //Blink Initialization--------------------
    public float blinkSpeed;       //ボタンの点滅変化の速度
    //public float blinkInterval = 1.5f;       //ボタンの点滅一往復の時間
    private float blinkTimer;
    private bool isBlinking;

    public GameObject sceneSwitchCurtain;//scene切り替えのカーテン
    private float sceneSwitchCurtainSpeed;//scene切り替えのカーテンを黒くなるスピード
    private float sceneSwitchCurtainAlpha;//シーン切り替えカーテンの初期値
    private bool isCurtainTurnOn;
    private bool isCurtainTurnBlack;
    private float sceneTimer;
    private float CREDITS_SCENE_TIME;

    private InputAction _anyKeyAction;
    public InputActionAsset _anyValueAction;
    [SerializeField]
    //private bool _isAnimationStopped;
    private int clickTimes;
    private bool isClicked;

    private void Awake()
    {
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
        _anyKeyAction.performed += OnSwitchScene;
        //_isAnimationStopped = false;
        AudioManager.Instance.PlayBGM("EndBGM", 0.3f);
    }
    private void Start()
    {
        timer = 3f;
        timerCon = 0.6f;
        isTimerOn = true;
        clickTimes = 0;
        isClicked = true;
        isCurtainTurnOn = true;
        isCurtainTurnBlack = false;

        //press any button点滅関連--------------------
        blinkTimer = 0f;
        blinkSpeed = 3.2f;       //ボタンの点滅変化の速度

        sceneSwitchCurtainAlpha = 1f;//scene切り替えのカーテンの透明度初期値（完全透明）
        sceneSwitchCurtainSpeed = 0.05f;//scene切り替えのカーテンを透明になるスピード
        sceneTimer= 0f;
        CREDITS_SCENE_TIME = Global.CREDITS_SCENE_TIME;
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);

        alphaSetTMP = 0.0f;
        alphachangeTMP = 0.025f;
        localScaleSetTMP = 20f;
        localScaleChangeTMP = 0.5f;

        alphaSet = 0.0f;
        alphachange = 0.025f;
        localScaleSet = 20f;
        localScaleChange = 0.5f;


        UISystem.SetPos(redScore,1500f,0f);
        UISystem.SetPos(yellowScore, 1500f, 0f);

        UISystem.SetLocalScale(draw, localScaleSetTMP, localScaleSetTMP, localScaleSetTMP);
        UISystem.SetAlphaTMP(draw,alphaSet);

        UISystem.SetLocalScale(winner, localScaleSetTMP, localScaleSetTMP, localScaleSetTMP);
        UISystem.SetAlphaTMP(winner, alphaSet);

        UISystem.SetLocalScale(winRed, localScaleSet, localScaleSet, localScaleSet);
        UISystem.SetAlpha(winRed, alphaSet);

        UISystem.SetLocalScale(winYellow, localScaleSet, localScaleSet, localScaleSet);
        UISystem.SetAlpha(winYellow, alphaSet);

        UISystem.DisplayOff(pressAnyBtn);
        //UISystem.DisplayOff(pressAnyBtn02);
        UISystem.DisplayOff(creditsScene);
    }
    private void Update()
    {
        this.redScore.GetComponent<TextMeshProUGUI>().text =
            ScoreModel.Instance.GetPlayer1Score().ToString()+"%";　  //テキストの内容
        this.yellowScore.GetComponent<TextMeshProUGUI>().text = 
            ScoreModel.Instance.GetPlayer2Score().ToString()+"%";　  //テキストの内容         
    }
    void FixedUpdate()
    {
        if(isTimerOn==true)//タイマーがonなら
        {
            timer -= Time.deltaTime;//カウントダウン開始
            if (timer <= 3.0f - timerCon)
            {
                UISystem.MoveToLeft(redScore, 1100, 100);
            }
            if (timer <= 2.6f - timerCon)
            {
                UISystem.MoveToLeft(yellowScore, 1100, 100);
            }
            if (timer <= 2.0f - timerCon)
            {
                if (ScoreModel.Instance.GetPlayer1Score() == ScoreModel.Instance.GetPlayer2Score())
                {
                    TurnSmallAndAppearTMP(draw);
                }
                else
                {
                    TurnSmallAndAppearTMP(winner);
                }
            }
            if (timer <= 1.4f - timerCon)
            {
                if (ScoreModel.Instance.GetPlayer1Score() > ScoreModel.Instance.GetPlayer2Score())
                {
                    TurnSmallAndAppear(winRed);
                }
                if (ScoreModel.Instance.GetPlayer1Score() < ScoreModel.Instance.GetPlayer2Score())//ScoreSystem.Instance.GetPlayer2Score()
                {
                    TurnSmallAndAppear(winYellow);
                }
            }
            if (timer <= 0.5f - timerCon)
            {
                timer = 3f;//タイマーをリセット
                //todo true->can switch scene;false -> cant switch scene until animation is stopped
                isClicked = false;//クリックできる状態になる
                isBlinking = true;//press any buttonwの明滅をon
                isTimerOn = false;//クリックできる状態になる
            }
        }

        //press any buttonの処理
        if (isBlinking == true) 
        {
            UISystem.DisplayOn(pressAnyBtn);
            Blink(pressAnyBtn);
            //if (clickTimes == 1)
            //{
            //    UISystem.DisplayOn(pressAnyBtn02);
            //    Blink(pressAnyBtn02);
            //}
        }

        //scene切り替えのカーテンのコントロラー
        if (isCurtainTurnOn == true)//カーテンのコントロラーのスイッチがon
        {
            CurtainTurnOnAndDoSomethingElse();//カーテンのコントロラーが起動
        }
        if(isCurtainTurnBlack == true)//カーテンが黒くなるスイッチがon
        {
            CurtainTurnBlack();//カーテンが黒くなる
        }     
    }
    void TurnSmallAndAppearTMP(GameObject ui)
    {
        alphaSetTMP += alphachangeTMP;
        UISystem.SetAlphaTMP(ui, alphaSetTMP);
        if (localScaleSetTMP > 1)
        {
            localScaleSetTMP -= localScaleChangeTMP;
            UISystem.SetLocalScale(ui, localScaleSetTMP, localScaleSetTMP, localScaleSetTMP);
        }
    }
    void TurnSmallAndAppear(GameObject ui)
    {
        alphaSet += alphachange;
        UISystem.SetAlpha(ui, alphaSet);
        if (localScaleSet > 1)
        {
            localScaleSet -= localScaleChange;
            UISystem.SetLocalScale(ui, localScaleSet, localScaleSet, localScaleSet);
        }
    }
    private void Blink(GameObject pressBtn)
    {
        blinkTimer += Time.fixedDeltaTime;

        Color newColor = pressBtn.GetComponent<TextMeshProUGUI>().color;

        newColor.a = (math.cos(blinkTimer * blinkSpeed) * 0.3f) + 0.7f;//newColor.aのあたいを0.4~1.0の間繰り返す。

        pressBtn.GetComponent<TextMeshProUGUI>().color = newColor;
    }
    private void CurtainTurnBlack()//カーテンを黒くする処理開始
    {
        sceneSwitchCurtainAlpha += sceneSwitchCurtainSpeed;//カーテンを黒くする
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態
        if (sceneSwitchCurtainAlpha >= 1f)//カーテンが黒になった
        {
            isCurtainTurnOn = true;//カーテンが戻る
            isCurtainTurnBlack = false;//カーテンが黒くなるスイッチがoff
        }
    }
    private void CurtainTurnOnAndDoSomethingElse()//カーテンを戻ると諸処理
    {
        if (clickTimes == 0)//クリックされていない状態なら
        {
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//カーテンを戻る
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態
            if (sceneSwitchCurtainAlpha <= 0f)//カーテンを戻たら
            {
                sceneSwitchCurtainAlpha = 0f;//カーテンのAlphaを固定
                isCurtainTurnOn = false;//カーテンを戻るスイッチoff
            }
        }
        if (clickTimes == 1)//クリックが第一回目済んだばい
        {
            UISystem.DisplayOn(creditsScene);//ゲーム感謝画面を現す
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//カーテンを戻る
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//カーテンの状態
            if (sceneSwitchCurtainAlpha <= 0f)//カーテンを戻たら
            {
                sceneSwitchCurtainAlpha = 0f;//カーテンのAlphaを固定
                sceneTimer += Time.fixedDeltaTime;//タイマーを起動
                if (sceneTimer >= CREDITS_SCENE_TIME)//タイマーが一定の時間を満たす場合
                {
                    sceneTimer = 0f;//タイマーをリセット
                    isClicked = false;//クリックを可能な状態になる
                    isCurtainTurnOn = false;//カーテンを戻るスイッチoff
                    //UISystem.DisplayOn(pressAnyBtn02);//ゲーム感謝画面のpress any buttonを現す
                    isBlinking = true;////press ant buttonの点滅状態をon
                }
            }
        }
    }
    private void OnSwitchScene(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isClicked == false)//クリックできる状態なら
            {
                switch (clickTimes)
                {
                    case 0:
                        isCurtainTurnBlack = true;//カーテンを黒くなる
                        isBlinking = false;//press any buttonの明滅をoff
                        AudioManager.Instance.PlayFX("ClickFX", 0.5f);
                        AudioManager.Instance.StopBGM();
                        clickTimes += 1;
                        isClicked = true;//クリックできない状態になる
                        break;
                    case 1:
                        AudioManager.Instance.PlayFX("ClickFX", 0.5f);
                        AudioManager.Instance.StopBGM();
                        AudioManager.Instance.PlayBGM("TitleBGM", 0.3f);
                        TypeEventSystem.Instance.Send<TitleSceneSwitch>();
                        isClicked = true;//クリックできない状態になる
                        break;
                }             
            }
        }
    }

    private void OnEnable() => _anyKeyAction.Enable();
    private void OnDisable() => _anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

}
