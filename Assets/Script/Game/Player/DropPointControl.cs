using Unity.VisualScripting;
using UnityEngine;

public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer _mTrailRenderer;                 // DropPointが繋がっていることを表すTrailRenderer
    protected GameObject pointPrefab;           // DropPointのプレハブ
    protected float fadeOutTimer;

    private Timer _dropTimer;                   // DropPointのインスタンス化することを管理するタイマー

    protected float offset;

    /// <summary>
    /// DropPointをインスタンス化する
    /// </summary>
    protected abstract void InstantiateDropPoint();

    /// <summary>
    /// TrailRendererの初期設定行う
    /// </summary>
    protected abstract void SetTRProperties();

    /// <summary>
    /// DropPointを置く
    /// </summary>    
    private void TryDropPoint()
    {
        // タイマーが null だったら
        if(_dropTimer == null)
        {
            // 新しいタイマーを作る
            _dropTimer = new Timer();
            _dropTimer.SetTimer(Global.DROP_POINT_INTERVAL,
                () =>
                {
                    // タイマーが終わったらDropPointを置く
                    InstantiateDropPoint();
                }
                );
        }
        // タイマーが終わったら
        else if(_dropTimer.IsTimerFinished())
        {
            // タイマーを消す
            _dropTimer = null;
        }
    }

    /// <summary>
    /// TrailRendererの全ての点を消す
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


