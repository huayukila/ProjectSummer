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
    private ScoreSystem scoreSystem;�@�@�@�@�@�@�@�@�@�@//ScoreSystem�����ቻ
    private Timer timer;
    public float setTheTimer = 60f;
    // Start is called before the first frame update
    void Start()
    {
        this.P1ScoreUI = GameObject.Find("P1ScoreUI");�@// �v���C���O�P��UI���Q�b�g
        this.P2ScoreUI = GameObject.Find("P2ScoreUI");�@// �v���C���O�Q��UI���Q�b�g
        this.TimeUI = GameObject.Find("TimeUI");

        timer = new Timer();
        timer.SetTimer(setTheTimer, () => { Debug.Log("Timer finished!"); });

        scoreSystem = new ScoreSystem();
    }

    // Update is called once per frame
    void Update()
    {
        int scoreplayer01 = scoreSystem.GetPlayer1Score();�@�@�@�@�@�@�@//�v���C���O�P�̃X�R�A����
        int scorePlayer02 = scoreSystem.GetPlayer2Score();�@�@�@�@�@�@�@//�v���C���O�Q�̃X�R�A����
        float timerRealTime = timer.GetTime();                          //�^�C�}�[�̃��A���^�C������
        this.P1ScoreUI.GetComponent<TextMeshProUGUI>().text = "RED SCORE:" + scoreplayer01.ToString();�@   //�e�L�X�g�̓��e
        this.P2ScoreUI.GetComponent<TextMeshProUGUI>().text = "YELLOW SCORE:" + scorePlayer02.ToString();�@//�e�L�X�g�̓��e
        this.TimeUI.GetComponent<TextMeshProUGUI>().text = "TIME:" + timerRealTime.ToString("F2");�@�@�@�@ //�^�C�}�[�̃e�L�X�g�̓��e
        if (timer.IsTimerFinished())                                    //�^�C�}�[���O�ɂȂ��END�V�[���ɐ؂�ւ���
        {
            SceneManager.LoadScene("End");
        }
    }
}
