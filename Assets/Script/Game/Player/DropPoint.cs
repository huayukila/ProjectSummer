using System.Data;
using UnityEngine;

public class DropPoint : MonoBehaviour
{
    private Timer _mTimer;        // DropPoint�̃^�C�}�[


    void Awake()
    {
        // �^�C�}�[������������
        _mTimer = new Timer();
        _mTimer.SetTimer(Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                // ���Ԃ��o������DropPoint������
                Destroy(gameObject);
            }
            );
    }

    void Update()
    {
        // �^�C�}�[���I���܂Ń`�F�b�N
        if(_mTimer.IsTimerFinished())
        {
            int i = -1;
            string numString = null;
            if(tag.Length > 9)
            {
                numString = tag.Substring(9);
            }
            if(int.TryParse(numString,out int num) == true)
            {
                i = num;
            }
            DropPointSystem.Instance.RemovePoint(i, gameObject);
            // �ǂ̃v���C���[�����Ƃ���DropPoint���`�F�b�N

        }
    }

}
