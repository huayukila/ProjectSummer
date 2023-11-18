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
    public Player.Status playerStatus;
    public int mID;

    public Timer spawnTimer;
}
public class GameManager : Singleton<GameManager>
{
    private static int maxPlayerCount = 2;
    private Dictionary<int, SpiderPlayer> spiderPlayers;
    private Dictionary<int, GameObject> players;
    public GameObject playerOne;
    public GameObject playerTwo;
    private ItemSystem itemSystem;
    private GameResourceSystem gameResourceSystem;
    private DropPointSystem dropPointSystem;

    private Timer _player1Timer;        // �v���C���[1�̑ҋ@�^�C�}�[
    private Timer _player2Timer;        // �v���C���[2�̑ҋ@�^�C�}�[

    protected override void Awake()
    {
        base.Awake();
        //�e�V�X�e���̎��ቻ�Ə�����
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
        //�V�[���̈ڍs���߂���
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
        //�e�V�X�e����update
        //�V�[���̈ڍs�Ȃ�
        RespawnCheck();
    }

    //�V�[���̈ڍs
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
        //TODO test
        players = new Dictionary<int, GameObject>();
        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            RespawnPlayer(e.player);

        }).UnregisterWhenGameObjectDestroyed(gameObject);

    }
    /// <summary>
    /// �v���C���[�̕����^�C�~���O���`�F�b�N����
    /// </summary>
    private void RespawnCheck()
    {
        // player1�̃^�C�}�[�̑ҋ@���Ԃ��I�������
        if (_player1Timer != null)
        {
            if(_player1Timer.IsTimerFinished())
            {
                _player1Timer = null;
            }

        }
        // player2�̃^�C�}�[�̑ҋ@���Ԃ��I�������
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
            // �V�����^�C�}�[�𐶐�����
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
            // �V�����^�C�}�[�𐶐�����
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
    /// �v���C���[1�𕜊�������
    /// </summary>
    private void RespawnPlayer1()
    {
        playerOne.transform.position = Global.PLAYER1_START_POSITION;
        playerOne.transform.forward = Vector3.right;
        playerOne.GetComponent<Player>().SetStatus(Player.Status.Fine);
        playerOne.GetComponentInChildren<TrailRenderer>().enabled = true;
        playerOne.GetComponent<DropPointControl>().enabled = true;
        playerOne.GetComponent<Collider>().enabled = true;
        GameObject smoke = Instantiate(gameResourceSystem.GetPrefabResource("Smoke"), playerOne.transform.position, Quaternion.identity);
        smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
        smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
    }

    /// <summary>
    /// �v���C���[2�𕜊�������
    /// </summary>
    private void RespawnPlayer2()
    {
        playerTwo.transform.position = Global.PLAYER2_START_POSITION;
        playerTwo.transform.forward = Vector3.left;
        playerTwo.GetComponent<Player>().SetStatus(Player.Status.Fine);
        playerTwo.GetComponentInChildren<TrailRenderer>().enabled = true;
        playerTwo.GetComponent<DropPointControl>().enabled = true;
        playerTwo.GetComponent<Collider>().enabled = true;
        GameObject smoke = Instantiate(gameResourceSystem.GetPrefabResource("Smoke"), playerTwo.transform.position, Quaternion.identity);
        smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
        smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
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
            //TODO test code
            for (int i = 0; i < maxPlayerCount; ++i)
            {
                GameObject playerPrefab = gameResourceSystem.GetPrefabResource("Player" + (i + 1).ToString());
                if(playerPrefab != null)
                {
                    GameObject gameObject = Instantiate(playerPrefab, Global.PLAYER_START_POSITIONS[i], Quaternion.identity);
                    gameObject.GetComponent<Player>()?.SetProperties(i + 1, Global.PLAYER_TRACE_COLORS[i]);
                    gameObject.transform.forward = Global.PLAYER_DEFAULT_FORWARD[i];
                    gameObject.GetComponent<DropPointControl>().Init();
                    if (players.TryGetValue(i + 1, out GameObject value) == false)
                    {
                        players.Add(i + 1, gameObject);
                        dropPointSystem.InitPlayerDropPointGroup(i + 1);
                    }
                }
                else
                {
                    Debug.LogError("Can't find Resource of Player" + (i + 1).ToString());
                }
            }

            //TODO refactorying
            playerOne = players[1];
            playerTwo = players[2];

            ScoreItemManager.Instance.Init();
            InputSetting.Init();
        }
        else
        {
            playerOne = null;
            playerTwo = null;
            _player1Timer = null;
            _player2Timer = null;
            players.Clear();
        }
    }

}
