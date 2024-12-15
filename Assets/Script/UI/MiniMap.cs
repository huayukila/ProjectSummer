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

    [Header("VSBarAnimSetting")] public Image VSBarImg;
    public Sprite[] vsBarAnim;
    public float changeTime;

    private float mapHeight;
    private float mapWidth;
    private int AnimIndex = 0;
    private float currentTime = 0;

    private Vector3 root = new Vector3(-450, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        miniMap.texture = PolygonPaintManager.Instance.GetMiniMapRT();
        TypeEventSystem.Instance.Register<RefreshVSBarEvent>(e =>
        {
            float[] values = PolygonPaintManager.Instance.GetPlayersAreaPercent();

            float total = values[0] + values[1];

            LeftBar.DOFillAmount(values[0] / total, 0.5f);
            RightBar.DOFillAmount(values[1] / total, 0.5f);

            VSBarImg.rectTransform.DOLocalMove(root + Vector3.right * (900 * values[0] / total), 0.5f);
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

        currentTime += Time.deltaTime;
        if (currentTime > changeTime)
        {
            AnimIndex++;
            AnimIndex = AnimIndex % vsBarAnim.Length;
            currentTime = 0;
            VSBarImg.sprite = vsBarAnim[AnimIndex];
        }
    }
}