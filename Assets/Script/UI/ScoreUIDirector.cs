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
 
    float player1Timer;
    float player2Timer;

    public GameObject redSpiderImageGray;
    public GameObject yellowSpiderImageGray;

    public GameObject timeUI;

    public Timer timer;

    bool player1RespawnCnt;
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

        UISystem.DisplayOff(p1RespawnUI);//復活のカウントダウンUIを隠す
        UISystem.DisplayOff(p2RespawnUI);

        UISystem.DisplayOff(redSpiderImageGray);
        UISystem.DisplayOff(yellowSpiderImageGray) ;
        
        TypeEventSystem.Instance.Register<Player1RespawnCntBegin>(e => { Player1RespawnCntBegin(); });  //プレイヤー１が死んだ時、カウントダウンを開始。
        TypeEventSystem.Instance.Register<Player2RespawnCntBegin>(e => { Player2RespawnCntBegin(); });　//プレイヤー２が死んだ時、カウントダウンを開始。
        //TypeEventSystem.Instance.Register<Player1RespawnCntEnd>(e =>   { Player1RespawnCntEnd(); });　　//プレイヤー１が復活する時、カウントダウンをリセット。
        //TypeEventSystem.Instance.Register<Player2RespawnCntEnd>(e =>   { Player2RespawnCntEnd(); });　　//プレイヤー２が復活する時、カウントダウンをリセット。
    }

    // Update is called once per frame
    void Update()
    {
        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();　  //テキストの内容
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();　  //テキストの内容
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timer.GetTime().ToString("F2");     //タイマーのテキストの内容

        if (player1RespawnCnt)
        {
            player1Timer -= Time.deltaTime;                                                    //カウントダウンを開始
            p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F1");  //カウントダウンのテキスト
            if (player1Timer <= 0)
            {
                Player1RespawnCntEnd();
            }
        }
        if (player2RespawnCnt)
        {
            player2Timer -= Time.deltaTime;                                                    //カウントダウンを開始
            p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F1");    //カウントダウンのテキスト
            if(player2Timer <= 0)
            {
                Player2RespawnCntEnd();
            }
        }

        if (timer.IsTimerFinished())                                    //タイマーが０になるとENDシーンに切り替える
        {
            TypeEventSystem.Instance.Send<GameOver>();                 //GameOver命令を発送、EndSceneへ切り替え
        }
    }
    void Player1RespawnCntBegin()
    {
        player1RespawnCnt = true;
        UISystem.DisplayOn(p1RespawnUI);         　　　　　　　　　　　　　　　　　　　　　//復活のカウントダウンUIを現す
        UISystem.DisplayOn(redSpiderImageGray);
    }
    void Player1RespawnCntEnd()
    {
        UISystem.DisplayOff(p1RespawnUI);　　　　　　　　　　　　　　　　　　　　　　　　　//復活のカウントダウンUIを隠す
        UISystem.DisplayOff(redSpiderImageGray);
        player1Timer = Global.RESPAWN_TIME;　　　　　　　　　　　　　　　　　　　　　　　　//カウントダウンをリセット
        player1RespawnCnt = false;
    }
    void Player2RespawnCntBegin()
    {
        player2RespawnCnt = true;
        UISystem.DisplayOn(p2RespawnUI);  　　　　　　　　　　　　　　　　　　　　　　　　 //復活のカウントダウンUIを現す
        UISystem.DisplayOn(yellowSpiderImageGray);                                                                
    }
    void Player2RespawnCntEnd()
    {
        UISystem.DisplayOff(p2RespawnUI);                                                  //復活のカウントダウンUIを隠す
        UISystem.DisplayOff(yellowSpiderImageGray) ;
        player2Timer = Global.RESPAWN_TIME;                                                //カウントダウンをリセット
        player2RespawnCnt = false;
    }
}
