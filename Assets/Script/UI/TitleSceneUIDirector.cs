using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject pressBtn;
    public bool isShiningOn = true;         //�u�^���_�ł̃X�C�b�`
    public float blinkSpeed = 0.02f;       //�{�^���̓_�ŕω��̑��x
    public float blinkInterval = 1.5f;       //�{�^���̓_�ňꉝ���̎���
    public InputActionAsset _anyValueAction;

    public GameObject sceneSwitchCurtain;//scene�؂�ւ��̃J�[�e��
    private float sceneSwitchCurtainSpeed;//scene�؂�ւ��̃J�[�e���������Ȃ�X�s�[�h
    private float sceneSwitchCurtainAlpha;//�V�[���؂�ւ��J�[�e���̏����l

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
        sceneSwitchCurtainAlpha = 0f;//scene�؂�ւ��̃J�[�e���̓����x�����l�i���S�����j
        sceneSwitchCurtainSpeed = 0.1f;//scene�؂�ւ��̃J�[�e���������Ȃ�X�s�[�h
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
