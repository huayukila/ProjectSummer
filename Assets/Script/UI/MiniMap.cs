using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Image LeftBar;
    public Image RightBar;

    public RectTransform LeftPlayerImg;
    public RectTransform RightPlayerImg;

    public RectTransform ItemBoxImg;

    public RawImage miniMap;

    public RectTransform[] Silks;

    public RectTransform ItemBox;

    private float mapHeight;
    private float mapWidth;

    // Start is called before the first frame update
    void Start()
    {
        miniMap.texture = PolygonPaintManager.Instance.GetMiniMapRT();
        TypeEventSystem.Instance.Register<RefreshVSBarEvent>(e =>
        {
            float[] values = PolygonPaintManager.Instance.GetPlayersAreaPercent();
            LeftBar.DOFillAmount(values[0] * 0.02f, 0.5f);
            RightBar.DOFillAmount(values[1] * 0.02f, 0.5f);
        }).UnregisterWhenGameObjectDestroyed(gameObject);
        mapHeight = Global.MAP_SIZE_HEIGHT;
        mapWidth = Global.MAP_SIZE_WIDTH;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 leftPlayerPos = GameManager.Instance.GetPlayerPos(1);

        Vector3 rightPlayerPos = GameManager.Instance.GetPlayerPos(2);
        LeftPlayerImg.localPosition = new Vector3(leftPlayerPos.x * mapHeight, leftPlayerPos.z * mapWidth, 0);
        RightPlayerImg.localPosition = new Vector3(rightPlayerPos.x * mapHeight, rightPlayerPos.z * mapWidth, 0);
    }
}