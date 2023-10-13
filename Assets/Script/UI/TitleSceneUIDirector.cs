using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject pressBtn;
    public GameObject sceneSwitchEffect;
    public bool isShiningOn = true;         //�u�^���_�ł̃X�C�b�`
    public float blinkSpeed = 0.02f;       //�{�^���̓_�ŕω��̑��x
    public float blinkInterval = 1.5f;       //�{�^���̓_�ňꉝ���̎���
    public InputActionAsset _anyValueAction;

    private float blinkTimer = 0f;
    private InputAction _anyKeyAction;
    private float sceneSwitchEffectAlpha;//�V�[���؂�ւ��G�t�F�N�g�̏����l
    private float sceneSwitchEffectSpeed;

    //float speed = 1f;�@�@�@�@�@�@�@�@�@�@//�e�X�g�p

    private void Awake()
    {
        ScoreSystem.Instance.ResetScore();
        _anyKeyAction = _anyValueAction.FindActionMap("AnyKey").FindAction("AnyKey");
        _anyKeyAction.performed += OnSwitchScene;
    }
    private void Start()
    {
        sceneSwitchEffectAlpha = 0f;
        sceneSwitchEffectSpeed = 0.2f;
        UISystem.SetAlpha(sceneSwitchEffect, sceneSwitchEffectAlpha);
    }
    private void FixedUpdate()
    {
        if(isShiningOn==true)
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
    }

    private void OnEnable()=>_anyKeyAction.Enable();
    private void OnDisable()=>_anyKeyAction.Disable();
    private void OnDestroy() => _anyKeyAction.performed -= OnSwitchScene;

    private void OnSwitchScene(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            sceneSwitchEffectAlpha += sceneSwitchEffectSpeed;
            UISystem.SetAlpha(sceneSwitchEffect, sceneSwitchEffectAlpha);                            //?????????????
            if (sceneSwitchEffectAlpha>=1f)
            {
                TypeEventSystem.Instance.Send<MenuSceneSwitch>();
            }
        }
    }


}
