using Character;
using Mirror;
using UnityEditor;
using UnityEngine;

public struct SendInitializedPlayerEvent
{
    public Player Player;
}
public class GamePlayer : View
{
    [SyncVar] public int playerIndex;
    private PlayerInterfaceContainer _playerInterfaceContainer;
    private DropPointControl _dropPointControl;

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Start");
        {
            if(TryGetComponent(out IPlayerInterfaceContainer container))
            {
                _playerInterfaceContainer = container.GetContainer();
            }
            else
            {
                Debug.LogError("Can't Get Player Interface Container:(" + name + ")");
            }
        }
        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            RpcRespawnPlayer();

        }).UnregisterWhenGameObjectDestroyed(gameObject);

        CmdInitItemSystem();


        // dropPointController
        {
            _dropPointControl = GetComponent<DropPointControl>();
        }

        // Input Device
        {
            DeviceSetting.Init();
        }

        _playerInterfaceContainer.GetInterface<IPlayerInfo>().SetInfo(playerIndex,Global.PLAYER_TRACE_COLORS[playerIndex-1]);
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
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.C))
        {
            (NetWorkRoomManagerExt.singleton as IRoomManager).ExitToOffline();
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
    }
    
    
    private void LateUpdate()
    {
        if (!isLocalPlayer)
            return;
    }

    /// <summary>
    /// 衝突があったとき処理する
    /// </summary>
    /// <param name="collision"></param>
    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
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
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag.Contains("DropPoint"))
        {
            // 自分のDropPoint以外のDropPointに当たったら
            if (other.gameObject.tag.Contains(_playerInterfaceContainer.GetInterface<IPlayerInfo>().ID.ToString()) == false)
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

    [ClientRpc]
    private void RpcRespawnPlayer()
    {
        Timer spawnTimer = new Timer(Time.time,Global.RESPAWN_TIME,
            () =>
            {
                _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Respawn);
            });
        spawnTimer.StartTimer(this);
        Camera.main.GetComponent<ICameraController>()?.StopLockOn();
    }
    public void SetPlayerInterface(PlayerInterfaceContainer container)
    {
        _playerInterfaceContainer = container;
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
    public void CmdOnNetworkObjectDestroy(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    [Command]
    public void CmdInitItemSystem()
    {
        GetSystem<IItemSystem>().InitItemSystem();
    }

    [Command]
    public void CmdDropSilkEvent(DropSilkEvent dropSilkEvent)
    {
        TypeEventSystem.Instance.Send(dropSilkEvent);
    }

    [Command]
    public void CmdOnInstantiateDropPoint(Vector3 pos)
    {
        GameObject dropPoint = Instantiate(_dropPointControl.DropPointPrefab,pos,Quaternion.identity);
        SpawnNetworkObj(dropPoint);
        _dropPointControl.RpcAddDropPoint(dropPoint);
    }

    [Command]
    public void CmdOnDestroyDropPoint(GameObject abandonDropPoint)
    {
        _dropPointControl.RemovePoint(abandonDropPoint);
        OnDestroyNetworkObj(abandonDropPoint);
    }

    [Command]
    public void CmdOnClearAllDropPoints()
    {
        _dropPointControl.RpcClearDropPoints();
    }
    private void OnDestroyNetworkObj(GameObject abandonedObj)
    {
        NetworkServer.Destroy(abandonedObj);
    }

    private void SpawnNetworkObj(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }
}