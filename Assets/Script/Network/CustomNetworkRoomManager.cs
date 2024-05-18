using Mirror;
using Mirror.Discovery;

public class CustomNetworkRoomManager : NetworkRoomManager
{
    protected NetworkDiscovery networkDiscovery;

    public override void Start()
    {
        base.Start();
        networkDiscovery = GetComponent<NetworkDiscovery>();
    }
}