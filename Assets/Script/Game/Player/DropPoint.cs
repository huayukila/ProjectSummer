using UnityEngine;

public class DropPoint : MonoBehaviour
{
    Timer _timer;        // DropPoint�̃^�C�}�[
    float _radius;
    bool _turnOn;
    void Awake()
    {
        // �^�C�}�[������������
        _timer = new Timer();
        _timer.SetTimer(Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                // ���Ԃ��o������DropPoint������
                Destroy(gameObject);
            }
            );
        _radius = GetComponent<SphereCollider>().radius;
        _turnOn = false;
    }

    void Update()
    {
        // �^�C�}�[���I���܂Ń`�F�b�N
        if(_timer.IsTimerFinished())
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

    private void FixedUpdate()
    {
        if(!_turnOn)
        {
            CheckIsAbleCollide();
        } 
    }

    /// <summary>
    /// DropPoint�ƃv���C���[�͈��̋����ȏ㗣�ꂽ��DropPoint�̃R���C�_�[���A�N�e�B�u�ɐݒ肷��
    /// </summary>
    private void CheckIsAbleCollide()
    {
        Vector3 playerPos = gameObject.CompareTag("DropPoint1") ? GameManager.Instance.playerOne.transform.position : GameManager.Instance.playerTwo.transform.position;
        Vector3 distance = playerPos - gameObject.transform.position;
        // 5.0f�͌�قǃv���C���[�̃R���C�_�[�̔��a�ɒu��������
        if (distance.magnitude > _radius + 5.0f)
        {
            _turnOn = true;
            GetComponent<SphereCollider>().enabled = true;
        }
    }

}
