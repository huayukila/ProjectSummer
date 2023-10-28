using Unity.VisualScripting;
using UnityEngine;

public enum DropMode
{
    Standard,
    Edge
};
public class ScoreItemManager : Singleton<ScoreItemManager>
{
    private Timer _goldenSilkSpawnTimer;        // 金の網を生成することを管理するタイマー
    private Vector3 _awayFromEdgeStartPos;
    private Vector3 _awayFromEdgeEndPos;
    private bool _isStartAwayFromEdge;
    private float _goalAnimationDurationTime = 0.5f;

    private GameObject _inSpaceSilk;            // 金の網
    private GameObject _goalPoint;              // ゴール

    private GameObject _silkShadow;
    private GameObject _silkAirdrop;

    private Timer _goalSpawnAnimationTimer;

    private bool _isPlayingFallFX;

    /// <summary>
    /// 金の網の生成位置を決める関数
    /// 未完成のため、固定位置に生成している
    /// </summary>
    /// <returns></returns>
    private Vector3 GetInSpaceRandomPosition()
    {
        // ステージの一定範囲内にインスタンス化する
        float spawnAreaLength = Global.STAGE_LENGTH / 2.5f;
        float spawnAreaWidth = Global.STAGE_WIDTH / 2.5f;
        float posX = 0.0f;
        float posZ = 0.0f;
        while (posX == 0.0f || posZ == 0.0f)
        {
            posX = Random.Range(-spawnAreaLength, spawnAreaLength);
            posZ = Random.Range(-spawnAreaWidth, spawnAreaWidth);
        }
        return new Vector3(posX, 0.64f, posZ);
    }

    /// <summary>
    /// 金の網はゴールまで運搬された時の操作をする
    /// </summary>
    private void SetReachGoalProperties(int ID)
    {
        ScoreSystem.Instance.AddScore(ID, Global.SILK_SCORE);
        // 新しい金の網を生成する
        GenerateNewSilk();
    }

    /// <summary>
    /// ゴールの位置を生成する
    /// </summary>
    private void SetGoalPoint(Vector3 pos)
    {
        _inSpaceSilk.SetActive(false);
        float posX = 0.0f;
        if (pos.x < 0.0f)
        {
            posX = Random.Range(Global.STAGE_LENGTH / 5.0f, Global.STAGE_LENGTH / 2.5f);
        }
        else if (pos.x > 0.0f)
        {
            posX = Random.Range(-Global.STAGE_LENGTH / 2.5f, -Global.STAGE_LENGTH / 5.0f);
        }

        float zRange = CalculateOvalRange(posX);
        float posZ = Random.Range(0, 1) == 0 ? Random.Range(zRange, Global.STAGE_WIDTH / 2.5f) : Random.Range(-Global.STAGE_WIDTH / 2.5f, -zRange);

        _goalPoint.transform.position = new Vector3(posX, 0.32f, posZ);
        _goalSpawnAnimationTimer = new Timer();
        _goalSpawnAnimationTimer.SetTimer(_goalAnimationDurationTime,
            () =>
            {
                _goalPoint.GetComponent<Collider>().enabled = true;
            });
        _goalPoint.SetActive(true);
    }

