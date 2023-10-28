using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public interface IPlayerSetStatus
{
    
}
public struct SpiderPlayer
{
    public GameObject gameObject;
    public GameObject playerImage;
    public PlayerStatus playerStatus;
    public Timer spawnTimer;
}
public class GameManager : Singleton<GameManager>
{
    private static int maxPlayerCount = 2;
    private static int count = 0;
    private Dictionary<int, SpiderPlayer> spiderPlayers;
    private Dictionary<int, GameObject> players;
    public GameObject playerOne;
    public GameObject playerTwo;
    public GameObject bigSpiderPrefab
    {
        get
        {
            return _bigSpiderPrefab;
        }

    }

    private ItemSystem itemSystem;
    private GameResourceSystem gameResourceSystem;

    private GameObject _bigSpiderPrefab;

    private Timer _player1Timer;        // プレイヤー1の待機タイマー
    private Timer _player2Timer;        // プレイヤー2の待機タイマー
    private GameObject _player1Prefab;
    private GameObject _player2Prefab;

    protected override void Awake()
    {
        base.Awake();
        count++;
        //各システムの実例化と初期化
        {
            itemSystem = ItemSystem.Instance;
            itemSystem.Init();
        }

        {
            gameResourceSystem = GameResourceSystem.Instance;
            gameResourceSystem.Init();
        }
        //シーンの移行命令を受け
        TypeEventSystem.Instance.Register<TitleSceneSwitch>(e => { TitleSceneSwitch(); });
        TypeEventSystem.Instance.Register<MenuSceneSwitch>(e => { MenuSceneSwitch(); });
        TypeEventSystem.Instance.Register<GamingSceneSwitch>(e => { GamingSceneSwitch(); });
        TypeEventSystem.Instance.Register<EndSceneSwitch>(e => { EndSceneSwitch(); });
        TypeEventSystem.Instance.Register<GameOver>(e => { EndSceneSwitch(); });

        Init();
        
        SceneManager.sceneLoaded += SceneLoaded;

        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    if (device is Keyboard)
                    {

                    }
                    else if (device is Gamepad)
                    {
                        for (int i = 0; i < PlayerInput.all.Count; ++i)
                        {
                            if (PlayerInput.all[i].GetDevice<InputDevice>() is not Gamepad)
                            {
                                PlayerInput.all[i].SwitchCurrentControlScheme(
                                "Gamepad",
                                device as Gamepad);
                                break;
                            }
                        }
                    }
                    Debug.Log(device.displayName + " Added");
                    break;
                case InputDeviceChange.Disconnected:
                    InputSystem.FlushDisconnectedDevices();
                    Debug.LogWarning(device.displayName + " Disconnected");
                    break;
                case InputDeviceChange.Removed:
                    if (device is Gamepad)
                    {
                        for (int i = 0; i < PlayerInput.all.Count; ++i)
                        {
                            if (PlayerInput.all[i].GetDevice<InputDevice>() == device && Keyboard.current != null)
                            {
                                PlayerInput.all[i].SwitchCurrentControlScheme(
                                "Keyboard&Mouse",
                                Keyboard.current);
                                break;
                            }
                        }
                    }
                    InputSystem.RemoveDevice(device);
                    Debug.LogWarning(device.displayName + " Removed");
                    break;
            }
        };

        _bigSpiderPrefab = Resources.Load("Prefabs/BigSpider") as GameObject;

        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log(count);
        SceneManager.LoadScene("Title");
    }

    private void Start()
    {
        AudioManager.Instance.PlayBGM("TitleBGM", 0.3f);
    }

    private void Update()
    {
        //各システムのupdate
        //シーンの移行など
        RespawnCheck();
    }

    //シーンの移行
    void TitleSceneSwitch()
    {
        SceneManager.LoadScene("Title");
        //ScoreSystem.Instance.ResetScore();
    }
    void MenuSceneSwitch()
    {
        SceneManager.LoadScene("MenuScene");
    }
    void GamingSceneSwitch()
    {
        SceneManager.LoadScene("Gaming");
    }
    void EndSceneSwitch()
    {
        SceneManager.LoadScene("End");
    }

    private void Init()
    {
        _player1Prefab = (GameObject)Resources.Load("Prefabs/Player1");
        _player2Prefab = (GameObject)Resources.Load("Prefabs/Player2");
        spiderPlayers = new Dictionary<int, SpiderPlayer>();
        //TODO test
        players = new Dictionary<int, GameObject>();
        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            RespawnPlayer(e.player);

        }).UnregisterWhenGameObjectDestroyed(gameObject);

    }
    /// <summary>
    /// プレイヤーの復活タイミングをチェックする
    /// </summary>
    private void RespawnCheck()
    {
        // player1のタイマーの待機時間が終わったら
        if (_player1Timer != null)
        {
            if(_player1Timer.IsTimerFinished())
            {
                _player1Timer = null;
            }

        }
        // player2のタイマーの待機時間が終わったら
        if (_player2Timer != null)
        {
            if (_player2Timer.IsTimerFinished())
            {
                _player2Timer = null;
            }
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        if (player == playerOne)
        {
            // 新しいタイマーを生成する
            if (_player1Timer == null)
            {
                _player1Timer = new Timer();
                _player1Timer.SetTimer(Global.RESPAWN_TIME,
                    () =>
                    {
                        RespawnPlayer1();
                    }
                    );
            }

        }
        else if (player == playerTwo)
        {
            // 新しいタイマーを生成する
            if (_player2Timer == null)
            {
                _player2Timer = new Timer();
                _player2Timer.SetTimer(Global.RESPAWN_TIME,
                    () =>
                    {
                        RespawnPlayer2();
                    }
                    );
            }

        }
    }
    /// <summary>
    /// プレイヤー1を復活させる
    /// </summary>
    private void RespawnPlayer1()
    {
        playerOne.transform.position = Global.PLAYER1_START_POSITION;
        playerOne.transform.forward = Vector3.right;
        playerOne.GetComponent<Player1Control>().SetStatus(PlayerStatus.Fine);
        playerOne.GetComponentInChildren<TrailRenderer>().enabled = true;
        playerOne.GetComponent<DropPointControl>().enabled = true;
        playerOne.GetComponent<Collider>().enabled = true;
        GameObject smoke = Instantiate(Resources.Load("Prefabs/Smoke") as GameObject, playerOne.transform.position, Quaternion.identity);
        smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
        smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
    }

    /// <summary>
    /// プレイヤー2を復活させる
    /// </summary>
    private void RespawnPlayer2()
    {
        playerTwo.transform.position = Global.PLAYER2_START_POSITION;
        playerTwo.transform.forward = Vector3.left;
        playerTwo.GetComponent<Player2Control>().SetStatus(PlayerStatus.Fine);
        playerTwo.GetComponentInChildren<TrailRenderer>().enabled = true;
        playerTwo.GetComponent<DropPointControl>().enabled = true;
        playerTwo.GetComponent<Collider>().enabled = true;
        GameObject smoke = Instantiate(Resources.Load("Prefabs/Smoke") as GameObject, playerTwo.transform.position, Quaternion.identity);
        smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
        smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.sceneLoaded -= SceneLoaded;
        gameResourceSystem.onDeleteResource();
    }

    private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        if (nextScene.name == "Gaming")
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayBGM("GamingBGM", 0.3f);
            ScoreSystem.Instance.ResetScore();
            //TODO test code
            for (int i = 0; i < maxPlayerCount; ++i)
            {
                GameObject gameObject = Instantiate(GameResourceSystem.Instance.playerPrefabs[i], Global.PLAYER_START_POSITIONS[i],Quaternion.identity);
                gameObject.GetComponent<Player>()?.SetProperties(i + 1, Global.PLAYER_TRACE_COLORS[i], "Player" + (i + 1).ToString());
                gameObject.transform.forward = Global.PLAYER_DEFAULT_FORWARD[i];
                if (players.TryGetValue(i + 1, out GameObject value) == false)
                {
                    players.Add(i + 1, gameObject);
                }
            }

            ScoreItemManager.Instance.Init();
            DropPointManager.Instance.Init();
            InputManager.Instance.Init();
        }
        else
        {
            playerOne = null;
            playerTwo = null;
            _player1Timer = null;
            _player2Timer = null;
            players.Clear();
            //GameObject switchSceneInputManager = Instantiate(Resources.Load("Prefabs/SwitchSceneManager") as GameObject);
        }
    }

}
