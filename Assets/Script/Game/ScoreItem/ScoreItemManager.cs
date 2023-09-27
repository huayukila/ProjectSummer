using UnityEngine;

public enum DropMode
{
    Standard,
    Edge
};
public class ScoreItemManager : Singleton<ScoreItemManager>
{   
    private Timer _goldenSilkSpawnTimer;        // 金の網を生成することを管理するタイマー
    private GameObject _gotSilkPlayer;          // 金の網を持っているプレイヤー
    private Vector3 _awayFromEdgeStartPos;
    private Vector3 _awayFromEdgeEndPos;
    private bool _isStartAwayFromEdge;

    private GameObject _inSpaceSilk;            // 金の網
    private GameObject _goalPoint;              // ゴール


    /// <summary>
    /// 金の網の生成位置を決める関数
    /// 未完成のため、固定位置に生成している
    /// </summary>
    /// <returns></returns>
    private Vector3 GetInSpaceRandomPosition()
    {
        // temp pos
        return new Vector3(0.0f,0.64f,0.0f);
    }

    /// <summary>
    /// 金の網はゴールまで運搬された時の操作をする
    /// </summary>
    private void SetReachGoalProperties(int ID)
    {
        ScoreSystem.Instance.AddScore(ID);
        // 新しい金の網を生成する
        GenerateNewSilk();
    }

    /// <summary>
    /// ゴールの位置を生成する
    /// 未完成のため、固定位置に生成している
    /// </summary>
    private void SetGoalPoint()
    {
        // temp pos
        _goalPoint.transform.position = new Vector3(35.0f,0.64f,15.0f);
        _goalPoint.SetActive(true);
    }

    private void GenerateNewSilk()
    {
        _goldenSilkSpawnTimer = new Timer();
        _goldenSilkSpawnTimer.SetTimer(10.0f,
            () =>
            {
                _inSpaceSilk.transform.position = GetInSpaceRandomPosition();
                _inSpaceSilk.SetActive(true);
            }
            );
        _inSpaceSilk.SetActive(false);
        _goalPoint.SetActive(false);
        _gotSilkPlayer = null;
    }
    /// <summary>
    /// 金の網が落ちたときのステータスを設定する
    /// </summary>
    private void SetDropSilkStatus()
    {
        _inSpaceSilk.SetActive(true);
        _goalPoint.SetActive(false);
        _gotSilkPlayer = null;
    }
    /// <summary>
    /// 金の網を持っているプレイヤーの設定をする
    /// </summary>
    /// <param name="ob"></param>
    private void SetGotSilkPlayer(GameObject ob)
    {
        if(_gotSilkPlayer == null)
        {
            _gotSilkPlayer = ob;
        }
        // 持っているプレイヤーの色を変える（区別するため）
        _gotSilkPlayer.GetComponent<Renderer>().material.color = Color.yellow;
        _inSpaceSilk.SetActive(false);
        SetGoalPoint();
    }

    /// <summary>
    /// 金の網を持っていたプレイヤーが死んだら金の網をドロップする
    /// </summary>
    private void DropGoldenSilk(DropMode mode)
    {
        if( _gotSilkPlayer != null )
        {
            switch(mode)
            {
                case DropMode.Standard:
                    {
                        _inSpaceSilk.transform.position = _gotSilkPlayer.transform.position;
                        break;
                    }
                case DropMode.Edge: 
                    {
                        _awayFromEdgeStartPos = _gotSilkPlayer.transform.position;
                        _awayFromEdgeEndPos = (_gotSilkPlayer.transform.position - new Vector3(0.0f, 0.64f, 0.0f)) * 0.8f + new Vector3(0.0f, 0.64f, 0.0f) * 0.2f;
                        _isStartAwayFromEdge = true;
                        break;
                    }        
            }

            SetDropSilkStatus();
        }
    }

    /// <summary>
    /// プレイヤーが金の網を持っているかどうかをチェックする
    /// </summary>
    /// <param name="ob">プレイヤー</param>
    /// <returns>持っていたらtrueを返す、それ以外はfalseを返す</returns>
    public bool IsGotSilk(GameObject ob) => _gotSilkPlayer == ob;

    protected override void Awake()
    {
        Init();
        GenerateNewSilk();
        _isStartAwayFromEdge = false;
        _inSpaceSilk.GetComponent<Renderer>().material.color = Color.yellow;
        _goalPoint.GetComponent<Renderer>().material.color = Color.yellow;

        TypeEventSystem.Instance.Register<AddScoreEvent>(e =>
        {
            SetReachGoalProperties(e.playerID);

        }).UnregisterWhenGameObjectDestroyde(gameObject);

        TypeEventSystem.Instance.Register<DropSilkEvent>(e =>
        {
            DropGoldenSilk(e.dropMode);

        }).UnregisterWhenGameObjectDestroyde(gameObject);
        TypeEventSystem.Instance.Register<PickSilkEvent>(e =>
        {
            SetGotSilkPlayer(e.player);

        }).UnregisterWhenGameObjectDestroyde(gameObject);

    }

    private void Init()
    {
        if(_inSpaceSilk == null)
        {
            GameObject silkPrefab = (GameObject)Resources.Load("Prefabs/GoldenSilk");
            _inSpaceSilk = Instantiate(silkPrefab, GetInSpaceRandomPosition(),Quaternion.identity);
            _inSpaceSilk.GetComponent<Renderer>().material.color = Color.yellow;
        }
        if(_goalPoint == null)
        {
            GameObject goalPrefab = (GameObject)Resources.Load("Prefabs/Goal");
            _goalPoint = Instantiate(goalPrefab, new Vector3(35.0f, 0.64f, 15.0f), Quaternion.identity);
            _goalPoint.GetComponent<Renderer>().material.color = Color.yellow;
        }

    }
    private void Update()
    {
        if (_goldenSilkSpawnTimer != null)
        {
            if (_goldenSilkSpawnTimer.IsTimerFinished())
            {
                _goldenSilkSpawnTimer = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isStartAwayFromEdge)
        {
            Vector3 temp = Vector3.Lerp(_awayFromEdgeStartPos, _awayFromEdgeEndPos, 0.05f);
            _awayFromEdgeStartPos = temp;
            _inSpaceSilk.transform.position = _awayFromEdgeStartPos;
            if((_awayFromEdgeStartPos - _awayFromEdgeEndPos).magnitude <= 0.1f)
            {
                _isStartAwayFromEdge = false;
            }
        }
    }

}
