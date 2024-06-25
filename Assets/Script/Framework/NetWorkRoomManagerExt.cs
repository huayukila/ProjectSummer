#define isNotDEBUG

using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;

using Character;
using Unity.VisualScripting;
using Gaming;
using JetBrains.Annotations;

// ルーム管理のインターフェース
interface IRoomManager
{
    void HostGame(); // ゲームをホストする
    void Connect(); // 接続する

    void ReturnToRoom();
    void ExitToOffline();

    Transform GetRespawnPosition(int index);
}

// ネットワークルームマネージャ拡張クラス
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


    void RegisterAllSystem() // すべてのシステムを登録する
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
        //ゲームデータの初期化
    }

    #region Network

    public override void Awake() // 起動時
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

    public override void Start() // 開始時
    {
        base.Start();
        networkDiscovery.OnServerFound.AddListener(OnDiscoverServer);
        RegisterNetPrefabs();
        SceneManager.LoadScene("Title");

    }

    public void HostGame() // ゲームホスティング
    {
        StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void Connect() // 接続
    {
        networkDiscovery.StartDiscovery();
    }

    //全部準備出来ました
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

    // ゲームプレイヤー作成時、プレイヤーの初期化はここです。
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        //次の生成ポイントゲット
        int index = conn.identity.GetComponent<CustomNetworkRoomPlayer>().index;
        Transform startPos = GetStartPosition();
        var table = Resources.Load<NetWorkPrefabsTable>("NetworkPrefabsTable");
        var gamePlayer = startPos != null
            ? Instantiate(table.PlayerPrefabs[0],
                startPos.position, startPos.rotation)
            : Instantiate(table.PlayerPrefabs[0],
                Vector3.zero, Quaternion.identity);

        // プレイヤー情報を初期化
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

    public override void OnDestroy() // 破棄時
    {
        networkDiscovery.OnServerFound.RemoveAllListeners();
        base.OnDestroy();
    }

    public override void OnRoomServerSceneChanged(string sceneName) // シーン変更時
    {
        if (sceneName == GameplayScene)
        {
        }
    }

    public Transform GetRespawnPosition(int index)
    {
        return startPositions[index];
    }

    #region 内部用

    private void OnDiscoverServer(ServerResponse info) // サーバー発見時
    {
        networkDiscovery.StopDiscovery();
        StartClient(info.uri);
    }

    private void RegisterNetPrefabs() // プレハブ登録
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
    /// プレイヤーの座標を取得する関数
    /// </summary>
    /// <param name="ID">プレイヤーのID</param>
    /// <returns>プレイヤーが存在したらワールド座標を返し、存在しない場合は常にVector3.zeroを返す</returns>
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