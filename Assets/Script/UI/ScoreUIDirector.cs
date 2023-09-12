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
        int scoreplayer01 = ScoreSystem.Instance.GetPlayer1Score();�@�@�@�@�@�@�@//�v���C���O�P�̃X�R�A����
        int scorePlayer02 = ScoreSystem.Instance.GetPlayer2Score();�@�@�@�@�@�@�@//�v���C���O�Q�̃X�R�A����
        
        float timerRealTime = timer.GetTime();                          //�^�C�}�[�̃��A���^�C������
        
        this.p1ScoreUI.GetComponent<TextMeshProUGUI>().text = "RED SCORE:" + scoreplayer01.ToString();�@   //�e�L�X�g�̓��e
        this.p2ScoreUI.GetComponent<TextMeshProUGUI>().text = "YELLOW SCORE:" + scorePlayer02.ToString();�@//�e�L�X�g�̓��e
        this.timeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timerRealTime.ToString("F2");�@�@�@�@ //�^�C�}�[�̃e�L�X�g�̓��e
        
        if (timer.IsTimerFinished())                                    //�^�C�}�[���O�ɂȂ��END�V�[���ɐ؂�ւ���
        {
            SceneManager.LoadScene("End");
        }
    }
}
