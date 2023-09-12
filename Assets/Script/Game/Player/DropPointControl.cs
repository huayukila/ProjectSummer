using UnityEngine;

public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer TR;                 // DropPointが繋がっていることを表すTrailRenderer
    protected GameObject _pointPrefab;          // DropPointのプレハブ
    Timer _dropTimer;                           // DropPointのインスタンス化することを管理するタイマー

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
    /// TrailRendererの全ての点を消す
    /// </summary>
    public void ClearTrail()
    {
        TR.Clear();
    }
}


