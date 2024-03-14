using UnityEngine;

public class DropPoint : MonoBehaviour
{
    private Timer m_NewTimer;            // DropPoint�̃^�C�}�[

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
        // �ǂ̃v���C���[�����Ƃ���DropPoint���`�F�b�N
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
