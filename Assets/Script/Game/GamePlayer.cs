using Character;
using Mirror;
using Mirror.Examples.MultipleMatch;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

public class SendInitializedPlayerEvent
{
    public Player Player;
}
public class GamePlayer : View
{
    [SyncVar] public int playerIndex;

    [SyncVar(hook = nameof(OnNameChanged))] 
    private string _playerName;

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

        // local名前を設定(バグだらけ)
        // name = "Player" + playerIndex.ToString();
        IPlayerInfo playerInfo = _playerInterfaceContainer.GetInterface<IPlayerInfo>();
        playerInfo.SetInfo(playerIndex,Global.PLAYER_TRACE_COLORS[playerIndex-1]);

        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            e.Player.GetComponent<GamePlayer>().RespawnPlayer();

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

        { 
            SendInitializedPlayerEvent playerEvent = new SendInitializedPlayerEvent
                                                    {
                                                        Player = GetComponent<Player>()
                                                    };

            TypeEventSystem.Instance.Send(playerEvent);
        }

    }
    public override void OnStartAuthority()
    {
        CmdOnChangeName();
        CmdOnInitDropPointCtrl();
    }
    private void Update()
    {
        if (!isLocalPlayer) 
            return;

        _playerMainLogic.Tick();

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
        if (!isLocalPlayer)
            return;
        RpcOnCollisionEnter();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer)
            return;
        RpcOnTriggerEnter(other.gameObject);
    }

    private void RespawnPlayer()
    {
        Timer spawnTimer = new Timer(Time.time,Global.RESPAWN_TIME,
            () =>
            {
                _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Respawn);
            });
        spawnTimer.StartTimer(this);
        Camera.main.GetComponent<ICameraController>()?.StopLockOn();
    }

    private void OnNameChanged(string _Old,string _New)
    {
        name = _playerName;
    }

    [Command]
    public void CmdOnChangeName()
    {
        _playerName = "Player" + playerIndex.ToString();
    }

    [Command]
    public void CmdOnItemSpawn(GameObject player)
    {
        if (!isLocalPlayer)
            return;

        IPlayer2ItemSystem player2ItemSystem;
        if(player.TryGetComponent<IPlayer2ItemSystem>(out player2ItemSystem))
        {
            player2ItemSystem.UseItem(player.GetComponent<Player>());
        }
        //NetworkServer.Spawn(obj);
    }


    [Command]
    public void CmdDropSilkEvent(DropSilkEvent dropSilkEvent)
    {
        if (!isLocalPlayer)
            return;

        TypeEventSystem.Instance.Send(dropSilkEvent);
    }
    [Command]
    public void CmdOnInitDropPointCtrl()
    {
        GetComponent<DropPointControl>().RpcDropPointInit();
    }

    [Command]
    public void CmdOnInstantiateDropPoint(Vector3 pos)
    {
        if (!isLocalPlayer)
            return;
        GameObject dropPoint = Instantiate(_dropPointControl.DropPointPrefab,pos,Quaternion.identity);
        SpawnNetworkObj(dropPoint);
        _dropPointControl.RpcAddDropPoint(dropPoint);
    }

    [Command]
    public void CmdOnDestroyDropPoint(GameObject abandonDropPoint)
    {
        if (!isLocalPlayer)
            return;
        _dropPointControl.RemovePoint(abandonDropPoint);
        OnDestroyNetworkObj(abandonDropPoint);
    }

    [Command]
    public void CmdOnClearAllDropPoints()
    {
        if (!isLocalPlayer)
            return;
        _dropPointControl.RpcClearDropPoints();
    }

    [Command]
    public void CmdSpawnDeadAnimation(Vector3 pos)
    {
        if (!isLocalPlayer)
            return;
        GameObject explosion = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Explosion"), pos, Quaternion.identity);
        explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        
        if(_networkAnimationProcess != null)
        {
            _networkAnimationProcess.RpcSetAnimationType(AnimType.Respawn);
        }
    }

    [Command]
    private void CmdUpdatePlayerAnimation()
    {
        if (!isLocalPlayer)
            return;

        _networkAnimationProcess.RpcUpdateAnimation();
    }

    [ServerCallback]
    private void OnDestroyNetworkObj(GameObject abandonedObj)
    {
        if (!isLocalPlayer)
            return;
        if(abandonedObj == null)
            return;

        NetworkServer.Destroy(abandonedObj);
    }

    private void SpawnNetworkObj(GameObject obj)
    {
        if (!isLocalPlayer)
            return;
        NetworkServer.Spawn(obj);
    }

    [ClientRpc]
    private void RpcOnCollisionEnter()
    {
        // 死亡したプレイヤーは金の網を持っていたら
        if (_playerInterfaceContainer.GetInterface<IPlayerInfo>().SilkCount > 0)
        {
            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                pos = transform.position,
            };
            // 金の糸のドロップ場所を設定する
            //HACK EventSystem temporary invalid
            //TypeEventSystem.Instance.Send(dropSilkEvent);
        }
        // 衝突したら死亡状態に設定する
        _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Dead);
        Debug.Log(transform.position);
    }
    [ClientRpc]
    private void RpcOnTriggerEnter(GameObject collisionObj)
    {
        if(collisionObj.tag.Contains("DropPoint"))
        {
            // 自分のDropPoint以外のDropPointに当たったら
            if (collisionObj.tag.Contains(_playerInterfaceContainer.GetInterface<IPlayerInfo>().ID.ToString()) == false)
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
                _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Dead);
            }
        }
    }

}