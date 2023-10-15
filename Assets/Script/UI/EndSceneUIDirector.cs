using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndSceneUIDirector : MonoBehaviour
{
    public GameObject redScore;
    public GameObject yellowScore;
    public GameObject winner;
    public GameObject draw;
    public GameObject winYellow;
    public GameObject winRed;
    public GameObject pressAnyBtn;

    float timer;
    float timerCon;

    float alphaSetTMP;
    float alphachangeTMP;
    float localScaleSetTMP;
    float localScaleChangeTMP;
    float alphaSet;
    float alphachange;
    float localScaleSet;
    float localScaleChange;

    //Blink Initialization--------------------
    public float blinkSpeed = 0.02f;       //�{�^���̓_�ŕω��̑��x
    public float blinkInterval = 1.5f;       //�{�^���̓_�ňꉝ���̎���
    float blinkTimer = 0f;

    public GameObject sceneSwitchCurtain;//scene�؂�ւ��̃J�[�e��
    private float sceneSwitchCurtainSpeed;//scene�؂�ւ��̃J�[�e���������Ȃ�X�s�[�h
    private float sceneSwitchCurtainAlpha;//�V�[���؂�ւ��J�[�e���̏����l
    bool isCurtainTurnBlack;

    private InputAction _anyKeyAction;
    public InputActionAsset _anyValueAction;
    [SerializeField]
    private bool _isAnimationStopped;

    private void Awake()
    {
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
        _anyKeyAction.performed += OnSwitchScene;
        _isAnimationStopped = false;
    }
    private void Start()
    {
        timer = 3f;
        timerCon = 0.6f;

        sceneSwitchCurtainAlpha = 1f;//scene�؂�ւ��̃J�[�e���̓����x�����l�i���S�����j
        sceneSwitchCurtainSpeed = 0.1f;//scene�؂�ւ��̃J�[�e���𓧖��ɂȂ�X�s�[�h
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

        //UISystem.SetLocalScale(pressAnyBtn, 10f, 10f, 10f);
        //UISystem.SetAlphaTMP(pressAnyBtn, alphaSet);

        //Debug.Log(redScore.transform.position);


    }
    private void Update()
    {
        this.redScore.GetComponent<TextMeshProUGUI>().text =
            "RED SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();�@  //�e�L�X�g�̓��e
        this.yellowScore.GetComponent<TextMeshProUGUI>().text = 
            "YELLOW SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();�@  //�e�L�X�g�̓��e 
        
    }
    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if(timer <= 3.0f-timerCon)
        {
            UISystem.MoveToLeft(redScore, 900, 100);

        }
        if (timer <= 2.6f - timerCon)
        {
            UISystem.MoveToLeft(yellowScore, 900, 100);
        }
        if (timer <= 2.0f - timerCon)
        {
            if (ScoreSystem.Instance.GetPlayer1Score() == ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppearTMP(draw);
            }
            else //if(ScoreSystem.Instance.GetPlayer1Score() >= ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppearTMP(winner);
                
            }      
        }
        if (timer <= 1.4f - timerCon)
        {
            if(ScoreSystem.Instance.GetPlayer1Score() > ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppear(winRed);
            }
            if (ScoreSystem.Instance.GetPlayer1Score() < ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppear(winYellow);
            }         
        }
        if (timer <= 0.5f - timerCon) 
        {
            UISystem.DisplayOn(pressAnyBtn);
            Blink();
            //todo true->can switch scene;false -> cant switch scene until animation is stopped
            _isAnimationStopped = true;
        }

        //scene�؂�ւ��̃J�[�e���̃R���g�����[
        if (isCurtainTurnBlack == false)
        {
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);
        }
        else
        {
            CurtainTurnBlackAndSceneSwitch();
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
    private void Blink()
    {
        blinkTimer += Time.fixedDeltaTime;

        Color newColor = pressAnyBtn.GetComponent<TextMeshProUGUI>().color;

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
            }
        }
        pressAnyBtn.GetComponent<TextMeshProUGUI>().color = newColor;
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

    private void OnSwitchScene(InputAction.CallbackContext context)
    {
        if (_isAnimationStopped)
        {
            
            if (context.performed)
            {
                AudioManager.Instance.PlayFX("ClickFX", 0.5f);
                AudioManager.Instance.StopBGM();
                AudioManager.Instance.PlayBGM("TitleBGM", 0.3f);
                TypeEventSystem.Instance.Send<TitleSceneSwitch>();
                
            }
        }
    }

    private void OnEnable() => _anyKeyAction.Enable();
    private void OnDisable() => _anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

}
