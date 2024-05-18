#define isNotDEBUG

using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;

// ���[���Ǘ��̃C���^�[�t�F�[�X
interface IRoomManager
{
    void HostGame(); // �Q�[�����z�X�g����
    void Connect(); // �ڑ�����

    void ReturnToRoom();
    void ExitToOffline();

    Transform GetRespawnPosition(int index);
}

// �l�b�g���[�N���[���}�l�[�W���g���N���X
public class NetWorkRoomManagerExt : CustomNetworkRoomManager, IRoomManager
{
    private EasyFramework _framework;

    void RegisterAllSystem() // ���ׂẴV�X�e����o�^����
    {
        _framework.RegisterSystem<IPaintSystem>(new PaintSystem());
        _framework.RegisterSystem<ITestSystem>(new TestSystem());
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
            ? Instantiate(table.PlayerPrefabs[index],
                startPos.position, startPos.rotation)
            : Instantiate(table.PlayerPrefabs[index],
                Vector3.zero, Quaternion.identity);

        gamePlayer.GetComponent<GamePlayer>().playerIndex = index;
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

    #endregion
}