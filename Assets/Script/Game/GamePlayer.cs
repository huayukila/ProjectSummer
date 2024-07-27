using Character;
using Mirror;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

public struct SendInitializedPlayerEvent
{
    public Player Player;
}

public class GamePlayer : View
{
    [SyncVar] public int playerIndex;
    private PlayerInterfaceContainer _playerInterfaceContainer;

    private INetworkAnimationProcess _networkAnimationProcess;
    private DropPointControl _dropPointControl;

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Start");
        {
            if (TryGetComponent(out IPlayerInterfaceContainer container))
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
            e.Player.GetComponent<GamePlayer>().RespawnPlayer();
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

        _playerInterfaceContainer.GetInterface<IPlayerInfo>()
            .SetInfo(playerIndex, Global.PLAYER_TRACE_COLORS[playerIndex - 1]);
        {
            SendInitializedPlayerEvent playerEvent = new SendInitializedPlayerEvent
            {
                Player = GetComponent<Player>()
            };

            TypeEventSystem.Instance.Send(playerEvent);
        }

        _networkAnimationProcess = GetComponent<INetworkAnimationProcess>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (_networkAnimationProcess != null)
        {
            if (!_networkAnimationProcess.IsStopped)
            {
                _networkAnimationProcess.RpcUpdateAnimation();
            }
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
    /// �Փ˂��������Ƃ���������
    /// </summary>
    /// <param name="collision"></param>
    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        // ���S�����v���C���[�͋��̖Ԃ������Ă�����
        if (_playerInterfaceContainer.GetInterface<IPlayerInfo>().SilkCount > 0)
        {
            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                pos = transform.position,
            };
            // ���̎��̃h���b�v�ꏊ��ݒ肷��
            //HACK EventSystem temporary invalid
            //TypeEventSystem.Instance.Send(dropSilkEvent);
        }

        // �Փ˂����玀�S��Ԃɐݒ肷��
        _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Dead);
        Debug.Log(transform.position);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("DropPoint"))
        {
            // ������DropPoint�ȊO��DropPoint�ɓ���������
            if (other.gameObject.tag.Contains(_playerInterfaceContainer.GetInterface<IPlayerInfo>().ID.ToString()) ==
                false)
            {
                if (_playerInterfaceContainer.GetInterface<IPlayerInfo>().SilkCount > 0)
                {
                    DropSilkEvent dropSilkEvent = new DropSilkEvent()
                    {
                        pos = transform.position,
                    };
                    TypeEventSystem.Instance.Send(dropSilkEvent);
                }

                // ���S��Ԃɐݒ肷��
                _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Dead);
            }
        }
    }

    private void RespawnPlayer()
    {
        Timer spawnTimer = new Timer(Time.time, Global.RESPAWN_TIME,
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
        if (player.TryGetComponent<IPlayer2ItemSystem>(out player2ItemSystem))
        {
            player2ItemSystem.UseItem(player.GetComponent<Player>());
        }
        //NetworkServer.Spawn(obj);
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
        GameObject dropPoint = Instantiate(_dropPointControl.DropPointPrefab, pos, Quaternion.identity);
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

    [Command]
    public void CmdSpawnDeadAnimation(Vector3 pos)
    {
        GameObject explosion = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Explosion"), pos,
            Quaternion.identity);
        explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        if (_networkAnimationProcess != null)
        {
            _networkAnimationProcess.RpcSetAnimationType(AnimType.Respawn);
        }
    }

    private void OnDestroyNetworkObj(GameObject abandonedObj)
    {
        if (abandonedObj == null)
            return;

        NetworkServer.Destroy(abandonedObj);
    }

    private void SpawnNetworkObj(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }
}