using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkRoomPlayer : NetworkRoomPlayer
{
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (SceneManager.GetActiveScene().name == "Room")
        {
            FindObjectOfType<RoomPanel>().UpdatePlayerUI(index, readyToBegin);
        }
    }
}