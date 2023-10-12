using UnityEngine;
using TMPro;

public class ScoreUIDirector : MonoBehaviour
{
    public GameObject p1ScoreUI;
    public GameObject p2ScoreUI;

    public GameObject p1RespawnUI;
    public GameObject p2RespawnUI;
 
    float player1Timer;
    float player2Timer;

    public GameObject redSpiderImageGray;
    public GameObject yellowSpiderImageGray;

    public GameObject timeUI;

    public Timer timer;

    bool isPlayer1Respawn;
    bool player2RespawnCnt;

    float timerSetting = Global.SET_GAME_TIME;

    void Start()
    {
        timer = new Timer();

        player1Timer = Global.RESPAWN_TIME;
        player2Timer = Global.RESPAWN_TIME;

        bool player1RespawnCnt = false;
        bool player2RespawnCnt = false;

        timer.SetTimer(timerSetting, () => { Debug.Log("Timer finished!"); });

        UISystem.DisplayOff(p1RespawnUI);//�����̃J�E���g�_�E��UI���B��
        UISystem.DisplayOff(p2RespawnUI);

        UISystem.DisplayOff(redSpiderImageGray);
        UISystem.DisplayOff(yellowSpiderImageGray) ;
        
        TypeEventSystem.Instance.Register<Player1RespawnCntBegin>(e => { Player1RespawnCntBegin(); });  //�v���C���[�P�����񂾎��A�J�E���g�_�E�����J�n�B
        TypeEventSystem.Instance.Register<Player2RespawnCntBegin>(e => { Player2RespawnCntBegin(); });�@//�v���C���[�Q�����񂾎��A�J�E���g�_�E�����J�n�B
        //TypeEventSystem.Instance.Register<Player1RespawnCntEnd>(e =>   { Player1RespawnCntEnd(); });�@�@//�v���C���[�P���������鎞�A�J�E���g�_�E�������Z�b�g�B
        //TypeEventSystem.Instance.Register<Player2RespawnCntEnd>(e =>   { Player2RespawnCntEnd(); });�@�@//�v���C���[�Q���������鎞�A�J�E���g�_�E�������Z�b�g�B
    }

    // Update is called once per frame
    void Update()
    {
        p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();�@  //�e�L�X�g�̓��e
        p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();�@  //�e�L�X�g�̓��e
        timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timer.GetTime().ToString("F2");     //�^�C�}�[�̃e�L�X�g�̓��e

        if (isPlayer1Respawn)
        {
            player1Timer -= Time.deltaTime;                                                    //�J�E���g�_�E�����J�n
            p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F1");  //�J�E���g�_�E���̃e�L�X�g
            if (player1Timer <= 0)
            {
                Player1RespawnCntEnd();
            }
        }
        if (player2RespawnCnt)
        {
            player2Timer -= Time.deltaTime;                                                    //�J�E���g�_�E�����J�n
            p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F1");    //�J�E���g�_�E���̃e�L�X�g
            if(player2Timer <= 0)
            {
                Player2RespawnCntEnd();
            }
        }

        if (timer.IsTimerFinished())                                    //�^�C�}�[���O�ɂȂ��END�V�[���ɐ؂�ւ���
        {
            TypeEventSystem.Instance.Send<GameOver>();                 //GameOver���߂𔭑��AEndScene�֐؂�ւ�
        }
    }
    void Player1RespawnCntBegin()
    {
        isPlayer1Respawn = true;
        UISystem.DisplayOn(p1RespawnUI);         �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�����̃J�E���g�_�E��UI������
        UISystem.DisplayOn(redSpiderImageGray);
    }
    void Player1RespawnCntEnd()
    {
        UISystem.DisplayOff(p1RespawnUI);�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�����̃J�E���g�_�E��UI���B��
        UISystem.DisplayOff(redSpiderImageGray);
        player1Timer = Global.RESPAWN_TIME;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�J�E���g�_�E�������Z�b�g
        isPlayer1Respawn = false;
    }
    void Player2RespawnCntBegin()
    {
        player2RespawnCnt = true;
        UISystem.DisplayOn(p2RespawnUI);  �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@ //�����̃J�E���g�_�E��UI������
        UISystem.DisplayOn(yellowSpiderImageGray);                                                                
    }
    void Player2RespawnCntEnd()
    {
        UISystem.DisplayOff(p2RespawnUI);                                                  //�����̃J�E���g�_�E��UI���B��
        UISystem.DisplayOff(yellowSpiderImageGray) ;
        player2Timer = Global.RESPAWN_TIME;                                                //�J�E���g�_�E�������Z�b�g
        player2RespawnCnt = false;
    }
}
