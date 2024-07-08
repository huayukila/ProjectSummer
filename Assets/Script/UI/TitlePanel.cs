using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    public Button createGameBtn;
    public Button joinGameBtn;

    private void Start()
    {
        createGameBtn.onClick.AddListener(ButtonHost);
        joinGameBtn.onClick.AddListener(ButtonClient);
    }

    public void ButtonHost()
    {
        (NetworkManager.singleton as NetWorkRoomManagerExt).HostGame();
    }

    public void ButtonClient()
    {
        (NetworkManager.singleton as NetWorkRoomManagerExt).Connect();
    }
    
    private void OnDestroy()
    {
        createGameBtn.onClick.RemoveAllListeners();
        joinGameBtn.onClick.RemoveAllListeners();
    }
}