using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer tr;                 // DropPoint���q�����Ă��邱�Ƃ�\��TrailRenderer
    protected GameObject pointPrefab;           // DropPoint�̃v���n�u
    protected float fadeOutTimer;

    private Timer _dropTimer;                   // DropPoint�̃C���X�^���X�����邱�Ƃ��Ǘ�����^�C�}�[



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

    /// <summary>
    /// TrailRenderer�̑S�Ă̓_������
    /// </summary>
    public void ClearTrail()
    {
        tr.Clear();
    }

    private void Awake()
    {
        pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
        tr = gameObject.GetComponent<TrailRenderer>();
        fadeOutTimer = 0.0f;
        SetTRProperties();
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        TryDropPoint();
    }

    private void FixedUpdate() { }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        Destroy(tr.material);
    }
}


