using Unity.VisualScripting;
using UnityEngine;

public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer _mTrailRenderer;                 // DropPoint���q�����Ă��邱�Ƃ�\��TrailRenderer
    protected GameObject pointPrefab;           // DropPoint�̃v���n�u
    protected float fadeOutTimer;

    private Timer _dropTimer;                   // DropPoint�̃C���X�^���X�����邱�Ƃ��Ǘ�����^�C�}�[

    protected float offset;

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
        _mTrailRenderer.Clear();
    }

    private void Awake()
    {
        pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
        fadeOutTimer = 0.0f;
        offset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
       
    }

    private void Start()
    {
        GameObject trail = new GameObject(name + "Trail");
        trail.transform.parent = transform;
        //todo take note
        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        trail.transform.localPosition = Vector3.zero - localForward * offset + Vector3.down * 0.5f; 
        trail.transform.localScale = Vector3.one;
        _mTrailRenderer = trail.gameObject.AddComponent<TrailRenderer>();
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
    }
}


