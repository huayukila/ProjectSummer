using Mirror;

public class View : NetworkBehaviour
{
    protected T GetSystem<T>() where T : class, ISystem
    {
        return (NetWorkRoomManagerExt.singleton as NetWorkRoomManagerExt).
            GetFramework().GetSystem<T>();
    }
}