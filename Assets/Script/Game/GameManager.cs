using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Character;
using Gaming;

public interface IPlayerSetStatus
{
    
}
public struct SpiderPlayer
{
    public GameObject player;
    public int ID;
    public Timer spawnTimer;
    public GameObject camera;
}
public class GameManager : Singleton<GameManager>
{
    private static int maxPlayerCount = 2;
    private Dictionary<int, SpiderPlayer> spiderPlayers;
    private ItemSystem itemSystem;
    private GameResourceSystem gameResourceSystem;
    private DropPointSystem dropPointSystem;

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
        CheckRespawn();
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
        dropPointSystem.Deinit();
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
            ICameraCtrl cameraCtrl = spiderPlayer.camera.GetComponent<ICameraCtrl>();
            cameraCtrl.StopLockOn();
            spiderPlayers[ID] = spiderPlayer;
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
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayBGM("GamingBGM", 0.3f);
            ScoreSystem.Instance.ResetScore();
            for (int i = 0; i < maxPlayerCount; ++i)
            {
                GameObject playerPrefab = gameResourceSystem.GetPrefabResource("Player" + (i + 1).ToString());
                if(playerPrefab != null)
                {
                    if (!spiderPlayers.ContainsKey(i + 1))
                    {
                        GameObject player = Instantiate(playerPrefab, Global.PLAYER_START_POSITIONS[i], Quaternion.identity);
                        player.GetComponent<Player>()?.SetProperties(i + 1, Global.PLAYER_TRACE_COLORS[i]);
                        player.transform.forward = Global.PLAYER_DEFAULT_FORWARD[i];
                        player.GetComponent<DropPointControl>().Init();
                        GameObject camera = new GameObject("Player" + (i + 1).ToString() + "Camera");
                        camera.transform.rotation = Quaternion.LookRotation(Vector3.down,Vector3.forward);
                        Camera cam = camera.AddComponent<Camera>();
                        cam.rect = new Rect((float)i / (float)maxPlayerCount, 0.0f, 1.0f / maxPlayerCount, 1.0f);
                        cam.orthographic = true;
                        cam.orthographicSize = 36.0f;
                        cam.depth = 1.0f;
                        CameraControl camCtrl = camera.AddComponent<CameraControl>();
                        camCtrl.LockOnTarget(player);
                        SpiderPlayer spiderPlayer = new SpiderPlayer
                        {
                            ID = i + 1,
                            player = player,
                            spawnTimer = null,
                            camera = camera
                        };
                        spiderPlayers.Add(i + 1, spiderPlayer);
                        dropPointSystem.InitPlayerDropPointGroup(i + 1);
                    }
                }
                else
                {
                    Debug.LogError("Can't find Resource of Player" + (i + 1).ToString());
                }
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

}
