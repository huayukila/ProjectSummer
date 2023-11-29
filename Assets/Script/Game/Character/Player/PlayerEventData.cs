using UnityEngine;
using Gaming.PowerUp;

public struct AddScoreEvent
{
    public int playerID;
    public int silkCount;
}

public struct PickSilkEvent
{

}
public struct SpawnSilkEvent
{
    public GameObject silk;
}
public struct DropSilkEvent
{
    public int ID;
    public Vector3 pos;
}

public struct PlayerRespawnEvent
{
    public int ID;
}

public struct SilkCapturedEvent
{
    public int ID;
    public Vector3[] positions;
}

