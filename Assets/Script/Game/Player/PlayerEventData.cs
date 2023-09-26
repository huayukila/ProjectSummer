using UnityEngine;

public struct AddScoreEvent
{ 
    public int playerID { get; set; }
}

public struct PickSilkEvent
{
    public GameObject player { get; set; }
}
public struct DropSilkEvent
{
    public DropMode dropMode { get; set; }
}

public class PlayerRespawnEvent
{
    public GameObject player { get; set; }
}