using Character;
using Mirror;
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

    private PlayerInterfaceContainer _playerInterfaceContainer;

    private INetworkAnimationProcess _networkAnimationProcess;
    private DropPointControl _dropPointControl;

    private IPlayerMainLogic _playerMainLogic;

    public override void OnStartLocalPlayer()
    {

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

        _networkAnimationProcess = GetComponent<INetworkAnimationProcess>();

        _playerName = "Player" + playerIndex.ToString();
        IPlayerInfo playerInfo = _playerInterfaceContainer.GetInterface<IPlayerInfo>();
        playerInfo?.SetInfo(playerIndex,Global.PLAYER_TRACE_COLORS[playerIndex-1]);

        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            e.Player.GetComponent<GamePlayer>().RespawnPlayer();

        }).UnregisterWhenGameObjectDestroyed(gameObject);

        // dropPointController
        {
            _dropPointControl = GetComponent<DropPointControl>();
            _dropPointControl.SetInfo(playerIndex);
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

    private void OnNameChanged(string _Old, string _New)
    {
        name = _playerName;
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

}