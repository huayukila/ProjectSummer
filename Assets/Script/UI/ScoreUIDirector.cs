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
    GameObject player1;
    GameObject player2;
    float player1Timer;
    float player2Timer;
    public GameObject timeUI;
    public Timer timer;
    public float timerSetting = 60f;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();

        player1Timer = Global.RESPAWN_TIME;
        player2Timer = Global.RESPAWN_TIME;

        timer.SetTimer(timerSetting, () => { Debug.Log("Timer finished!"); });

        UIsystem.DisplayOff(p1RespawnUI);//�����̃J�E���g�_�E��UI���B��
        UIsystem.DisplayOff(p2RespawnUI);
        this.player1 = GameObject.Find("Player1");
        this.player2 = GameObject.Find("Player2");

        TypeEventSystem.Instance.Register<Player1RespawnCntBegin>(e => { Player1RespawnCntBegin(); });
        TypeEventSystem.Instance.Register<Player1RespawnCntEnd>(e => { Player1RespawnCntEnd(); });
        TypeEventSystem.Instance.Register<Player2RespawnCntBegin>(e => { Player2RespawnCntBegin(); });
        TypeEventSystem.Instance.Register<Player2RespawnCntEnd>(e => { Player2RespawnCntEnd(); });
    }

    // Update is called once per frame
    void Update()
    {
        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();�@  //�e�L�X�g�̓��e
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();�@  //�e�L�X�g�̓��e
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timer.GetTime().ToString("F2");     //�^�C�}�[�̃e�L�X�g�̓��e

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

        if (timer.IsTimerFinished())                                    //�^�C�}�[���O�ɂȂ��END�V�[���ɐ؂�ւ���
        {
            TypeEventSystem.Instance.Send<GameOver>();                 //GameOver���߂𔭑��AEndScene�֐؂�ւ�
        }
    }
    void Player1RespawnCntBegin()
    {
        UIsystem.DisplayOn(p1RespawnUI);         ////�����̃J�E���g�_�E��UI���B��
        player1Timer -= Time.deltaTime;
        p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F1");
    }
    void Player1RespawnCntEnd()
    {
        UIsystem.DisplayOff(p1RespawnUI);
        player1Timer = Global.RESPAWN_TIME;
    }
    void Player2RespawnCntBegin()
    {
        UIsystem.DisplayOn(p2RespawnUI);         ////�����̃J�E���g�_�E��UI���B��
        player2Timer -= Time.deltaTime;
        this.p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F1");
    }
    void Player2RespawnCntEnd()
    {
        UIsystem.DisplayOff(p2RespawnUI);
        player2Timer = Global.RESPAWN_TIME;
    }
}
