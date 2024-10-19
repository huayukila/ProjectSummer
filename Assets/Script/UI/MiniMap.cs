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

    // Start is called before the first frame update
    void Start()
    {
        miniMap.texture = PolygonPaintManager.Instance.GetMiniMapRT();
        TypeEventSystem.Instance.Register<RefreshVSBarEvent>(e =>
        {
            float[] values = PolygonPaintManager.Instance.GetPlayersAreaPercent();
            LeftBar.DOFillAmount(values[0],0.5f);
            RightBar.DOFillAmount(values[1],0.5f);
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}