using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Kit;
using System.Collections;

public class ScoreUIDirector : MonoBehaviour
{
    //public GameObject timeUI;

    float timerSetting = Global.SET_GAME_TIME;

    public Image TimeBarFront;
    //TODO changed by Mai
    //�X�y���~�X Dush => Dash
    public Image DashBar_Yellow_Left;
    public Image DashBar_Yellow_Right;

    private Image[] boostBar = new Image[2];
    private bool[] isBoostStart = new bool[2];
    private float[] boostCooldownTimer = new float[2];

    [Header("Prefab")]
    public CountDownCtrler startCountDownCtrler;
    public CountDownCtrler finishCountDownCtrler;

    void Start()
    {
        Time.timeScale = 0.0f;
        startCountDownCtrler.StartCountDown(() => Time.timeScale = 1.0f);

        for (int i = 0; i < 2; i++)
        {
            //TODO changed by Mai
            //isBoostStart�͏��������Ă��Ȃ� 
            isBoostStart[i] = false;
            boostCooldownTimer[i]= Global.BOOST_COOLDOWN_TIME;//�u�[�X�g�o�[�̃N�[���_�E����������
        }
        //TODO changed by Mai
        //�X�y���~�X Dush => Dash
        boostBar[0] = DashBar_Yellow_Left;
        boostBar[1] = DashBar_Yellow_Right;

        //�^�C�}�[���Z�b�g�iGlobal.SET_GAME_TIME - 9f�j�A�^�C�}�[���I���ƁA�J�E���g�_�E�����n�߂�B
        ActionKit.Delay(Global.SET_GAME_TIME - 9f, () =>
        {
            finishCountDownCtrler.StartCountDown(() =>
            {
                AudioManager.Instance.StopBGM();
                TypeEventSystem.Instance.Send<GameOver>();
            });
        }).Start(this);

        TypeEventSystem.Instance.Register<BoostStart>(e =>
        {
            //TODO changed by Mai
            //�X�y���~�X nomber => number
            BoostStart(e.Number);
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    private void FixedUpdate()
    {
        BoostBarUpdate();
    }
    void Update()
    {
        #region �Վ��R�[�h
        //if (timer.GetTime() <= 9.0f)
        //{

        //    startCountDownCtrler.StartCountDown(() => 
        //    { 
        //        AudioManager.Instance.StopBGM(); 
        //        TypeEventSystem.Instance.Send<GameOver>(); 
        //    });
        //}
        //if(fightAnimControl.isPlayDone == true)
        //{
        //    AudioManager.Instance.StopBGM();
        //    TypeEventSystem.Instance.Send<GameOver>();                 //GameOver���߂𔭑��AEndScene�֐؂�ւ�
        //}
        #endregion

        timerSetting -= Time.deltaTime;

        TimeBarFront.fillAmount = timerSetting / Global.SET_GAME_TIME;

        #region Boost�e�X�g�p
        //if (Input.GetKeyDown(KeyCode.V))
        //{

        //    BoostStart boostStart1 = new BoostStart();
        //    boostStart1.Nomber = 0;

        //    TypeEventSystem.Instance.Send<BoostStart>(boostStart1);

        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{

        //    BoostStart boostStart2 = new BoostStart();
        //    boostStart2.Nomber = 1;

        //    TypeEventSystem.Instance.Send<BoostStart>(boostStart2);

        //}
        #endregion
    }

    //TODO changed by Mai
    //�X�y���~�X nomber => number
    void BoostStart(int number)
    {
        //TODO �^��_
        //switch���g���Ӗ��́H
        switch (number)
        {
            case 1:
                if (isBoostStart[0] != true)
                {
                    isBoostStart[0] = true;
                }
                break;
            case 2:
                if (isBoostStart[1] != true)
                {
                    isBoostStart[1] = true;
                }
                break;
        }
    }

    //TODO �Ӗ��s���ȏ���
    //�o�[�̃`���[�W�\�����Ӗ��s��
    private void BoostBarUpdate()
    {
        for (int i = 0; i < 2; ++i) 
        {
            if (isBoostStart[i])
            {
                boostBar[i].fillAmount -= Time.fixedDeltaTime / Global.BOOST_DURATION_TIME;//�u�[�X�g�̃`���[�W�o�[������
                boostCooldownTimer[i] -= Time.fixedDeltaTime;//�N�[���_�E���^�C�}�[����
                if (boostCooldownTimer[i] <= (1.0f / Global.BOOST_BAR_CHARGING_SPEED) * Time.fixedDeltaTime) //���`���[�W���Ԃ̒��O��
                {
                    isBoostStart[i] = false;
                }
            }
            else
            {
                boostCooldownTimer[i] = Global.BOOST_COOLDOWN_TIME;//�N�[���_�E���^�C�}�[���Z�b�g
                boostBar[i].fillAmount += Global.BOOST_BAR_CHARGING_SPEED;//�u�[�X�g�̃`���[�W�o�[�����`���[�W
            }
        }
    }
}
