#define isNotDEBUG

using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    void RegisterAllSystem() // すべてのシステムを登録する
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
            ? Instantiate(table.PlayerPrefabs[index],
                startPos.position, startPos.rotation)
            : Instantiate(table.PlayerPrefabs[index],
                Vector3.zero, Quaternion.identity);

        gamePlayer.GetComponent<GamePlayer>().playerIndex = index;
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
}