using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer tr;                 // DropPointが繋がっていることを表すTrailRenderer
    protected GameObject pointPrefab;           // DropPointのプレハブ
    protected float fadeOutTimer;

    private Timer _dropTimer;                   // DropPointのインスタンス化することを管理するタイマー



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


