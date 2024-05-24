using System.Collections.Generic;
using System.Net;
using Mirror;
using Mirror.Discovery;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    //
    public Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
}