    private float CalculateOvalRange(float x)
    {
        return Mathf.Sqrt(Mathf.Pow(Global.STAGE_WIDTH / 2.5f, 2) * (1 - (Mathf.Pow(x, 2) / Mathf.Pow(Global.STAGE_LENGTH / 2.5f, 2))));
    }
    private void GenerateNewSilk()
    {
        if(_goldenSilkSpawnTimer == null)
        {
            _inSpaceSilk.transform.position = GetInSpaceRandomPosition();
            ResetAnimationStatus();
            _goldenSilkSpawnTimer = new Timer();
            _goldenSilkSpawnTimer.SetTimer(Global.SILK_SPAWN_TIME,
                () =>
                {
                    ResetAnimationStatus();
                    _inSpaceSilk.SetActive(true);
                    GameObject smoke = Instantiate(Resources.Load("Prefabs/Smoke") as GameObject, _inSpaceSilk.transform.position, Quaternion.identity);
                    smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                    smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);
                }
                );
            _inSpaceSilk.SetActive(false);
            _goalPoint.SetActive(false);
            _goalPoint.transform.localScale = Vector3.zero;
            _goalPoint.GetComponent<Collider>().enabled = false;
        }
    }
    /// <summary>
    /// 金の網が落ちたときのステータスを設定する
    /// </summary>
    private void SetDropSilkStatus()
    {
        _inSpaceSilk.SetActive(true);
        _goalPoint.SetActive(false);
        _goalPoint.transform.localScale = Vector3.zero;
        _goalSpawnAnimationTimer = null;
    }

    /// <summary>
    /// 金の網を持っていたプレイヤーが死んだら金の網をドロップする
    /// </summary>
    private void DropGoldenSilk(DropMode mode, Vector3 pos)
    {
        switch (mode)
        {
            case DropMode.Standard:
                _inSpaceSilk.transform.position = pos;
                break;
            case DropMode.Edge:
                _inSpaceSilk.transform.position = pos;
                _awayFromEdgeStartPos = pos;
                _awayFromEdgeEndPos = (pos - new Vector3(0.0f, 0.64f, 0.0f)) * 0.7f + new Vector3(0.0f, 0.64f, 0.0f) * 0.3f;
                _isStartAwayFromEdge = true;
                break;
        }
        SetDropSilkStatus();
    }

    protected override void Awake()
    {
        EventRegister();
    }

    public void Init()
    {
        if (_inSpaceSilk == null)
        {
            GameObject silkPrefab = Resources.Load("Prefabs/GoldenSilk") as GameObject;
            _inSpaceSilk = Instantiate(silkPrefab, GetInSpaceRandomPosition(), Quaternion.identity);
            _silkAirdrop = Instantiate(silkPrefab, _inSpaceSilk.transform.position, Quaternion.identity);
            _silkAirdrop.GetComponent<BoxCollider>().enabled = false;
            _silkAirdrop.SetActive(false);
            _silkShadow = Instantiate(Resources.Load("Prefabs/SilkShadow") as GameObject, _inSpaceSilk.transform.position - new Vector3(0.0f, 0.1f, 0.0f), Quaternion.identity);
            _silkShadow.SetActive(false);
        }
        if (_goalPoint == null)
        {
            GameObject goalPrefab = Resources.Load("Prefabs/Goal") as GameObject;
            _goalPoint = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity);
            _goalPoint.transform.localScale = Vector3.zero;
            _goalPoint.GetComponent<Collider>().enabled = false;
            _goldenSilkSpawnTimer = null;
        }
        _isStartAwayFromEdge = false;
        System.Random rand = new System.Random((int)Time.time);
        _isPlayingFallFX = false;
        GenerateNewSilk();
    }

    private void EventRegister()
    {
        TypeEventSystem.Instance.Register<AddScoreEvent>(e =>
        {
            AudioManager.Instance.PlayFX("GetFX", 0.7f);
            SetReachGoalProperties(e.playerID);

        }).UnregisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Instance.Register<DropSilkEvent>(e =>
        {
            DropGoldenSilk(e.dropMode, e.pos);

        }).UnregisterWhenGameObjectDestroyed(gameObject);
        TypeEventSystem.Instance.Register<PickSilkEvent>(e =>
        {
            AudioManager.Instance.PlayFX("SpawnFX", 0.7f);
            SetGoalPoint(_inSpaceSilk.transform.position);

        }).UnregisterWhenGameObjectDestroyed(gameObject);

    }
    private void PlayGoldenSilkAnimation()
    {
        if(_silkAirdrop.gameObject.activeSelf == false)
        {
            _silkAirdrop.SetActive(true);
            _silkAirdrop.transform.position = _inSpaceSilk.transform.position + Vector3.forward * 100.0f;
        }
        if(_silkShadow.gameObject.activeSelf == false)
        {
            _silkShadow.SetActive(true);
            _silkShadow.transform.position = _inSpaceSilk.transform.position - new Vector3(0.0f, 0.6f, 0.0f);
        }
        _silkAirdrop.transform.Translate(0.0f,0.0f,-200.0f / Global.SILK_SPAWN_TIME * Time.deltaTime);
        _silkAirdrop.transform.localScale -= Vector3.one * Time.deltaTime * 2.0f / Global.SILK_SPAWN_TIME;
        _silkShadow.transform.localScale += Vector3.one * Time.deltaTime * 2.0f/ Global.SILK_SPAWN_TIME;
    }

    private void ResetAnimationStatus()
    {
        _silkAirdrop.SetActive(false);
        _silkShadow.SetActive(false);
        _silkAirdrop.transform.localScale = Vector3.one * 1.9f;
        _silkShadow.transform.localScale = Vector3.zero;
        _silkShadow.GetComponent<Renderer>().material.color = new Color(0.0f,0.0f,0.0f,1.0f);
    }

    private void PlayGoalAnimation()
    {
        _goalPoint.transform.localScale = _goalPoint.transform.localScale.x >= 1.0f ? Vector3.one : _goalPoint.transform.localScale + Vector3.one * Time.deltaTime / _goalAnimationDurationTime;

    }
    private void Update()
    {
        if (_goldenSilkSpawnTimer != null)
        {
            if(_goldenSilkSpawnTimer.GetTime() <= Global.SILK_SPAWN_TIME / 2.0f)
            {
                if(_isPlayingFallFX == false)
                {
                    AudioManager.Instance.PlayFX("FallFX", 0.7f);
                    _isPlayingFallFX = true;
                }
                PlayGoldenSilkAnimation();
            }
            if (_goldenSilkSpawnTimer.IsTimerFinished())
            {
                _goldenSilkSpawnTimer = null;
                _isPlayingFallFX = false;
            }
        }

        if (_goalSpawnAnimationTimer != null)
        {
            PlayGoalAnimation();
            if(_goalSpawnAnimationTimer.IsTimerFinished())
            {
                _goalSpawnAnimationTimer = null;
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
                _awayFromEdgeStartPos = Vector3.zero;
                _awayFromEdgeEndPos = Vector3.zero;
            }
        }
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        _isPlayingFallFX = false;
        _isStartAwayFromEdge = false;
        _goalSpawnAnimationTimer = null;
    }

}
