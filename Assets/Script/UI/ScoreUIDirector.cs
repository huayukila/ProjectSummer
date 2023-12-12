using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Kit;
using System.Collections;

public class ScoreUIDirector : MonoBehaviour
{
    //public GameObject timeUI;

    float timerSetting = Global.SET_GAME_TIME;

    public Image TimeBarFront;
    public Image DushBar_Yellow_Left;
    public Image DushBar_Yellow_Right;

    private Image[] boostBar = new Image[2];
    private bool[] isBoostStart = new bool[2];
    private float[] boostCooldownTimer = new float[2];

    [Header("Prefab")]
    public CountDownCtrler startCountDownCtrler;
    public CountDownCtrler finishCountDownCtrler;

    void Start()
    {
        Time.timeScale = 0.0f;
        startCountDownCtrler.StartCountDown(() => Time.timeScale = 1.0f);

        for (int i = 0; i < 2; i++)
        {
            boostCooldownTimer[i]= Global.BOOST_COOLDOWN_TIME;//ブーストバーのクールダウンを初期化
        }
        boostBar[0] = DushBar_Yellow_Left;
        boostBar[1] = DushBar_Yellow_Right;

        //タイマーをセット（Global.SET_GAME_TIME - 9f）、タイマーを終わると、カウントダウンを始める。
        ActionKit.Delay(Global.SET_GAME_TIME - 9f, () =>
        {
            finishCountDownCtrler.StartCountDown(() =>
            {
                AudioManager.Instance.StopBGM();
                ScoreModel.Instance.SetScore();
                TypeEventSystem.Instance.Send<GameOver>();
            });
        }).Start(this);

        TypeEventSystem.Instance.Register<BoostStart>(e =>
        {
            BoostStart(e.Nomber);
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    private void FixedUpdate()
    {
        BoostBarUpdate();
    }
    void Update()
    {
        #region 臨時コード
        //if (timer.GetTime() <= 9.0f)
        //{

        //    startCountDownCtrler.StartCountDown(() => 
        //    { 
        //        AudioManager.Instance.StopBGM(); 
        //        TypeEventSystem.Instance.Send<GameOver>(); 
        //    });
        //}
        //if(fightAnimControl.isPlayDone == true)
        //{
        //    AudioManager.Instance.StopBGM();
        //    TypeEventSystem.Instance.Send<GameOver>();                 //GameOver命令を発送、EndSceneへ切り替え
        //}
        #endregion

        timerSetting -= Time.deltaTime;

        TimeBarFront.fillAmount = timerSetting / Global.SET_GAME_TIME;

        #region Boostテスト用
        //if (Input.GetKeyDown(KeyCode.V))
        //{

        //    BoostStart boostStart1 = new BoostStart();
        //    boostStart1.Nomber = 0;

        //    TypeEventSystem.Instance.Send<BoostStart>(boostStart1);

        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{

        //    BoostStart boostStart2 = new BoostStart();
        //    boostStart2.Nomber = 1;

        //    TypeEventSystem.Instance.Send<BoostStart>(boostStart2);

        //}
        #endregion
    }

    void BoostStart(int nomber)
    {
        switch (nomber)
        {
            case 0:
                if (isBoostStart[0] != true)
                {
                    isBoostStart[0] = true;
                }
                break;
            case 1:
                if (isBoostStart[1] != true)
                {
                    isBoostStart[1] = true;
                }
                break;
        }
    }
    private void BoostBarUpdate()
    {
        for (int i = 0; i < 2; ++i) 
        {
            if (isBoostStart[i])
            {
                boostBar[i].fillAmount -= Time.fixedDeltaTime / Global.BOOST_DURATION_TIME;//ブーストのチャージバーを減少
                boostCooldownTimer[i] -= Time.fixedDeltaTime;//クールダウンタイマー減少
                if (boostCooldownTimer[i] <= (1.0f / Global.BOOST_BAR_CHARGING_SPEED) * Time.fixedDeltaTime) //リチャージ時間の直前に
                {
                    isBoostStart[i] = false;
                }
            }
            else
            {
                boostCooldownTimer[i] = Global.BOOST_COOLDOWN_TIME;//クールダウンタイマーリセット
                boostBar[i].fillAmount += Global.BOOST_BAR_CHARGING_SPEED;//ブーストのチャージバーをリチャージ
            }
        }
    }
}
