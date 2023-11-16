using Unity.VisualScripting;
using UnityEngine;

public class DropPointControl : MonoBehaviour
{
    protected TrailRenderer _mTrailRenderer;                 // DropPoint���q�����Ă��邱�Ƃ�\��TrailRenderer
    protected GameObject pointPrefab;           // DropPoint�̃v���n�u
    protected float fadeOutTimer;

    private Timer _dropTimer;                   // DropPoint�̃C���X�^���X�����邱�Ƃ��Ǘ�����^�C�}�[

    protected float offset;

    //TODO refactorying
    [SerializeField]
    private string _mTag;
    [SerializeField]
    private int _mID;
    [SerializeField]
    private Color _mColor;

    private Player _mPlayer;

    private void Awake()
    {
        pointPrefab = GameResourceSystem.Instance.GetPrefabResource("DropPoint");
        fadeOutTimer = 0.0f;
        offset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
        _mPlayer = gameObject.GetOrAddComponent<Player>();
        _mTag = "DropPoint";
        _mID = -1;
        _mColor = Color.clear;
        GameObject trail = new GameObject(name + "Trail");
        trail.transform.parent = transform;
        //todo take note
        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        trail.transform.localPosition = Vector3.zero - localForward * offset + Vector3.down * 0.5f;
        trail.transform.localScale = Vector3.one;
        _mTrailRenderer = trail.gameObject.AddComponent<TrailRenderer>();

    }
    // Update is called once per frame
    private void Update()
    {
        TryDropPoint();
        fadeOutTimer += Time.deltaTime;
        if (fadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && fadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
        {
            float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * fadeOutTimer + 1.95f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(_mColor, 0.0f), new GradientColorKey(_mColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha >= 0.05f ? alpha : 0.05f, 1.0f) }
            );
            _mTrailRenderer.colorGradient = gradient;
        }

    }

    /// <summary>
    /// DropPoint���C���X�^���X������
    /// </summary>
    private void InstantiateDropPoint()
    {
        GameObject pt = Instantiate(pointPrefab, transform.position - transform.forward * offset, transform.rotation);
        pt.tag = _mTag;
        DropPointSystem.Instance.AddPoint(_mID,pt);
    }

    /// <summary>
    /// TrailRenderer�̏����ݒ�s��
    /// </summary>
    public void Init()
    {
        _mID = _mPlayer.GetID();
        _mTag = "DropPoint" + _mID.ToString();
        _mColor = _mPlayer.GetColor();
        _mTrailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _mTrailRenderer.startColor = Global.PLAYER_TRACE_COLORS[_mID - 1];
        _mTrailRenderer.endColor = Global.PLAYER_TRACE_COLORS[_mID - 1];
        _mTrailRenderer.startWidth = 1.0f;
        _mTrailRenderer.endWidth = 1.0f;
        _mTrailRenderer.time = Global.DROP_POINT_ALIVE_TIME;
    }

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
    /// TrailRenderer�̏�Ԃ����Z�b�g����
    /// </summary>
    public void ResetTrail()
    {
        _mTrailRenderer.Clear();
        fadeOutTimer = 0.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(_mColor, 0.0f), new GradientColorKey(_mColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        _mTrailRenderer.colorGradient = gradient;
    }
}


