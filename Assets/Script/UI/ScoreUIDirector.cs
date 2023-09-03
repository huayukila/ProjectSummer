using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUIDirector : MonoBehaviour
{
    GameObject P1ScoreUI;
    GameObject P2ScoreUI;
    GameObject TimeUI;
    private ScoreSystem scoreSystem;　　　　　　　　　　//ScoreSystemを実例化
    private Timer timer;
    public float setTheTimer = 60f;
    // Start is called before the first frame update
    void Start()
    {
        this.P1ScoreUI = GameObject.Find("P1ScoreUI");　// プレイヤ０１のUIをゲット
        this.P2ScoreUI = GameObject.Find("P2ScoreUI");　// プレイヤ０２のUIをゲット
        this.TimeUI = GameObject.Find("TimeUI");

        timer = new Timer();
        timer.SetTimer(setTheTimer, () => { Debug.Log("Timer finished!"); });

        scoreSystem = new ScoreSystem();
    }

    // Update is called once per frame
    void Update()
    {
        int scoreplayer01 = scoreSystem.GetPlayer1Score();　　　　　　　//プレイヤ０１のスコアを代入
        int scorePlayer02 = scoreSystem.GetPlayer2Score();　　　　　　　//プレイヤ０２のスコアを代入
        float timerRealTime = timer.GetTime();                          //タイマーのリアルタイムを代入
        this.P1ScoreUI.GetComponent<TextMeshProUGUI>().text = "RED SCORE:" + scoreplayer01.ToString();　   //テキストの内容
        this.P2ScoreUI.GetComponent<TextMeshProUGUI>().text = "YELLOW SCORE:" + scorePlayer02.ToString();　//テキストの内容
        this.TimeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timerRealTime.ToString("F2");　　　　 //タイマーのテキストの内容
        if (timer.IsTimerFinished())                                    //タイマーが０になるとENDシーンに切り替える
        {
            SceneManager.LoadScene("End");
        }
    }
}
