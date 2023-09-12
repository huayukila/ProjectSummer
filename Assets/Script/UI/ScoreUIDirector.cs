using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUIDirector : MonoBehaviour
{
    public GameObject p1ScoreUI;
    public GameObject p2ScoreUI;
    public GameObject timeUI;
    public Timer timer;
    public float timerSetting = 60f;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetTimer(timerSetting, () => { Debug.Log("Timer finished!"); });
    }

    // Update is called once per frame
    void Update()
    {
        int scoreplayer01 = ScoreSystem.Instance.GetPlayer1Score();　　　　　　　//プレイヤ０１のスコアを代入
        int scorePlayer02 = ScoreSystem.Instance.GetPlayer2Score();　　　　　　　//プレイヤ０２のスコアを代入
        
        float timerRealTime = timer.GetTime();                          //タイマーのリアルタイムを代入
        
        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "RED SCORE:" + scoreplayer01.ToString();　   //テキストの内容
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "YELLOW SCORE:" + scorePlayer02.ToString();　//テキストの内容
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timerRealTime.ToString("F2");　　　　 //タイマーのテキストの内容
        
        if (timer.IsTimerFinished())                                    //タイマーが０になるとENDシーンに切り替える
        {
            SceneManager.LoadScene("End");
        }
    }
}
