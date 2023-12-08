using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Kit;

public class ScoreUIDirector : MonoBehaviour
{
    //public GameObject p1ScoreUI;
    //public GameObject p2ScoreUI;

    //public GameObject p1RespawnUI;
    //public GameObject p2RespawnUI;

    float player1Timer;
    float player2Timer;

    //public GameObject redSpiderImageGray;
    //public GameObject yellowSpiderImageGray;

    public GameObject timeUI;

    bool isPlayer1Respawn;
    bool isPlayer2Respawn;

    float timerSetting = Global.SET_GAME_TIME;

    public Image TimeBarFront;

    bool aaa;

    [Header("Prefab")]
    public CountDownCtrler startCountDownCtrler;
    public CountDownCtrler finishCountDownCtrler;

    void Start()
    {
        Time.timeScale = 0.0f;
        startCountDownCtrler.StartCountDown(() => Time.timeScale = 1.0f);

        player1Timer = Global.RESPAWN_TIME;//復活の時間を設定
        player2Timer = Global.RESPAWN_TIME;

        //タイマーをセット（Global.SET_GAME_TIME - 9f）、タイマーを終わると、カウントダウンを始める。
        ActionKit.Delay(Global.SET_GAME_TIME - 9f, () =>
        {
            finishCountDownCtrler.StartCountDown(() =>
            {
                AudioManager.Instance.StopBGM();
                TypeEventSystem.Instance.Send<GameOver>();
            });
        }).Start(this);

        //timer.SetTimer(Global.SET_GAME_TIME - 9f, () =>
        //{
            
        //});

        //UISystem.DisplayOff(p1RespawnUI);//復活のカウントダウンUIを隠す
        //UISystem.DisplayOff(p2RespawnUI);

        //UISystem.DisplayOff(redSpiderImageGray);//灰色のスパイダーアイコンを隠す
        //UISystem.DisplayOff(yellowSpiderImageGray) ;
        
        TypeEventSystem.Instance.Register<Player1RespawnCntBegin>(e => { Player1RespawnCntBegin(); });  //プレイヤー１が死んだ時、カウントダウンを開始。
        TypeEventSystem.Instance.Register<Player2RespawnCntBegin>(e => { Player2RespawnCntBegin(); });　//プレイヤー２が死んだ時、カウントダウンを開始。
        //TypeEventSystem.Instance.Register<Player1RespawnCntEnd>(e =>   { Player1RespawnCntEnd(); });　　//プレイヤー１が復活する時、カウントダウンをリセット。
        //TypeEventSystem.Instance.Register<Player2RespawnCntEnd>(e =>   { Player2RespawnCntEnd(); });　　//プレイヤー２が復活する時、カウントダウンをリセット。
    }

    // Update is called once per frame
    void Update()
    {
        //p1ScoreUI.GetComponent<TextMeshProUGUI>().text =
        //    ScoreSystem.Instance.GetPlayer1Score().ToString();　  //テキストの内容
        //p2ScoreUI.GetComponent<TextMeshProUGUI>().text =
        //    ScoreSystem.Instance.GetPlayer2Score().ToString();　  //テキストの内容

        if (isPlayer1Respawn)
        {
            player1Timer -= Time.deltaTime;                                                    //カウントダウンを開始
            //p1RespawnUI.GetComponent<TextMeshProUGUI>().text = player1Timer.ToString("F1");  //カウントダウンのテキスト
            if (player1Timer <= 0)//カウントダウンを終わると
            {
                Player1RespawnCntEnd();//カウントダウンをリセットなど
            }
        }
        if (isPlayer2Respawn)
        {
            player2Timer -= Time.deltaTime;                                                    //カウントダウンを開始
            //p2RespawnUI.GetComponent<TextMeshProUGUI>().text = player2Timer.ToString("F1");    //カウントダウンのテキスト
            if(player2Timer <= 0)//カウントダウンを終わると
            {
                Player2RespawnCntEnd();//カウントダウンをリセットなど
            }
        }

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

        timerSetting-= Time.deltaTime;


        TimeBarFront.fillAmount = timerSetting/ Global.SET_GAME_TIME;

        //テスト用‐‐‐‐‐‐‐‐
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TypeEventSystem.Instance.Send<Player2RespawnCntBegin>();
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    TypeEventSystem.Instance.Send<Player1RespawnCntBegin>();
        //}
    }
    void Player1RespawnCntBegin()
    {
        isPlayer1Respawn = true;　　　　　　　　　　　 //復活の処理をon!
        //UISystem.DisplayOn(p1RespawnUI);         　　　//復活のカウントダウンUIを現す
        //UISystem.DisplayOn(redSpiderImageGray);        //スパイダーアイコンを灰色に置き換え
    }
    void Player1RespawnCntEnd()
    {
        //UISystem.DisplayOff(p1RespawnUI);　　　　　　  //復活のカウントダウンUIを隠す
        //UISystem.DisplayOff(redSpiderImageGray);　　　 //灰色のスパイダーアイコンを隠す
        player1Timer = Global.RESPAWN_TIME;　　　　　  //カウントダウンをリセット
        isPlayer1Respawn = false;                      //復活の処理をoff!
    }
    void Player2RespawnCntBegin()
    {
        isPlayer2Respawn = true;                        //復活の処理をon!
        //UISystem.DisplayOn(p2RespawnUI);  　　　　　　　//復活のカウントダウンUIを現す
        //UISystem.DisplayOn(yellowSpiderImageGray);      //スパイダーアイコンを灰色に置き換え                                                        
    }
    void Player2RespawnCntEnd()
    {
        //UISystem.DisplayOff(p2RespawnUI);               //復活のカウントダウンUIを隠す
        //UISystem.DisplayOff(yellowSpiderImageGray);     //灰色のスパイダーアイコンを隠す
        player2Timer = Global.RESPAWN_TIME;             //カウントダウンをリセット
        isPlayer2Respawn = false;                       //復活の処理をoff!
    }
}
