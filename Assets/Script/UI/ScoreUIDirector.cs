using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ScoreUIDirector : MonoBehaviour
{
    public GameObject p1ScoreUI;
    public GameObject p2ScoreUI;

    public GameObject p1RespawnUI;
    public GameObject p2RespawnUI;
    //GameObject player1;//�e�X�g�p
    //GameObject player2;
    float player1Timer;
    float player2Timer;

    public GameObject timeUI;

    public Timer timer;


    float timerSetting = Global.SET_GAME_TIME;


    void Start()
    {
        timer = new Timer();

        player1Timer = Global.RESPAWN_TIME;
        player2Timer = Global.RESPAWN_TIME;

        timer.SetTimer(timerSetting, () => { Debug.Log("Timer finished!"); });

        UISystem.DisplayOff(p1RespawnUI);//�����̃J�E���g�_�E��UI���B��
        UISystem.DisplayOff(p2RespawnUI);

        //this.player1 = GameObject.Find("Player1");//�e�X�g�p
        //this.player2 = GameObject.Find("Player2");
        
        TypeEventSystem.Instance.Register<Player1RespawnCntBegin>(e => { Player1RespawnCntBegin(); });  //�v���C���[�P�����񂾎��A�J�E���g�_�E�����J�n�B
        TypeEventSystem.Instance.Register<Player1RespawnCntEnd>(e =>   { Player1RespawnCntEnd(); });�@�@//�v���C���[�P���������鎞�A�J�E���g�_�E�������Z�b�g�B
        TypeEventSystem.Instance.Register<Player2RespawnCntBegin>(e => { Player2RespawnCntBegin(); });�@//�v���C���[�Q�����񂾎��A�J�E���g�_�E�����J�n�B
        TypeEventSystem.Instance.Register<Player2RespawnCntEnd>(e =>   { Player2RespawnCntEnd(); });�@�@//�v���C���[�Q���������鎞�A�J�E���g�_�E�������Z�b�g�B
    }

    // Update is called once per frame
    void Update()
    {
        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();�@  //�e�L�X�g�̓��e
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();�@  //�e�L�X�g�̓��e
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timer.GetTime().ToString("F2");     //�^�C�}�[�̃e�L�X�g�̓��e


        //�e�X�g�p�]�]�]�]�]�]�]�]�]�]�]�]�]�]�]�]�]�]
        //if (this.player1.active ==false)
        //{
        //    TypeEventSystem.Instance.Send<Player1RespawnCntBegin>();
        //}
        //else
        //{
        //    TypeEventSystem.Instance.Send<Player1RespawnCntEnd>();
        //}

        //if (this.player2.active == false)
        //{
        //    TypeEventSystem.Instance.Send<Player2RespawnCntBegin>();
        //}
        //else
        //{
        //    TypeEventSystem.Instance.Send<Player2RespawnCntEnd>();
        //}


        
        /*
        if (this.player1.active == false) 
        {
            p1RespawnUI.active= true;
            player1Timer -= Time.deltaTime;
            this.p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F0");
        }
        else
        {
            p1RespawnUI.active = false;
            player1Timer = Global.RESPAWN_TIME;
        }

        if (this.player2.active == false)
        {
            p2RespawnUI.active = true;
            player2Timer -= Time.deltaTime;
            this.p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F0");
        }
        else
        {
            p2RespawnUI.active = false;
            player2Timer = Global.RESPAWN_TIME;
        }
        */
 
        if (timer.IsTimerFinished())                                    //�^�C�}�[���O�ɂȂ��END�V�[���ɐ؂�ւ���
        {
            TypeEventSystem.Instance.Send<GameOver>();                 //GameOver���߂𔭑��AEndScene�֐؂�ւ�
        }
    }
    void Player1RespawnCntBegin()
    {
        UISystem.DisplayOn(p1RespawnUI);         �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�����̃J�E���g�_�E��UI������
        player1Timer -= Time.deltaTime;                                                    //�J�E���g�_�E�����J�n
        p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F1");�@�@//�J�E���g�_�E���̃e�L�X�g
    }
    void Player1RespawnCntEnd()
    {
        UISystem.DisplayOff(p1RespawnUI);�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�����̃J�E���g�_�E��UI���B��
        player1Timer = Global.RESPAWN_TIME;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�J�E���g�_�E�������Z�b�g
    }
    void Player2RespawnCntBegin()
    {
        UISystem.DisplayOn(p2RespawnUI);  �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@ //�����̃J�E���g�_�E��UI������       
        player2Timer -= Time.deltaTime;                                                    //�J�E���g�_�E�����J�n
        p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F1");    //�J�E���g�_�E���̃e�L�X�g
    }
    void Player2RespawnCntEnd()
    {
        UISystem.DisplayOff(p2RespawnUI);                                                  //�����̃J�E���g�_�E��UI���B��
        player2Timer = Global.RESPAWN_TIME;                                                //�J�E���g�_�E�������Z�b�g
    }
}
