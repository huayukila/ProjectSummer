using UnityEngine;

public class MiniMapCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform playerTrans;

    public RectTransform miniPlayerTrans;

    public Transform[] silksTrans = new Transform[3];

    // private List<Transform> 
    void Start()
    {
        TypeEventSystem.Instance.Register<SendInitializedPlayerEvent>(e => { playerTrans = e.Player.transform; })
            .UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //if player is not dead
        if (!playerTrans)
            return;
        miniPlayerTrans.anchoredPosition = new Vector3(playerTrans.position.x, playerTrans.position.z);
    }
}