using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class TitleSceneUIDirector : MonoBehaviour
{
    //press any button�֘A
    public GameObject pressBtn;�@�@�@�@�@�@�@//PRESS ANY BUTTON�̃I�u�W�F�N�g
    public GameObject pressBtn02;
    public float blinkSpeed;         //�{�^���̓_�ŕω��̑��x
    private float blinkTimer;�@�@�@�@�@//�_�ł̃^�C�}�[
    //�V�[���؂�ւ��֘A
    public InputActionAsset _anyValueAction;
    private InputAction _anyKeyAction;
    public GameObject sceneSwitchCurtain;//scene�؂�ւ��̃J�[�e��
    private float sceneSwitchCurtainSpeed;//scene�؂�ւ��̃J�[�e���������Ȃ�X�s�[�h
    private float sceneSwitchCurtainAlpha;//�V�[���؂�ւ��J�[�e���̏����l

    public GameObject InstructionScene;
    public GameObject loadingScene;
    public GameObject LoadingSpider01;
    public GameObject LoadingSpider02;
    private bool isShiningOn = true;          //press any button�_�ł̃X�C�b�`
    private bool isCurtainTurnBlackSwitchOn;
    private bool isCurtainTurnOn;
    private bool isLoadingAnimationOn;
    private bool isSceneSwitchOn;
    private bool isClicked;
    private float sceneTimer;//���[�h�V�[���̌p�����Ԃ̃^�C�}�[
    private float loadingSceneTime;//���[�h�V�[���̌p������
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
        sceneSwitchCurtainAlpha = 0f;//�J�[�e���̓����x�����l�i���S�����j
        sceneSwitchCurtainSpeed = 0.05f;//�J�[�e���������Ȃ�X�s�[�h
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏�Ԃ�������
        UISystem.DisplayOff(InstructionScene);//�Q�[��������@�Љ��ʂ��B��
        UISystem.DisplayOff(loadingScene);//���[�h�V�[�����B��
        loadingSpider01x = UISystem.GetPositionX(LoadingSpider01);//���[�h�V�[���̃X�p�C�_�[��x���W���l��
        loadingSpider02x = UISystem.GetPositionX(LoadingSpider02);
        loadingSpider01MoveSpeed = 40f;//���[�h�V�[���̃X�p�C�_�[�̃X�s�[�h���Z�b�g
        loadingSpider02MoveSpeed = 40f;
        UISystem.SetPos(LoadingSpider01, -1200f, 0f);//���[�h�V�[���̃X�p�C�_�[�̈ʒu��������
        UISystem.SetPos(LoadingSpider02, 1200f, 0f);
        sceneTimer = 0f;//���[�h�V�[���̌p�����Ԃ̃^�C�}�[
        loadingSceneTime = 2f;//���[�h�V�[���̌p������
        INTSTRUCTON_SCENE_TIME = Global.INTSTRUCTON_SCENE_TIME;//�Q�[��������@�Љ��� PRESS�����̎���
        clickTimes = 0;//�{�^���̉�����ĉ񐔃R���g�����[

        //press any button�֘A
        UISystem.DisplayOff(pressBtn02); //�Q�[��������@�Љ��ʂ�press any button���B��
        blinkTimer = 0f;//�_�ł̃^�C�}�[
        blinkSpeed = 2.2f;//�{�^���̓_�ŕω��̑��x
    }
    private void FixedUpdate()
    {
        if (isShiningOn==true)//press any button�_�ł̃X�C�b�`��on�Ȃ�
        {
            Blink(pressBtn);//press any button��_��
            Blink(pressBtn02);
        }

        if (isCurtainTurnBlackSwitchOn == true)//�J�[�e������������X�C�b�`on!
        {
            CurtainTurnBlack();//�J�[�e�����������鏈���J�n
        }
        if (isCurtainTurnOn == true)//�J�[�e����߂�X�C�b�`on�I
        {
            CurtainTurnOnAndLoadingSceneOn();//�J�[�e����߂�ƃ��[�h�V�[������������
        }
        if (isLoadingAnimationOn == true)//���[�h�V�[���̃A�j���V����on�I
        {
            LoadingAnimationOn();//���[�h�V�[���̃A�j���V����
        }
    }
    private void CurtainTurnBlack()//�J�[�e�����������鏈���J�n
    {
        sceneSwitchCurtainAlpha += sceneSwitchCurtainSpeed;//�J�[�e������������
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏��
        if (sceneSwitchCurtainAlpha >= 1f)//�J�[�e�������ɂȂ���
        {
            isCurtainTurnBlackSwitchOn = false;////�J�[�e������������X�C�b�`off!
            if (isSceneSwitchOn == true)//�V�[���؂�ւ��̃X�C�b�`on!
            {
                isSceneSwitchOn = false;//�V�[���؂�ւ��̃X�C�b�`��off�ɂ���
                isClicked = false;//�N���b�N����Ă��Ȃ���Ԃɖ߂�E�N���b�N��Ԃ����Z�b�g
                clickTimes = 0;//�N���b�N�񐔂̃R���g�����[�����Z�b�g
                TypeEventSystem.Instance.Send<GamingSceneSwitch>();//�V�[����؂�ւ�
            }
            else//�V�[���؂�ւ��̃X�C�b�`��off�̏�ԂȂ�
            {
                isCurtainTurnOn = true;//�J�[�e����߂�X�C�b�`on�I
            }
        }
    }
    private void CurtainTurnOnAndLoadingSceneOn()//�J�[�e����߂�ƃ��[�h�V�[������������
    {
        if (clickTimes == 1)//�N���b�N������ڂ̂΂�
        {
            UISystem.DisplayOn(InstructionScene);//�Q�[��������@�Љ��ʂ�����
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//�J�[�e����߂�
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏��
            if (sceneSwitchCurtainAlpha <= 0f)//�J�[�e����߂���
            {
                sceneSwitchCurtainAlpha = 0f;//�J�[�e����Alpha���Œ�
                sceneTimer += Time.fixedDeltaTime;//�^�C�}�[���N��
                if(sceneTimer >= INTSTRUCTON_SCENE_TIME)//�^�C�}�[�����̎��Ԃ𖞂����ꍇ
                {
                    sceneTimer = 0f;//�^�C�}�[�����Z�b�g
                    isClicked = false;//�N���b�N���\�ȏ�ԂɂȂ�
                    isCurtainTurnOn = false;//�J�[�e����߂�X�C�b�`off
                    UISystem.DisplayOn(pressBtn02);//�Q�[��������@�Љ��ʂ�press any button������
                    isShiningOn = true;////press ant button�̓_�ŏ�Ԃ�on
                }
            }
        }
        if (clickTimes == 2)//�N���b�N����Q��ڂ̂΂�
        {
            UISystem.DisplayOn(loadingScene);//���[�h�V�[��������
            sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//�J�[�e����߂�
            UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏��
            if (sceneSwitchCurtainAlpha <= 0f)//�J�[�e����߂���
            {
                sceneSwitchCurtainAlpha = 0f;//�J�[�e����Alpha���Œ�
                isCurtainTurnOn = false;//�J�[�e����߂�X�C�b�`off
                isLoadingAnimationOn = true;//���[�h�V�[���̃A�j���V����on�I
            }
        } 
    }
    private void LoadingAnimationOn()//���[�h�V�[���̃A�j���V����
    {
        sceneTimer += Time.fixedDeltaTime;//���[�h�V�[���̌p�����Ԃ��J�E���g���߂�
        UISystem.MoveToRight(LoadingSpider01, loadingSpider01x, loadingSpider01MoveSpeed);//�X�p�C�_�[ui�̍s������
        UISystem.MoveToLeft(LoadingSpider02, loadingSpider02x, loadingSpider02MoveSpeed);//�X�p�C�_�[ui�̍s������
        if (sceneTimer > loadingSceneTime)//���[�h�V�[���̌p�����Ԃɓ��B
        {
            sceneTimer = 0;//�^�C�}�[�����Z�b�g
            isLoadingAnimationOn = false;//���[�h�V�[���̃A�j���V������off
            isSceneSwitchOn = true;//�V�[���؂�ւ��̃X�C�b�`on!
            isCurtainTurnBlackSwitchOn = true;//�J�[�e������������X�C�b�`on!
        }
    }
    private void Blink(GameObject pressBtn)
    {
        blinkTimer += Time.fixedDeltaTime;

        Color newColor = pressBtn.GetComponent<TextMeshProUGUI>().color;

        newColor.a = (math.cos(blinkTimer * blinkSpeed) *0.2f)+0.8f;//newColor.a�̂�������0.6~1.0�̊ԌJ��Ԃ��B

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
                if (isClicked == false)//�N���b�N����Ă��Ȃ���ԂȂ�
                {
                    switch (clickTimes)
                    {
                        case 0:
                            clickTimes += 1;
                            AudioManager.Instance.PlayFX("ClickFX", 0.5f);//�N���b�N�̉���炷
                            isCurtainTurnBlackSwitchOn = true;//�J�[�e������������X�C�b�`on!
                            isClicked = true;//�N���b�N�ς݂̏�Ԃɂ���
                            isShiningOn = false;//press ant button�̓_�ŏ�Ԃ�off
                            break;
                        case 1:
                            clickTimes += 1;
                            AudioManager.Instance.PlayFX("ClickFX", 0.5f);//�N���b�N�̉���炷
                            isCurtainTurnBlackSwitchOn = true;//�J�[�e������������X�C�b�`on!
                            isClicked = true;//�N���b�N�ς݂̏�Ԃɂ���
                            break;
                    }
                }

            }
        }
    }
}
