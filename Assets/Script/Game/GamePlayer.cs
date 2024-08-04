using System;
using WSV.Character;
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
  
    private void OnNameChanged(string _Old,string _New)
    {
        name = _playerName;
    }
    private PlayerInterfaceContainer _playerInterfaceContainer;     // �v���C���[�̃C���^�[�t�F�[�X�R���e�i

    private INetworkAnimationProcess _networkAnimationProcess;      // �A�j���[�V�����Đ�(�l�b�g���[�N��p)

    private DropPointControl _dropPointControl;                     // �K���̓����蔻��p�I�u�W�F�N�g���Ǘ�����R���g���[���[

    private IPlayerMainLogic _playerMainLogic;                      // �v���C���[�̃��C�����W�b�N(Update,FixedUpdate)

    public override void OnStartLocalPlayer()
    {
        // �T�[�o�[��̃v���C���[�����������Ă���
        CmdInitializeGamePlayer();
      
        _playerName = "Player" + playerIndex.ToString();

        _playerMainLogic = GetComponent<IPlayerMainLogic>();

        // �J��������������
        {
            Camera mainCam = Camera.main;
            mainCam.orthographicSize = 35f;
            CameraControl cameraCtrl = mainCam.AddComponent<CameraControl>();
            cameraCtrl.LockOnTarget(gameObject);
            
        }
        // Input Device
        {
            DeviceSetting.Init();
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

        if(_dropPointControl != null)
            if(_dropPointControl.enabled)
                _dropPointControl.DropNewPoint();
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
        RpcSendMessageToAllClient($"{name} oncollision function called");
        _dropPointControl.ClearAllDropPoints();
        // �Փ˂����玀�S��Ԃɐݒ肷��
        RpcCallPlayerCommand(EPlayerCommand.Dead);

        NetworkIdentity selfConnection = GetComponent<NetworkIdentity>();
        TargetResetVelocity(selfConnection.connectionToClient);

    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Contains("DropPoint"))
        {
            // ������DropPoint�ȊO��DropPoint�ɓ���������
            if (other.tag.Contains(_playerInterfaceContainer.GetInterface<IPlayerInfo>().ID.ToString()) == false)
            {
                RpcSendMessageToAllClient($"{name} ontrigger function called");
                _dropPointControl.CmdOnClearAllDropPoints();
                // ���S��Ԃɐݒ肷��
                RpcCallPlayerCommand(EPlayerCommand.Dead);

                NetworkIdentity selfConnection = GetComponent<NetworkIdentity>();
                TargetResetVelocity(selfConnection.connectionToClient);
            }
        }
    }

    [Command]
    public void CmdSendMessage(string message)
    {
        RpcSendMessageToAllClient(message);
    }

    [ClientRpc]
    private void RpcSendMessageToAllClient(string message)
    {
        Debug.Log(message);
    }


    [ClientRpc]
    private void RpcCallPlayerCommand(EPlayerCommand command)
    {
        IPlayerCommand playerCommand = _playerInterfaceContainer.GetInterface<IPlayerCommand>();
        playerCommand.CallPlayerCommand(command);
    }

    [TargetRpc]
    private void TargetResetVelocity(NetworkConnection conn)
    {
        GetComponent<Player>().ResetRigidbody();
    }
    

    [Command]
    public void CmdRespawnPlayer()
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
    public void CmdInstantiateDropPoint(Vector3 pos,string dropPointTag)
    {
        GameObject dropPoint = Instantiate(GameResourceSystem.Instance.GetPrefabResource("DropPoint"),pos,Quaternion.identity);
        dropPoint.tag = dropPointTag;
        dropPoint.name = dropPointTag;
        NetworkServer.Spawn(dropPoint);
         _dropPointControl.RpcAddDropPoint(dropPoint);
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


    #region Complete Area
    [Command]
    private void CmdInitializeGamePlayer()
    {
        RpcInit();
    }

    [ClientRpc]
    private void RpcInit()
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

        {
            _dropPointControl = GetComponent<DropPointControl>();
        }

        SpriteRenderer playerImage = GetComponentInChildren<SpriteRenderer>();
        playerImage.sprite = GameResourceSystem.Instance.GetCharacterImage("Player" + playerIndex.ToString());

        IPlayerInfo playerInfo = _playerInterfaceContainer.GetInterface<IPlayerInfo>();
        playerInfo.SetInfo(playerIndex,Global.PLAYER_TRACE_COLORS[playerIndex-1]);
      
        _dropPointControl.InitDropPointCtrl(_playerInterfaceContainer.GetInterface<IPlayerInfo>());

        { 
            SendInitializedPlayerEvent playerEvent = new SendInitializedPlayerEvent
            {
                Player = GetComponent<Player>()
            };
            TypeEventSystem.Instance.Send(playerEvent);
        }
    }

    [Command]
    public void CmdPaintArea(PaintAreaEvent paintAreaEvent)
    {
        IPaintSystem paintSystem = GetSystem<IPaintSystem>();
        if(paintSystem != null)
        {
            paintSystem.Paint(paintAreaEvent.Verts,paintAreaEvent.PlayerID,paintAreaEvent.PlayerAreaColor);
        }
    }
    #endregion
    // Complete Area(Don't touch!!!!!!!!!!!!!!!)

}