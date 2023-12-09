using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Kit;
using System.Collections;

public class ScoreUIDirector : MonoBehaviour
{
    public GameObject timeUI;

    float timerSetting = Global.SET_GAME_TIME;

    public Image TimeBarFront;
    public Image DushBar_Yellow_Left;
    public Image DushBar_Yellow_Right;
    private bool[] isBoostStart = new bool[2];
    private bool[] isBoostEnd = new bool[2];
    private float boostTimer0;
    private float boostTimer1;

    [Header("Prefab")]
    public CountDownCtrler startCountDownCtrler;
    public CountDownCtrler finishCountDownCtrler;

    void Start()
    {
        Time.timeScale = 0.0f;
        startCountDownCtrler.StartCountDown(() => Time.timeScale = 1.0f);

        for (int i = 0; i < 2; i++)//�u�[�X�g�̃X�C�b�`��������
        {
            isBoostStart[i] = false;
            isBoostEnd[i] = false;
        }
        boostTimer0 = Global.BOOST_DURATION_TIME;//�^�C�}�[��������
        boostTimer1 = Global.BOOST_DURATION_TIME;


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
            BoostStart(e.Nomber);
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
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

        timerSetting -= Time.deltaTime;

        TimeBarFront.fillAmount = timerSetting / Global.SET_GAME_TIME;

        if (isBoostStart[0])
        {
            boostTimer0 -= Time.deltaTime;
            DushBar_Yellow_Left.fillAmount = boostTimer0 / Global.BOOST_DURATION_TIME;//�u�[�X�g�̃`���[�W�o�[������
            if (DushBar_Yellow_Left.fillAmount <= 0.0f)
            {
                Invoke("BoostBarFillBack1", Global.BOOST_COOLDOWN_TIME);//�uGlobal.BOOST_COOLDOWN_TIME�v�b�ナ�`���[�W
                boostTimer0 = Global.BOOST_DURATION_TIME;//�^�C�}�[���Z�b�g
                isBoostStart[0] = false;
            }
        }
        if (isBoostStart[1])
        {
            boostTimer1 -= Time.deltaTime;
            DushBar_Yellow_Right.fillAmount = boostTimer1 / Global.BOOST_DURATION_TIME;
            if (DushBar_Yellow_Right.fillAmount <= 0.0f)
            {
                Invoke("BoostBarFillBack2", Global.BOOST_COOLDOWN_TIME);
                boostTimer1 = Global.BOOST_DURATION_TIME;
                isBoostStart[1] = false;
            }
        }

        if (isBoostEnd[0])
        {
            if (DushBar_Yellow_Left.fillAmount < 1)
            {
                DushBar_Yellow_Left.fillAmount += 0.05f;
            }
            else
            {
                isBoostEnd[0] = false;
            }
        }
        if (isBoostEnd[1])
        {
            if (DushBar_Yellow_Right.fillAmount < 1)
            {
                DushBar_Yellow_Right.fillAmount += 0.05f;
            }
            else
            {
                isBoostEnd[1] = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {

            BoostStart boostStart1 = new BoostStart();
            boostStart1.Nomber = 0;

            TypeEventSystem.Instance.Send<BoostStart>(boostStart1);

        }
        if (Input.GetKeyDown(KeyCode.B))
        {

            BoostStart boostStart2 = new BoostStart();
            boostStart2.Nomber = 1;

            TypeEventSystem.Instance.Send<BoostStart>(boostStart2);

        }

        //�e�X�g�p�]�]�]�]�]�]�]�]
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TypeEventSystem.Instance.Send<Player2RespawnCntBegin>();
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    TypeEventSystem.Instance.Send<Player1RespawnCntBegin>();
        //}
    }

    void BoostStart(int nomber)
    {
        switch (nomber)
        {
            case 0:
                if (isBoostStart[0] != true)
                {
                    isBoostStart[0] = true;
                }
                break;
            case 1:
                if (isBoostStart[1] != true)
                {
                    isBoostStart[1] = true;
                }
                break;
        }
    }
    void BoostBarFillBack1()
    {
        isBoostEnd[0] = true;
        CancelInvoke("BoostBarFillBack1");
    }
    void BoostBarFillBack2()
    {
        isBoostEnd[1] = true;
        CancelInvoke("BoostBarFillBack2");
    }
}
