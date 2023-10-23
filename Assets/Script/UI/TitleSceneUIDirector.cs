using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject pressBtn;�@�@�@�@�@�@�@//PRESS ANY BUTTON�̃I�u�W�F�N�g
    public bool isShiningOn = true;          //�u�^���_�ł̃X�C�b�`
    public float blinkSpeed = 0.02f;         //�{�^���̓_�ŕω��̑��x
    public float blinkInterval = 1.5f;       //�{�^���̓_�ňꉝ���̎���
    public InputActionAsset _anyValueAction;

    public GameObject sceneSwitchCurtain;//scene�؂�ւ��̃J�[�e��
    private float sceneSwitchCurtainSpeed;//scene�؂�ւ��̃J�[�e���������Ȃ�X�s�[�h
    private float sceneSwitchCurtainAlpha;//�V�[���؂�ւ��J�[�e���̏����l

    private float blinkTimer = 0f;�@�@�@�@�@//�_�ł̃^�C�}�[
    private InputAction _anyKeyAction;

    public GameObject loadingScene;
    public GameObject LoadingSpider01;
    public GameObject LoadingSpider02;
    private bool isCurtainTurnBlackSwitchOn;
    private bool isCurtainTurnOn;
    private bool isLoadingAnimationOn;
    private bool isSceneSwitchOn;
    private bool isClickOnce;
    private float loadingSceneTimer;//���[�h�V�[���̌p�����Ԃ̃^�C�}�[
    private float loadingSceneTime;//���[�h�V�[���̌p������
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
        sceneSwitchCurtainAlpha = 0f;//�J�[�e���̓����x�����l�i���S�����j
        sceneSwitchCurtainSpeed = 0.05f;//�J�[�e���������Ȃ�X�s�[�h
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏�Ԃ�������
        UISystem.DisplayOff(loadingScene);//���[�h�V�[�����B��
        loadingSpider01x = UISystem.GetPositionX(LoadingSpider01);//���[�h�V�[���̃X�p�C�_�[��x���W���l��
        loadingSpider02x = UISystem.GetPositionX(LoadingSpider02);
        loadingSpider01MoveSpeed = 40f;//���[�h�V�[���̃X�p�C�_�[�̃X�s�[�h���Z�b�g
        loadingSpider02MoveSpeed = 40f;
        UISystem.SetPos(LoadingSpider01, -1200f, 0f);//���[�h�V�[���̃X�p�C�_�[�̈ʒu��������
        UISystem.SetPos(LoadingSpider02, 1200f, 0f);
        loadingSceneTimer = 0f;//���[�h�V�[���̌p�����Ԃ̃^�C�}�[
        loadingSceneTime = 2f;//���[�h�V�[���̌p������
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
                isClickOnce = false;//�N���b�N����Ă��Ȃ���Ԃɖ߂�E�N���b�N��Ԃ����Z�b�g
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
        UISystem.DisplayOn(loadingScene);//���[�h�V�[��������
        sceneSwitchCurtainAlpha -= sceneSwitchCurtainSpeed;//�J�[�e����߂�
        UISystem.SetAlpha(sceneSwitchCurtain, sceneSwitchCurtainAlpha);//�J�[�e���̏��
        if (sceneSwitchCurtainAlpha <=0f)//�J�[�e����߂���
        {
            sceneSwitchCurtainAlpha = 0f;//�J�[�e����Alpha���Œ�
            isCurtainTurnOn = false;//�J�[�e����߂�X�C�b�`off
            isLoadingAnimationOn = true;//���[�h�V�[���̃A�j���V����on�I
        }
    }
    private void LoadingAnimationOn()//���[�h�V�[���̃A�j���V����
    {
        loadingSceneTimer += Time.fixedDeltaTime;//���[�h�V�[���̌p�����Ԃ��J�E���g���߂�
        UISystem.MoveToRight(LoadingSpider01, loadingSpider01x, loadingSpider01MoveSpeed);//�X�p�C�_�[ui�̍s������
        UISystem.MoveToLeft(LoadingSpider02, loadingSpider02x, loadingSpider02MoveSpeed);//�X�p�C�_�[ui�̍s������
        if (loadingSceneTimer > loadingSceneTime)//���[�h�V�[���̌p�����Ԃɓ��B
        {
            loadingSceneTimer = 0;//�^�C�}�[�����Z�b�g
            isLoadingAnimationOn = false;//���[�h�V�[���̃A�j���V������off
            isSceneSwitchOn = true;//�V�[���؂�ւ��̃X�C�b�`on!
            isCurtainTurnBlackSwitchOn = true;//�J�[�e������������X�C�b�`on!
        }
    }

    private void OnEnable()=>_anyKeyAction.Enable();
    private void OnDisable()=>_anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

    private void OnSwitchScene(InputAction.CallbackContext context)
    { 
        if (context.performed)
        {
            if(isClickOnce==false)//�N���b�N����Ă��Ȃ���ԂȂ�
            {
                AudioManager.Instance.PlayFX("ClickFX",0.5f);//�N���b�N�̉���炷
                isCurtainTurnBlackSwitchOn = true;//�J�[�e������������X�C�b�`on!
                isClickOnce = true;//�N���b�N�ς݂̏�Ԃɂ���
            }
        }
    }
}
