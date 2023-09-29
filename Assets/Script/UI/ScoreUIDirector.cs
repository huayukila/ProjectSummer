using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUIDirector : MonoBehaviour
{
    public GameObject p1ScoreUI;
    public GameObject p2ScoreUI;
    public GameObject p1RespawnUI;
    public GameObject p2RespawnUI;
    public GameObject timeUI;
    public Timer timer;
    public float timerSetting = 60f;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetTimer(timerSetting, () => { Debug.Log("Timer finished!"); });

        p1RespawnUI.SetActive(false);      //UIを隠す
        p2RespawnUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        int scoreplayer01 = ScoreSystem.Instance.GetPlayer1Score();　　　　　　　//プレイヤ０１のスコアを代入
        int scorePlayer02 = ScoreSystem.Instance.GetPlayer2Score();　　　　　　　//プレイヤ０２のスコアを代入

        float timerRealTime = timer.GetTime();                          //タイマーのリアルタイムを代入

        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + scoreplayer01.ToString();　  //テキストの内容
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "SCORE: " + scorePlayer02.ToString();　  //テキストの内容
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timerRealTime.ToString("F2");     //タイマーのテキストの内容

        //if (!GameManager.Instance.playerOne.activeSelf)
        //{

        //}

        //if (!GameManager.Instance.playerTwo.activeSelf)
        //{

        //}

        if (timer.IsTimerFinished())                                    //タイマーが０になるとENDシーンに切り替える
        {
            TypeEventSystem.Instance.Send<GameOver>();                 //GameOver命令を発送、EndSceneへ切り替え
        }
    }
}
