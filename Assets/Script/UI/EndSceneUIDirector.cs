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
    public float blinkSpeed;       //�{�^���̓_�ŕω��̑��x
    //public float blinkInterval = 1.5f;       //�{�^���̓_�ňꉝ���̎���
    private float blinkTimer;
    private bool isBlinking;

    public GameObject sceneSwitchCurtain;//scene�؂�ւ��̃J�[�e��
    private float sceneSwitchCurtainSpeed;//scene�؂�ւ��̃J�[�e���������Ȃ�X�s�[�h
    private float sceneSwitchCurtainAlpha;//�V�[���؂�ւ��J�[�e���̏����l
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

        //press any button�_�Ŋ֘A--------------------
        blinkTimer = 0f;
        blinkSpeed = 3.2f;       //�{�^���̓_�ŕω��̑��x

        sceneSwitchCurtainAlpha = 1f;//scene�؂�ւ��̃J�[�e���̓����x�����l�i���S�����j
        sceneSwitchCurtainSpeed = 0.05f;//scene�؂�ւ��̃J�[�e���𓧖��ɂȂ�X�s�[�h
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
            ScoreModel.Instance.GetPlayer1Score().ToString()+"%";�@  //�e�L�X�g�̓��e
        this.yellowScore.GetComponent<TextMeshProUGUI>().text = 
            ScoreModel.Instance.GetPlayer2Score().ToString()+"%";�@  //�e�L�X�g�̓��e         
    }
    void FixedUpdate()
    {
        if(isTimerOn==true)//�^�C�}�[��on�Ȃ�
        {
            timer -= Time.deltaTime;//�J�E���g�_�E���J�n
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
                timer = 3f;//�^�C�}�[�����Z�b�g
                //todo true->can switch scene;false -> cant switch scene until animation is stopped
                isClicked = false;//�N���b�N�ł����ԂɂȂ�
                isBlinking = true;//press any buttonw�̖��ł�on
                isTimerOn = false;//�N���b�N�ł����ԂɂȂ�
            }
        }

        //press any button�̏���
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

        //scene�؂�ւ��̃J�[�e���̃R���g�����[
        if (isCurtainTurnOn == true)//�J�[�e���̃R���g�����[�̃X�C�b�`��on
        {
            CurtainTurnOnAndDoSomethingElse();//�J�[�e���̃R���g�����[���N��
        }
        if(isCurtainTurnBlack == true)//�J�[�e���������Ȃ�X�C�b�`��on
        {
            CurtainTurnBlack();//�J�[�e���������Ȃ�
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

        newColor.a = (math.cos(blinkTimer * blinkSpeed) * 0.3f) + 0.7f;//newColor.a�̂�������0.4~1.0�̊ԌJ��Ԃ��B

        pressBtn.GetComponent<TextMeshProUGUI>().color = newColor;
    }
    private void CurtainTurnBlack()//�J�[�e�����������鏈���J�n
    {
        sceneSwitchCurtainAlpha += sceneSwitchCurtainSpeed;//�J�[�e������������
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏��
        if (sceneSwitchCurtainAlpha >= 1f)//�J�[�e�������ɂȂ���
        {
            isCurtainTurnOn = true;//�J�[�e�����߂�
            isCurtainTurnBlack = false;//�J�[�e���������Ȃ�X�C�b�`��off
        }
    }
    private void CurtainTurnOnAndDoSomethingElse()//�J�[�e����߂�Ə�����
    {
        if (clickTimes == 0)//�N���b�N����Ă��Ȃ���ԂȂ�
        {
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//�J�[�e����߂�
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏��
            if (sceneSwitchCurtainAlpha <= 0f)//�J�[�e����߂���
            {
                sceneSwitchCurtainAlpha = 0f;//�J�[�e����Alpha���Œ�
                isCurtainTurnOn = false;//�J�[�e����߂�X�C�b�`off
            }
        }
        if (clickTimes == 1)//�N���b�N������ڍς񂾂΂�
        {
            UISystem.DisplayOn(creditsScene);//�Q�[�����Ӊ�ʂ�����
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//�J�[�e����߂�
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏��
            if (sceneSwitchCurtainAlpha <= 0f)//�J�[�e����߂���
            {
                sceneSwitchCurtainAlpha = 0f;//�J�[�e����Alpha���Œ�
                sceneTimer += Time.fixedDeltaTime;//�^�C�}�[���N��
                if (sceneTimer >= CREDITS_SCENE_TIME)//�^�C�}�[�����̎��Ԃ𖞂����ꍇ
                {
                    sceneTimer = 0f;//�^�C�}�[�����Z�b�g
                    isClicked = false;//�N���b�N���\�ȏ�ԂɂȂ�
                    isCurtainTurnOn = false;//�J�[�e����߂�X�C�b�`off
                    //UISystem.DisplayOn(pressAnyBtn02);//�Q�[�����Ӊ�ʂ�press any button������
                    isBlinking = true;////press ant button�̓_�ŏ�Ԃ�on
                }
            }
        }
    }
    private void OnSwitchScene(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isClicked == false)//�N���b�N�ł����ԂȂ�
            {
                switch (clickTimes)
                {
                    case 0:
                        isCurtainTurnBlack = true;//�J�[�e���������Ȃ�
                        isBlinking = false;//press any button�̖��ł�off
                        AudioManager.Instance.PlayFX("ClickFX", 0.5f);
                        AudioManager.Instance.StopBGM();
                        clickTimes += 1;
                        isClicked = true;//�N���b�N�ł��Ȃ���ԂɂȂ�
                        break;
                    case 1:
                        AudioManager.Instance.PlayFX("ClickFX", 0.5f);
                        AudioManager.Instance.StopBGM();
                        AudioManager.Instance.PlayBGM("TitleBGM", 0.3f);
                        TypeEventSystem.Instance.Send<TitleSceneSwitch>();
                        isClicked = true;//�N���b�N�ł��Ȃ���ԂɂȂ�
                        break;
                }             
            }
        }
    }

    private void OnEnable() => _anyKeyAction.Enable();
    private void OnDisable() => _anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

}
