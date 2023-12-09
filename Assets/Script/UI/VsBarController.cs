using UnityEngine;
using UnityEngine.UI;

public class VsBarController : MonoBehaviour
{
    public Image Left;

    public Image Right;

    private int m_FullValue = 2;

    private int m_LeftValue = 1;

    private int m_RightValue = 1;

    private void Start()
    {
        TypeEventSystem.Instance.Register<RefrashVSBarEvent>(e =>
        {
            m_LeftValue = e.PlayerPixelNums[0] + 1;
            m_RightValue = e.PlayerPixelNums[1] + 1;
            m_FullValue = m_LeftValue + m_RightValue;
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        Left.fillAmount = Mathf.Lerp(Left.fillAmount, m_LeftValue / (float)m_FullValue, 0.01f);
        Right.fillAmount = Mathf.Lerp(Right.fillAmount, m_RightValue / (float)m_FullValue, 0.01f);
    }
}