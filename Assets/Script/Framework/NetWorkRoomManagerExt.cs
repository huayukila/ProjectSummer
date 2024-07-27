#define isNotDEBUG

using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;

using Unity.VisualScripting;
using UnityEngine.InputSystem;


// ���[���Ǘ��̃C���^�[�t�F�[�X
interface IRoomManager
{
    void HostGame(); // �Q�[�����z�X�g����
    void Connect(); // �ڑ�����

    void ReturnToRoom();
    void ExitToOffline();

    Transform GetRespawnPosition(int index);
}

public static class TempFunc
{
    public static void DetectDevice()
    {
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

    }
}

// �l�b�g���[�N���[���}�l�[�W���g���N���X
public class NetWorkRoomManagerExt : CustomNetworkRoomManager, IRoomManager
{
    private EasyFramework _framework;

    void RegisterAllSystem() // ���ׂẴV�X�e����o�^����
    {
        _framework.RegisterSystem<IPaintSystem>(new PaintSystem());
        _framework.RegisterSystem<ITestSystem>(new TestSystem());
        _framework.RegisterSystem<IItemSystem>(new ItemSystem());
    }

    public EasyFramework GetFramework()
    {
        return _framework;
    }

    void ResetGame()
    {
        //�Q�[���f�[�^�̏�����
    }

    #region Network

    public override void Awake() // �N����
    {
        base.Awake();
        if (_framework == null)
        {
            _framework = new EasyFramework();
        }

        RegisterAllSystem();
        _framework.FrameworkInit();
        

        TempFunc.DetectDevice();

    }

    public override void Start() // �J�n��
    {
        base.Start();
        networkDiscovery.OnServerFound.AddListener(OnDiscoverServer);
        RegisterNetPrefabs();
        SceneManager.LoadScene("Title");
        
    }

    public void HostGame() // �Q�[���z�X�e�B���O
    {
        StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void Connect() // �ڑ�
    {
        networkDiscovery.StartDiscovery();
    }

    //�S�������o���܂���
    public override void OnRoomServerPlayersReady()
    {
        if (Utils.IsSceneActive(RoomScene) && NetworkServer.active)
        {
            ServerChangeScene(GameplayScene);
        }
    }

    public void ReturnToRoom()
    {
        ResetGame();
        ServerChangeScene(RoomScene);
    }

    public void ExitToOffline()
    {
        ResetGame();
        if (!NetworkServer.active)
        {
            StopClient();
        }
        else
        {
            StopHost();
        }

        networkDiscovery.StopDiscovery();
    }

    // �Q�[���v���C���[�쐬���A�v���C���[�̏������͂����ł��B
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        //���̐����|�C���g�Q�b�g
        int index = conn.identity.GetComponent<CustomNetworkRoomPlayer>().index;
        Transform startPos = GetStartPosition();
        var table = Resources.Load<NetWorkPrefabsTable>("NetworkPrefabsTable");
        var gamePlayer = startPos != null
            ? Instantiate(table.PlayerPrefabs[0],
                startPos.position, startPos.rotation)
            : Instantiate(table.PlayerPrefabs[0],
                Vector3.zero, Quaternion.identity);

        // GamePlayer��������
        GamePlayer networkPlayer = gamePlayer.GetComponent<GamePlayer>();
        if(networkPlayer != null)
        {
            networkPlayer.playerIndex = index + 1;
        }

        Resources.UnloadAsset(table);
        return gamePlayer;
    }

    public override void OnDestroy() // �j����
    {
        networkDiscovery.OnServerFound.RemoveAllListeners();
        base.OnDestroy();
    }

    public override void OnRoomServerSceneChanged(string sceneName) // �V�[���ύX��
    {
        if (sceneName == GameplayScene)
        {
        }
    }

    public Transform GetRespawnPosition(int index)
    {
        return startPositions[index];
    }

    #region �����p

    private void OnDiscoverServer(ServerResponse info) // �T�[�o�[������
    {
        networkDiscovery.StopDiscovery();
        StartClient(info.uri);
    }

    private void RegisterNetPrefabs() // �v���n�u�o�^
    {
        NetWorkPrefabsTable table = Resources.Load<NetWorkPrefabsTable>("NetworkPrefabsTable");
        foreach (var obj in table.Prefabs)
        {
            spawnPrefabs.Add(obj);
        }

        foreach (var player in table.PlayerPrefabs)
        {
            spawnPrefabs.Add(player);
        }
    }

    #endregion
    // �����p

    #endregion
    // Network

   
}