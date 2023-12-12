using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Character;
using Gaming;



public class GameManager : Singleton<GameManager>
{
    private struct SpiderPlayer
    {
        public GameObject player;
        public Timer spawnTimer;
        public GameObject camera;
    }
    private static readonly int maxPlayerCount = 2;
    private Dictionary<int, SpiderPlayer> spiderPlayers;
    private ItemSystem itemSystem;
    private GameResourceSystem gameResourceSystem;
    private IDropPointSystem dropPointSystem;

    protected override void Awake()
    {
        base.Awake();
        //各システムの実例化と初期化
        {
            itemSystem = ItemSystem.Instance;
            itemSystem.Init();
        }

        {
            gameResourceSystem = GameResourceSystem.Instance;
            gameResourceSystem.Init();
        }

        {
            dropPointSystem = DropPointSystem.Instance;
            dropPointSystem.Init();
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
                    break;
                case InputDeviceChange.Disconnected:
                    InputSystem.FlushDisconnectedDevices();
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
                    break;
            }
        };

        Cursor.lockState = CursorLockMode.Locked;

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
        // gaming scene process
        if (SceneManager.GetActiveScene().name == "Gaming")
        {
            CheckRespawn();
        }

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
        spiderPlayers = new Dictionary<int, SpiderPlayer>();
        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            RespawnPlayer(e.ID);

        }).UnregisterWhenGameObjectDestroyed(gameObject);

    }
    /// <summary>
    /// プレイヤーの復活タイミングをチェックする
    /// </summary>
    private void CheckRespawn()
    {
        foreach (var player in spiderPlayers.Values) 
        {
            if(player.spawnTimer != null)
            {
                player.spawnTimer.IsTimerFinished();
            }
        }
    }

    private void RespawnPlayer(int ID)
    {

        if(spiderPlayers.ContainsKey(ID))
        {
            
            Timer spawnTimer = new Timer();
            spawnTimer.SetTimer(Global.RESPAWN_TIME,
                 () =>
                 {
                     spiderPlayers[ID].player.GetComponent<Player>()?.StartRespawn();
                     spawnTimer = null;
                 }
                 );
            SpiderPlayer spiderPlayer = spiderPlayers[ID];
            spiderPlayer.spawnTimer = spawnTimer;
            ICameraController cameraCtrl = spiderPlayer.camera.GetComponent<ICameraController>();
            cameraCtrl.StopLockOn();
            spiderPlayers[ID] = spiderPlayer;

        }
    }

    private void SpawnPlayer(int ID)
    {
        GameObject playerPrefab = gameResourceSystem.GetPrefabResource("Player");
        if (playerPrefab != null)
        {
            if (!spiderPlayers.ContainsKey(ID))
            {
                GameObject player = Instantiate(playerPrefab, Global.PLAYER_START_POSITIONS[ID - 1], Quaternion.identity);
                player.GetComponent<Player>()?.SetProperties(ID, Global.PLAYER_TRACE_COLORS[ID - 1]);
                SpriteRenderer playerImage = player.GetComponentInChildren<SpriteRenderer>();
                playerImage.sprite = GameResourceSystem.Instance.GetCharacterImage("Player" + ID.ToString());

                GameObject camera = new GameObject("Player" + (ID).ToString() + "Camera");
                camera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                Camera cam = camera.AddComponent<Camera>();
                cam.rect = new Rect((float)(ID -1) / (float)maxPlayerCount, 0.0f, 1.0f / maxPlayerCount, 1.0f);
                cam.orthographic = true;
                cam.orthographicSize = 54.0f;
                cam.depth = 1.0f;
                cam.backgroundColor = Color.red;
                CameraControl camCtrl = camera.AddComponent<CameraControl>();
                camCtrl.LockOnTarget(player);
                SpiderPlayer spiderPlayer = new SpiderPlayer
                {
                    player = player,
                    spawnTimer = null,
                    camera = camera
                };
                spiderPlayers.Add(ID, spiderPlayer);
                dropPointSystem.InitPlayerDropPointGroup(ID);
            }
        }
        else
        {
            Debug.LogError("Can't find Resource of Player" + ID.ToString());
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        gameResourceSystem.Deinit();
    }

    private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        if (nextScene.name == "Gaming")
        {
            dropPointSystem.Deinit();
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayBGM("GamingBGM", 0.3f);
            ScoreSystem.Instance.ResetScore();
            for (int i = 0; i < maxPlayerCount; ++i)
            {
                SpawnPlayer(i + 1);
            }

            Gaming.PowerUp.GoldenSilkSystem.Instance.Init();
            GoldenSilkManager goldenSilkManager = GoldenSilkManager.Instance;
            DeviceSetting.Init();
        }
        else
        {
            spiderPlayers.Clear();
        }
    }

    #region interface
    /// <summary>
    /// プレイヤーの座標を取得する関数
    /// </summary>
    /// <param name="ID">プレイヤーのID</param>
    /// <returns>プレイヤーが存在したらワールド座標を返し、存在しない場合は常にVector3.zeroを返す</returns>
    public Vector3 GetPlayerPos(int ID)
    {
        Vector3 ret = Vector3.zero;
        if(spiderPlayers.TryGetValue(ID, out SpiderPlayer value) == true)
        {
            ret = value.player.transform.position;
        }
        return ret;
    }

    public bool IsPlayerDead(int ID)
    {
        bool ret = true;
        if(spiderPlayers.TryGetValue(ID, out SpiderPlayer value) == true)
        {
            //TODO インターフェースでやる
            ret = value.player.GetComponent<Player>().IsDead();
        }
        return ret;
    }
    #endregion
}
