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

        p1RespawnUI.SetActive(false);      //�����̃J�E���g�_�E��UI���B��
        p2RespawnUI.SetActive(false);

        this.player1 = GameObject.Find("Player1");
        this.player2 = GameObject.Find("Player2");
    }

    // Update is called once per frame
    void Update()
    {
        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();�@  //�e�L�X�g�̓��e
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();�@  //�e�L�X�g�̓��e
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timer.GetTime().ToString("F2");     //�^�C�}�[�̃e�L�X�g�̓��e
        
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
}
