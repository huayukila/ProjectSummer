using UnityEngine;

public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer TR;                 // DropPoint���q�����Ă��邱�Ƃ�\��TrailRenderer
    protected GameObject _pointPrefab;          // DropPoint�̃v���n�u
    Timer _dropTimer;                           // DropPoint�̃C���X�^���X�����邱�Ƃ��Ǘ�����^�C�}�[

    /// <summary>
    /// DropPoint���C���X�^���X������
    /// </summary>
    protected abstract void InstantiateDropPoint();

    /// <summary>
    /// TrailRenderer�̏����ݒ�s��
    /// </summary>
    protected abstract void SetTRProperties();

    /// <summary>
    /// DropPoint��u��
    /// </summary>    
    private void TryDropPoint()
    {
        // �^�C�}�[�� null ��������
        if(_dropTimer == null)
        {
            // �V�����^�C�}�[�����
            _dropTimer = new Timer();
            _dropTimer.SetTimer(Global.DROP_POINT_INTERVAL,
                () =>
                {
                    // �^�C�}�[���I�������DropPoint��u��
                    InstantiateDropPoint();
                }
                );
        }
        // �^�C�}�[���I�������
        else if(_dropTimer.IsTimerFinished())
        {
            // �^�C�}�[������
            _dropTimer = null;
        }
    }

    private void Awake()
    {
        _pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
        TR = gameObject.AddComponent<TrailRenderer>();
        SetTRProperties();
    }

    // Update is called once per frame
    private void Update()
    {
        TryDropPoint();
    }

    private void FixedUpdate() { }

    /// <summary>
    /// TrailRenderer�̑S�Ă̓_������
    /// </summary>
    public void ClearTrail()
    {
        TR.Clear();
    }
}


