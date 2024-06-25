#define isNotDEBUG

using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;

using Character;
using Unity.VisualScripting;
using Gaming;
using JetBrains.Annotations;

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

    
    private struct SpiderPlayer
    {
        public int ID;
        public GameObject player;
        public ICameraController cameraCtrl;
        public PlayerInterfaceContainer playerInterface;
    }
    private SpiderPlayer _spiderPlayer;


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

        _spiderPlayer = new SpiderPlayer();


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

        // �v���C���[����������
        {
            Camera mainCam = Camera.main;
            _spiderPlayer.ID = index + 1;
            _spiderPlayer.player = gamePlayer;
            _spiderPlayer.cameraCtrl = mainCam.AddComponent<CameraControl>();
            _spiderPlayer.cameraCtrl.LockOnTarget(_spiderPlayer.player);

            _spiderPlayer.playerInterface = gamePlayer.GetComponent<IPlayerInterfaceContainer>().GetContainer();
            _spiderPlayer.playerInterface.GetInterface<IPlayerInfo>().SetInfo(index + 1,Global.PLAYER_TRACE_COLORS[index]);

            SpriteRenderer playerImage = _spiderPlayer.player.GetComponentInChildren<SpriteRenderer>();
            playerImage.sprite = GameResourceSystem.Instance.GetCharacterImage("Player" + (index + 1).ToString());

        }

        //TODO need refactoring
        gamePlayer.GetComponent<GamePlayer>().playerIndex = index + 1;
        gamePlayer.GetComponent<GamePlayer>().SetPlayerInterface(_spiderPlayer.playerInterface);

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

    #region Interface
    /// <summary>
    /// �v���C���[�̍��W���擾����֐�
    /// </summary>
    /// <param name="ID">�v���C���[��ID</param>
    /// <returns>�v���C���[�����݂����烏�[���h���W��Ԃ��A���݂��Ȃ��ꍇ�͏��Vector3.zero��Ԃ�</returns>
    public Vector3 GetPlayerPos()
    {
        if(_spiderPlayer.player == null)
            return Vector3.zero;
        return _spiderPlayer.player.transform.position;
    }

    // TODO this method sucks
    public bool IsPlayerDead()
    {
        if(_spiderPlayer.player == null)
            return false;

        return _spiderPlayer.playerInterface.GetInterface<IPlayerState>().IsDead;
    }
    #endregion // Interface
}