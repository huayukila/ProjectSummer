using UnityEngine;

public struct AddScoreEvent
{
    public int playerID;
    public int silkCount;
}

public struct PickSilkEvent
{

}
public struct DropSilkEvent
{
    public DropMode dropMode;
    public Vector3 pos;
}

public struct PlayerRespawnEvent
{
    public GameObject player;
}