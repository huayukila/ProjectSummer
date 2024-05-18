using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VsBarController : MonoBehaviour
{
    public Image Left;

    public Image Right;

    public Transform VsImgTrans;

    public TextMeshProUGUI LeftText;
    public TextMeshProUGUI RightText;

    private int m_FullValue = 2;

    private int m_LeftValue = 1;

    private int m_RightValue = 1;
    private float anchorX;

    private void Start()
    {
        LeftText.text = "0%";
        RightText.text = "0%";
        VsImgTrans.localPosition = Left.transform.localPosition;
        anchorX = Left.transform.localPosition.x - Left.rectTransform.sizeDelta.x / 2.0f;
        TypeEventSystem.Instance.Register<RefreshVSBarEvent>(e =>
        {
            m_LeftValue = e.PlayerPixelNums[0] + 1;
            m_RightValue = e.PlayerPixelNums[1] + 1;
            m_FullValue = m_LeftValue + m_RightValue;
            LeftText.text = (Mathf.Round(PolygonPaintManager.Instance.GetPlayersAreaPercent()[0] * 100f) / 100f)
                .ToString() + "%";
            RightText.text = (Mathf.Round(PolygonPaintManager.Instance.GetPlayersAreaPercent()[1] * 100f) / 100f)
                .ToString() + "%";
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    private void FixedUpdate()
    {
        Left.fillAmount = Mathf.Lerp(Left.fillAmount, m_LeftValue / (float)m_FullValue, 0.1f);
        Right.fillAmount = Mathf.Lerp(Right.fillAmount, m_RightValue / (float)m_FullValue, 0.1f);

        VsImgTrans.localPosition = new Vector3(anchorX + Left.rectTransform.sizeDelta.x * Left.fillAmount,
            Left.transform.localPosition.y, 0);
    }
}