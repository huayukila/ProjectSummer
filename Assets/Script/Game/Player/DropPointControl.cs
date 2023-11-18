using Unity.VisualScripting;
using UnityEngine;

public class DropPointControl : MonoBehaviour
{
    private TrailRenderer _mTrailRenderer;      // DropPointが繋がっていることを表すTrailRenderer
    private GameObject pointPrefab;             // DropPointのプレハブ
    private float fadeOutTimer;

    private Timer _dropTimer;                   // DropPointのインスタンス化することを管理するタイマー

    private float trailOffset;

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
        trailOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
        _mPlayer = gameObject.GetOrAddComponent<Player>();
        _mTag = "DropPoint";
        _mID = -1;
        _mColor = Color.clear;

        // 尻尾を描画するGameObjectを作る
        GameObject trail = new GameObject(name + "Trail");
        // プレイヤーを親にする
        trail.transform.parent = transform;
        //todo take note
        // ワールド座標をローカル座標に変換する
        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        trail.transform.localPosition = Vector3.down * 0.5f - localForward * trailOffset;
        trail.transform.localScale = Vector3.one;
        // TrailRendererをアタッチする
        _mTrailRenderer = trail.gameObject.AddComponent<TrailRenderer>();

    }
    // Update is called once per frame
    private void Update()
    {
        TryDropPoint();
        fadeOutTimer += Time.deltaTime;
        // プレイヤーが場に一定時間を移動し続けたら（DropPointの生存時間の半分）
        if (fadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && fadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
        {
            // 不透明度を計算する　※　y = -1.9x + 1.95;
            float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * fadeOutTimer + 1.95f;
            // 不透明度の最小値を0.05に設定する
            if(alpha < 0.05f)
            {
                alpha = 0.05f;
            }
            SetTrailGradient(alpha);
        }

    }

    /// <summary>
    /// DropPointをインスタンス化する
    /// </summary>
    private void InstantiateDropPoint()
    {
        GameObject pt = Instantiate(pointPrefab, transform.position - transform.forward * trailOffset, transform.rotation);
        pt.tag = _mTag;
        DropPointSystem.Instance.AddPoint(_mID,pt);
    }

    /// <summary>
    /// TrailRendererの初期設定行う
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
    /// DropPointを置いてみる関数
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
    /// TrailRendererの状態をリセットする
    /// </summary>
    public void ResetTrail()
    {
        _mTrailRenderer.Clear();
        fadeOutTimer = 0.0f;
        SetTrailGradient(1.0f);
    }

    /// <summary>
    /// TrailRendererのグラディエントを設定する
    /// </summary>
    /// <param name="alpha">一番後ろの不透明度</param>
    private void SetTrailGradient(float alpha)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(_mColor, 0.0f), new GradientColorKey(_mColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        _mTrailRenderer.colorGradient = gradient;
    }
}


