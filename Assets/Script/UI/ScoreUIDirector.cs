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
    //GameObject player1;//テスト用
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

        UISystem.DisplayOff(p1RespawnUI);//復活のカウントダウンUIを隠す
        UISystem.DisplayOff(p2RespawnUI);

        //this.player1 = GameObject.Find("Player1");//テスト用
        //this.player2 = GameObject.Find("Player2");
        
        TypeEventSystem.Instance.Register<Player1RespawnCntBegin>(e => { Player1RespawnCntBegin(); });  //プレイヤー１が死んだ時、カウントダウンを開始。
        TypeEventSystem.Instance.Register<Player1RespawnCntEnd>(e =>   { Player1RespawnCntEnd(); });　　//プレイヤー１が復活する時、カウントダウンをリセット。
        TypeEventSystem.Instance.Register<Player2RespawnCntBegin>(e => { Player2RespawnCntBegin(); });　//プレイヤー２が死んだ時、カウントダウンを開始。
        TypeEventSystem.Instance.Register<Player2RespawnCntEnd>(e =>   { Player2RespawnCntEnd(); });　　//プレイヤー２が復活する時、カウントダウンをリセット。
    }

    // Update is called once per frame
    void Update()
    {
        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();　  //テキストの内容
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();　  //テキストの内容
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timer.GetTime().ToString("F2");     //タイマーのテキストの内容


        //テスト用‐‐‐‐‐‐‐‐‐‐‐‐‐‐‐‐‐‐
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
 
        if (timer.IsTimerFinished())                                    //タイマーが０になるとENDシーンに切り替える
        {
            TypeEventSystem.Instance.Send<GameOver>();                 //GameOver命令を発送、EndSceneへ切り替え
        }
    }
    void Player1RespawnCntBegin()
    {
        UISystem.DisplayOn(p1RespawnUI);         　　　　　　　　　　　　　　　　　　　　　//復活のカウントダウンUIを現す
        player1Timer -= Time.deltaTime;                                                    //カウントダウンを開始
        p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F1");　　//カウントダウンのテキスト
    }
    void Player1RespawnCntEnd()
    {
        UISystem.DisplayOff(p1RespawnUI);　　　　　　　　　　　　　　　　　　　　　　　　　//復活のカウントダウンUIを隠す
        player1Timer = Global.RESPAWN_TIME;　　　　　　　　　　　　　　　　　　　　　　　　//カウントダウンをリセット
    }
    void Player2RespawnCntBegin()
    {
        UISystem.DisplayOn(p2RespawnUI);  　　　　　　　　　　　　　　　　　　　　　　　　 //復活のカウントダウンUIを現す       
        player2Timer -= Time.deltaTime;                                                    //カウントダウンを開始
        p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F1");    //カウントダウンのテキスト
    }
    void Player2RespawnCntEnd()
    {
        UISystem.DisplayOff(p2RespawnUI);                                                  //復活のカウントダウンUIを隠す
        player2Timer = Global.RESPAWN_TIME;                                                //カウントダウンをリセット
    }
}
