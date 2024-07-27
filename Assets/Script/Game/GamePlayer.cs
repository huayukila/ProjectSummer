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
    public string _playerName;

    private PlayerInterfaceContainer _playerInterfaceContainer;     // �v���C���[�̃C���^�[�t�F�[�X�R���e�i

    private INetworkAnimationProcess _networkAnimationProcess;      // �A�j���[�V�����Đ�(�l�b�g���[�N��p)

    private DropPointControl _dropPointControl;                     // �K���̓����蔻��p�I�u�W�F�N�g���Ǘ�����R���g���[���[

    private IPlayerMainLogic _playerMainLogic;                      // �v���C���[�̃��C�����W�b�N(Update,FixedUpdate)

    public override void OnStartLocalPlayer()
    {
        // �C���^�[�t�F�[�X�R���e�i�擾
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

        // �v���C���[����������
        {
            Camera mainCam = Camera.main;
            mainCam.orthographicSize = 35f;
            CameraControl cameraCtrl = mainCam.AddComponent<CameraControl>();
            cameraCtrl.LockOnTarget(gameObject);
            
            SpriteRenderer playerImage = GetComponentInChildren<SpriteRenderer>();
            playerImage.sprite = GameResourceSystem.Instance.GetCharacterImage("Player" + playerIndex.ToString());

        }

        _playerMainLogic = GetComponent<IPlayerMainLogic>();

        #region not call on client 
            CmdOnChangeName();
            CmdOnInitDropPointCtrl();
        #endregion

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
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Contains("DropPoint"))
        {
            // ������DropPoint�ȊO��DropPoint�ɓ���������
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
                // ���S��Ԃɐݒ肷��
                _playerInterfaceContainer.GetInterface<IPlayerCommand>().CallPlayerCommand(EPlayerCommand.Dead);
            }
        }
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

    private void OnNameChanged(string _Old,string _New)
    {
        
    }
    [Command]
    public void CmdOnChangeName()
    {
        RpcChangeName();
        Debug.Log(name);
    }

    [ClientRpc]
    private void RpcChangeName()
    {
        name = "Player" + playerIndex.ToString();
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
    public void CmdDropSilkEvent(DropSilkEvent dropSilkEvent)
    {

        TypeEventSystem.Instance.Send(dropSilkEvent);
    }
    [Command]
    public void CmdOnInitDropPointCtrl()
    {
        _dropPointControl.RpcDropPointInit();
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

    [ServerCallback]
    private void OnDestroyNetworkObj(GameObject abandonedObj)
    {

        if(abandonedObj == null)
            return;

        NetworkServer.Destroy(abandonedObj);
    }

    private void SpawnNetworkObj(GameObject obj)
    {
        NetworkServer.Spawn(obj,gameObject);
    }

    [ClientRpc]
    private void RpcOnCollisionEnter()
    {
       
    }
    [ClientRpc]
    private void RpcOnTriggerEnter(GameObject collisionObj)
    {
        
    }

}