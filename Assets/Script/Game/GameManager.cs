using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerOne;
    public GameObject playerTwo;
    public PlayerStatus p1Status { get; private set; }
    public PlayerStatus p2Status { get; private set; }
    private ItemSystem itemSystem;

    Timer _player1Timer;        // プレイヤー1の待機タイマー
    Timer _player2Timer;        // プレイヤー2の待機タイマー
    GameObject _player1Prefab;
    GameObject _player2Prefab;

    protected override void Awake()
    {
        base.Awake();
        //各システムの実例化と初期化
        itemSystem = ItemSystem.Instance;
        itemSystem.Init();
        //シーンの移行命令を受け
        TypeEventSystem.Instance.Register<TitleSceneSwitch>(e => { TitleSceneSwitch(); });
        TypeEventSystem.Instance.Register<MenuSceneSwitch>(e => { MenuSceneSwitch(); });
        TypeEventSystem.Instance.Register<GamingSceneSwitch>(e => { GamingSceneSwitch(); });
        TypeEventSystem.Instance.Register<EndSceneSwitch>(e => { EndSceneSwitch(); });
        TypeEventSystem.Instance.Register<GameOver>(e => { EndSceneSwitch(); });

        _player1Prefab = (GameObject)Resources.Load("Prefabs/Player1");
        _player2Prefab = (GameObject)Resources.Load("Prefabs/Player2");

        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            RespawnPlayer(e.player);

        }).UnregisterWhenGameObjectDestroyed(gameObject);

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

    /// <summary>
    /// プレイヤーの復活タイミングをチェックする
    /// </summary>
    private void RespawnCheck()
    {
        // player1のタイマーの待機時間が終わったら
        if (_player1Timer != null && _player1Timer.IsTimerFinished())
        {
            _player1Timer = null;
        }
        // player2のタイマーの待機時間が終わったら
        if (_player2Timer != null && _player2Timer.IsTimerFinished())
        {
            _player2Timer = null;
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
        playerOne.GetComponent<TrailRenderer>().enabled = true;
        playerOne.GetComponent<DropPointControl>().enabled = true;
    }

    /// <summary>
    /// プレイヤー2を復活させる
    /// </summary>
    private void RespawnPlayer2()
    {
        playerTwo.transform.position = Global.PLAYER2_START_POSITION;
        playerTwo.transform.forward = Vector3.left;
        playerTwo.GetComponent<Player2Control>().SetStatus(PlayerStatus.Fine);
        playerTwo.GetComponent<TrailRenderer>().enabled = true;
        playerTwo.GetComponent<DropPointControl>().enabled = true;
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        if (nextScene.name == "Gaming")
        {
            GameObject player1 = Instantiate(_player1Prefab, Global.PLAYER1_START_POSITION, Quaternion.identity);
            player1.transform.forward = Vector3.right;
            GameObject player2 = Instantiate(_player2Prefab, Global.PLAYER2_START_POSITION, Quaternion.identity);
            player2.transform.forward = Vector3.left;

            GameObject scoreItemManager = new GameObject("ScoreItemManager");
            scoreItemManager.AddComponent<ScoreItemManager>();

            GameObject dropPointManager = new GameObject("DropPointManager");
            dropPointManager.AddComponent<DropPointManager>();

            GameObject inputManager = new GameObject("InputManager");
            inputManager.AddComponent<InputManager>();
        }
        else
        {
            _player1Timer = null;
            _player2Timer = null;
        }
    }

}
