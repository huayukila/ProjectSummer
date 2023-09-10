using UnityEngine;

public class DropPoint : MonoBehaviour
{
    Timer timer;        // DropPoint�̃^�C�}�[

    void Awake()
    {
        // �^�C�}�[������������
        timer = new Timer();
        timer.SetTimer(Global.DROP_POINT_ALIVE_TIME,
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
        if(timer.IsTimerFinished())
        {
            // �ǂ̃v���C���[�����Ƃ���DropPoint���`�F�b�N
            if(gameObject.CompareTag("DropPoint1"))
            {
                DropPointManager.Instance.PlayerOneRemovePoint(gameObject);
            }
            else
            {
                DropPointManager.Instance.PlayerTwoRemovePoint(gameObject);
            }
        }
    }


}
