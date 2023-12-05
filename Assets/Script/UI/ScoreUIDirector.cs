using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Kit;

public class ScoreUIDirector : MonoBehaviour
{
    //public GameObject p1ScoreUI;
    //public GameObject p2ScoreUI;

    //public GameObject p1RespawnUI;
    //public GameObject p2RespawnUI;

    float player1Timer;
    float player2Timer;

    //public GameObject redSpiderImageGray;
    //public GameObject yellowSpiderImageGray;

    public GameObject timeUI;

    bool isPlayer1Respawn;
    bool isPlayer2Respawn;

    float timerSetting = Global.SET_GAME_TIME;

    public Image TimeBarFront;

    bool aaa;

    [Header("Prefab")]
    public CountDownCtrler startCountDownCtrler;
    public CountDownCtrler finishCountDownCtrler;

    void Start()
    {
        Time.timeScale = 0.0f;
        startCountDownCtrler.StartCountDown(() => Time.timeScale = 1.0f);

        player1Timer = Global.RESPAWN_TIME;//�����̎��Ԃ�ݒ�
        player2Timer = Global.RESPAWN_TIME;

        //�^�C�}�[���Z�b�g�iGlobal.SET_GAME_TIME - 9f�j�A�^�C�}�[���I���ƁA�J�E���g�_�E�����n�߂�B
        ActionKit.Delay(Global.SET_GAME_TIME - 9f, () =>
        {
            finishCountDownCtrler.StartCountDown(() =>
            {
                AudioManager.Instance.StopBGM();
                TypeEventSystem.Instance.Send<GameOver>();
            });
        }).Start(this);

        //timer.SetTimer(Global.SET_GAME_TIME - 9f, () =>
        //{
            
        //});

        //UISystem.DisplayOff(p1RespawnUI);//�����̃J�E���g�_�E��UI���B��
        //UISystem.DisplayOff(p2RespawnUI);

        //UISystem.DisplayOff(redSpiderImageGray);//�D�F�̃X�p�C�_�[�A�C�R�����B��
        //UISystem.DisplayOff(yellowSpiderImageGray) ;
        
        TypeEventSystem.Instance.Register<Player1RespawnCntBegin>(e => { Player1RespawnCntBegin(); });  //�v���C���[�P�����񂾎��A�J�E���g�_�E�����J�n�B
        TypeEventSystem.Instance.Register<Player2RespawnCntBegin>(e => { Player2RespawnCntBegin(); });�@//�v���C���[�Q�����񂾎��A�J�E���g�_�E�����J�n�B
        //TypeEventSystem.Instance.Register<Player1RespawnCntEnd>(e =>   { Player1RespawnCntEnd(); });�@�@//�v���C���[�P���������鎞�A�J�E���g�_�E�������Z�b�g�B
        //TypeEventSystem.Instance.Register<Player2RespawnCntEnd>(e =>   { Player2RespawnCntEnd(); });�@�@//�v���C���[�Q���������鎞�A�J�E���g�_�E�������Z�b�g�B
    }

    // Update is called once per frame
    void Update()
    {
        //p1ScoreUI.GetComponent<TextMeshProUGUI>().text =
        //    ScoreSystem.Instance.GetPlayer1Score().ToString();�@  //�e�L�X�g�̓��e
        //p2ScoreUI.GetComponent<TextMeshProUGUI>().text =
        //    ScoreSystem.Instance.GetPlayer2Score().ToString();�@  //�e�L�X�g�̓��e

        if (isPlayer1Respawn)
        {
            player1Timer -= Time.deltaTime;                                                    //�J�E���g�_�E�����J�n
            //p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F1");  //�J�E���g�_�E���̃e�L�X�g
            if (player1Timer <= 0)//�J�E���g�_�E�����I����
            {
                Player1RespawnCntEnd();//�J�E���g�_�E�������Z�b�g�Ȃ�
            }
        }
        if (isPlayer2Respawn)
        {
            player2Timer -= Time.deltaTime;                                                    //�J�E���g�_�E�����J�n
            //p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F1");    //�J�E���g�_�E���̃e�L�X�g
            if(player2Timer <= 0)//�J�E���g�_�E�����I����
            {
                Player2RespawnCntEnd();//�J�E���g�_�E�������Z�b�g�Ȃ�
            }
        }

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

        timerSetting-= Time.deltaTime;


        TimeBarFront.fillAmount = timerSetting/ Global.SET_GAME_TIME;

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
    void Player1RespawnCntBegin()
    {
        isPlayer1Respawn = true;�@�@�@�@�@�@�@�@�@�@�@ //�����̏�����on!
        //UISystem.DisplayOn(p1RespawnUI);         �@�@�@//�����̃J�E���g�_�E��UI������
        //UISystem.DisplayOn(redSpiderImageGray);        //�X�p�C�_�[�A�C�R�����D�F�ɒu������
    }
    void Player1RespawnCntEnd()
    {
        //UISystem.DisplayOff(p1RespawnUI);�@�@�@�@�@�@  //�����̃J�E���g�_�E��UI���B��
        //UISystem.DisplayOff(redSpiderImageGray);�@�@�@ //�D�F�̃X�p�C�_�[�A�C�R�����B��
        player1Timer = Global.RESPAWN_TIME;�@�@�@�@�@  //�J�E���g�_�E�������Z�b�g
        isPlayer1Respawn = false;                      //�����̏�����off!
    }
    void Player2RespawnCntBegin()
    {
        isPlayer2Respawn = true;                        //�����̏�����on!
        //UISystem.DisplayOn(p2RespawnUI);  �@�@�@�@�@�@�@//�����̃J�E���g�_�E��UI������
        //UISystem.DisplayOn(yellowSpiderImageGray);      //�X�p�C�_�[�A�C�R�����D�F�ɒu������                                                        
    }
    void Player2RespawnCntEnd()
    {
        //UISystem.DisplayOff(p2RespawnUI);               //�����̃J�E���g�_�E��UI���B��
        //UISystem.DisplayOff(yellowSpiderImageGray);     //�D�F�̃X�p�C�_�[�A�C�R�����B��
        player2Timer = Global.RESPAWN_TIME;             //�J�E���g�_�E�������Z�b�g
        isPlayer2Respawn = false;                       //�����̏�����off!
    }
}
