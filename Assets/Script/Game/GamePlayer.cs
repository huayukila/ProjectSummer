using Mirror;
using Mirror.Examples.MultipleMatch;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GamePlayer : View
{
    [SyncVar] public int playerIndex;
    private PlayerInterfaceContainer _playerInterfaceContainer;

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
    public void CmdOnItemSpawn(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        Debug.Log($"start spawn {prefab.name}");
        GameObject obj = Instantiate(prefab,position,rotation);
        NetworkServer.Spawn(obj);
         Debug.Log($"end spawn {prefab.name}");
    }

    [Command]
    public void CmdInitItemSystem()
    {
        GetSystem<IItemSystem>().InitItemSystem();
    }
}