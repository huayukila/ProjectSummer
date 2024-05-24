using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    public Button readyOrNotBtn;
    public Button quitBtn;

    public Image[] playerImages;
    Color notReadyColor = Color.gray;

    // Start is called before the first frame update
    private void Awake()
    {
        foreach (var image in playerImages)
        {
            image.color = notReadyColor;
        }
    }

    public void UpdatePlayerUI(int id, bool isReady)
    {
        playerImages[id].color = isReady ? Color.white : notReadyColor;
    }

    void Start()
    {
        quitBtn.onClick.AddListener(() => { (NetworkManager.singleton as IRoomManager).ExitToOffline(); });

        readyOrNotBtn.onClick.AddListener(() =>
        {
            NetworkClient.connection.identity.gameObject.GetComponent<CustomNetworkRoomPlayer>()
                .CmdChangeReadyState(true);
            readyOrNotBtn.gameObject.SetActive(false);
        });
    }

    private void OnDestroy()
    {
        readyOrNotBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
    }
}