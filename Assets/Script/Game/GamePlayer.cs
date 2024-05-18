using Mirror;
using UnityEngine;

public class GamePlayer : View
{
    [SyncVar] public int playerIndex;

    public override void OnStartLocalPlayer()
    {
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
}