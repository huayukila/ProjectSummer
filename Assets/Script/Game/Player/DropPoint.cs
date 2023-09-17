using UnityEngine;

public class DropPoint : MonoBehaviour
{
    Timer timer;        // DropPoint�̃^�C�}�[
    Timer _collidoractivetimer;
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
        _collidoractivetimer = new Timer();
        _collidoractivetimer.SetTimer(0.5f,
            () =>
            {
                gameObject.GetComponent<Collider>().enabled = true;
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
        if(_collidoractivetimer?.IsTimerFinished() == true)
        {
            _collidoractivetimer = null;
        }
    }


}
