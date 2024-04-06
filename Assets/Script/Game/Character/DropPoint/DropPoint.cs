using UnityEngine;

public class DropPoint : MonoBehaviour
{
    private Timer m_NewTimer;            // DropPointのタイマー

    void Awake()
    {
        m_NewTimer = new Timer(Time.time,Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                Destroy(gameObject);
            });
        m_NewTimer.StartTimer(this);
    }

    private void OnDestroy()
    {
        // どのプレイヤーが落としたDropPointをチェック
        int i = -1;
        string numString = null;
        if (tag.Length > 9)
        {
            numString = tag.Substring(9);
        }
        if (int.TryParse(numString, out int num) == true)
        {
            i = num;
        }
        DropPointSystem.Instance.RemovePoint(i, gameObject);

    }
}
