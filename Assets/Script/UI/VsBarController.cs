using UnityEngine;
using UnityEngine.UI;

public class VsBarController : MonoBehaviour
{
    public Image Left;

    public Image Right;

    public Transform VsImgTrans;
    private int m_FullValue = 2;

    private int m_LeftValue = 1;

    private int m_RightValue = 1;
    private float anchorX;

    private void Start()
    {
        VsImgTrans.position = Left.transform.position;
        anchorX = Left.transform.position.x - Left.rectTransform.sizeDelta.x / 2;
        TypeEventSystem.Instance.Register<RefreshVSBarEvent>(e =>
        {
            m_LeftValue = e.PlayerPixelNums[0] + 1;
            m_RightValue = e.PlayerPixelNums[1] + 1;
            m_FullValue = m_LeftValue + m_RightValue;
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    private void FixedUpdate()
    {
        Left.fillAmount = Mathf.Lerp(Left.fillAmount, m_LeftValue / (float)m_FullValue, 0.1f);
        Right.fillAmount = Mathf.Lerp(Right.fillAmount, m_RightValue / (float)m_FullValue, 0.1f);
        
        VsImgTrans.position = new Vector3(anchorX + Left.rectTransform.sizeDelta.x * Left.fillAmount,
            Left.transform.position.y, 0);
    }
}