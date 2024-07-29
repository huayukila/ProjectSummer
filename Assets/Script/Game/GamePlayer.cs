using Character;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;


public class SendInitializedPlayerEvent
{
    public Player Player;
}
public class GamePlayer : View
{
    [SyncVar] public int playerIndex;
    [SyncVar(hook = nameof(OnNameChanged))]
    public string _playerName = string.Empty;

    private PlayerInterfaceContainer _playerInterfaceContainer;     // プレイヤーのインターフェースコンテナ

    private INetworkAnimationProcess _networkAnimationProcess;      // アニメーション再生(ネットワーク専用)

    private DropPointControl _dropPointControl;                     // 尻尾の当たり判定用オブジェクトを管理するコントローラー

    private IPlayerMainLogic _playerMainLogic;                      // プレイヤーのメインロジック(Update,FixedUpdate)

    public override void OnStartLocalPlayer()
    {
        // インターフェースコンテナ取得
        if(TryGetComponent(out IPlayerInterfaceContainer container))
        {
            _playerInterfaceContainer = container.GetContainer();
        }
        else
        {
            Debug.LogError("Can't Get Player Interface Container:(" + name + ")");
        }

        _networkAnimationProcess = GetComponent<INetworkAnimationProcess>();

        IPlayerInfo playerInfo = _playerInterfaceContainer.GetInterface<IPlayerInfo>();
        playerInfo.SetInfo(playerIndex,Global.PLAYER_TRACE_COLORS[playerIndex-1]);

        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            e.Player.GetComponent<GamePlayer>().CmdRespawnPlayer();

        }).UnregisterWhenGameObjectDestroyed(gameObject);

        // dropPointController
        {
            _dropPointControl = GetComponent<DropPointControl>();
        }

        // Input Device
        {
            DeviceSetting.Init();
        }

        // プレイヤー情報を初期化
        {
            Camera mainCam = Camera.main;
            mainCam.orthographicSize = 35f;
            CameraControl cameraCtrl = mainCam.AddComponent<CameraControl>();
            cameraCtrl.LockOnTarget(gameObject);
            
            SpriteRenderer playerImage = GetComponentInChildren<SpriteRenderer>();
            playerImage.sprite = GameResourceSystem.Instance.GetCharacterImage("Player" + playerIndex.ToString());

        }

        _playerMainLogic = GetComponent<IPlayerMainLogic>();

        CmdChangeName("Player" + playerIndex.ToString());

        _dropPointControl.InitDropPointCtrl(_playerInterfaceContainer.GetInterface<IPlayerInfo>());

        { 
            SendInitializedPlayerEvent playerEvent = new SendInitializedPlayerEvent
            {
                Player = GetComponent<Player>()
            };

            TypeEventSystem.Instance.Send(playerEvent);
        }


    }

    private void Update()
    {
        if (!isLocalPlayer) 
            return;

        _playerMainLogic.Tick();

        if(Input.GetKeyDown(KeyCode.C))
        {
            (NetWorkRoomManagerExt.singleton as NetWorkRoomManagerExt).ExitToOffline();
        }

    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        _playerMainLogic.FixedTick();
    }
    
    
    private void LateUpdate()
    {
        if (!isLocalPlayer)
            return;

        if(_networkAnimationProcess != null)
        {
            if(!_networkAnimationProcess.IsStopped)
            {
                CmdUpdatePlayerAnimation();
            }               
        }
    }

    /// <summary>
    /// 衝突があったとき処理する
    /// </summary>
    /// <param name="collision"></param>  
    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        RpcSendMessageToAllClient();
        // 衝突したら死亡状態に設定する
        CallPlayerCommand(EPlayerCommand.Dead);

    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Contains("DropPoint"))
        {
            // 自分のDropPoint以外のDropPointに当たったら
            if (other.tag.Contains(_playerInterfaceContainer.GetInterface<IPlayerInfo>().ID.ToString()) == false)
            {
                if (_playerInterfaceContainer.GetInterface<IPlayerInfo>().SilkCount > 0)
                {
                    DropSilkEvent dropSilkEvent = new DropSilkEvent()
                    {
                        pos = transform.position,
                    };
                    TypeEventSystem.Instance.Send(dropSilkEvent);
                }
                // 死亡状態に設定する
                CallPlayerCommand(EPlayerCommand.Dead);
            }
        }
    }

    [ClientRpc]
    private void RpcSendMessageToAllClient()
    {
        Debug.Log($"{name} ontrigger/oncollision function called");
    }


    private void CallPlayerCommand(EPlayerCommand command)
    {
        _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(command);
    }

    [Command]
    private void CmdRespawnPlayer()
    {
        Timer spawnTimer = new Timer(Time.time,Global.RESPAWN_TIME,
            () =>
            {
                _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Respawn);
            });
        spawnTimer.StartTimer(this);
        Camera.main.GetComponent<ICameraController>()?.StopLockOn();
    }

    [Command]
    private void CmdChangeName(string newName)
    {
        _playerName = newName;
    }

    private void OnNameChanged(string _Old,string _New)
    {
        name = _playerName;
    }

    [ClientRpc]
    private void RpcInitDropPointCtrl()
    {
        //_dropPointControl.InitDropPointCtrl();
    }

    [Command]
    public void CmdOnItemSpawn(GameObject player)
    {
        IPlayer2ItemSystem player2ItemSystem;
        if(player.TryGetComponent<IPlayer2ItemSystem>(out player2ItemSystem))
        {
            player2ItemSystem.UseItem(player.GetComponent<Player>());
        }
        //NetworkServer.Spawn(obj);
    }

    [Command]
    public void CmdOnInstantiateDropPoint(Vector3 pos,string dropPointTag)
    {

        GameObject dropPoint = Instantiate(GameResourceSystem.Instance.GetPrefabResource("DropPoint"),pos,Quaternion.identity);
        dropPoint.tag = dropPointTag;
        dropPoint.name = dropPointTag;
        dropPoint.GetComponent<DropPoint>().SetDestroyCallback(_dropPointControl.RpcRemoveDropPoint);
        NetworkServer.Spawn(dropPoint);

         _dropPointControl.RpcAddDropPoint(dropPoint);

    }

    [Command]
    public void CmdOnClearAllDropPoints()
    {
        // TODO
        //_dropPointControl.ClearAllDropPoints();
    }

    [Command]
    public void CmdSpawnDeadAnimation(Vector3 pos)
    {

        GameObject explosion = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Explosion"), pos, Quaternion.identity);
        explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        
        if(_networkAnimationProcess != null)
        {
            _networkAnimationProcess.SetAnimationType(AnimType.Respawn);
        }
    }

    [Command]
    private void CmdUpdatePlayerAnimation()
    {
        _networkAnimationProcess.RpcUpdateAnimation();
    }

}