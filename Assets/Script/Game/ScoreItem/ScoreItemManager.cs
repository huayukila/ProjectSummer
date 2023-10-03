using UnityEngine;

public enum DropMode
{
    Standard,
    Edge
};
public class ScoreItemManager : Singleton<ScoreItemManager>
{   
    private Timer _goldenSilkSpawnTimer;        // ���̖Ԃ𐶐����邱�Ƃ��Ǘ�����^�C�}�[
    private Vector3 _awayFromEdgeStartPos;
    private Vector3 _awayFromEdgeEndPos;
    private bool _isStartAwayFromEdge;

    private GameObject _inSpaceSilk;            // ���̖�
    private GameObject _goalPoint;              // �S�[��


    /// <summary>
    /// ���̖Ԃ̐����ʒu�����߂�֐�
    /// �������̂��߁A�Œ�ʒu�ɐ������Ă���
    /// </summary>
    /// <returns></returns>
    private Vector3 GetInSpaceRandomPosition()
    {
        float spawnAreaLength = Global.STAGE_LENGTH / 2.5f;
        float spawnAreaWidth = Global.STAGE_WIDTH / 2.5f;
        float posX = Random.Range(-spawnAreaLength,spawnAreaLength);
        float posZ = Random.Range(-spawnAreaWidth, spawnAreaWidth);
        return new Vector3(posX,0.64f,posZ);
    }

    /// <summary>
    /// ���̖Ԃ̓S�[���܂ŉ^�����ꂽ���̑��������
    /// </summary>
    private void SetReachGoalProperties(int ID)
    {
        ScoreSystem.Instance.AddScore(ID);
        // �V�������̖Ԃ𐶐�����
        GenerateNewSilk();
    }

    /// <summary>
    /// �S�[���̈ʒu�𐶐�����
    /// �������̂��߁A�Œ�ʒu�ɐ������Ă���
    /// </summary>
    private void SetGoalPoint(Vector3 pos)
    {
        _inSpaceSilk.SetActive(false);
        //todo temp pos
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
    }
    /// <summary>
    /// ���̖Ԃ��������Ƃ��̃X�e�[�^�X��ݒ肷��
    /// </summary>
    private void SetDropSilkStatus()
    {
        _inSpaceSilk.SetActive(true);
        _goalPoint.SetActive(false);
    }

    /// <summary>
    /// ���̖Ԃ������Ă����v���C���[�����񂾂���̖Ԃ��h���b�v����
    /// </summary>
    private void DropGoldenSilk(DropMode mode , Vector3 pos)
    {
        switch(mode)
        {
            case DropMode.Standard:
                _inSpaceSilk.transform.position = pos;
                break;
            case DropMode.Edge: 
                _awayFromEdgeStartPos = pos;
                _awayFromEdgeEndPos = (pos - new Vector3(0.0f, 0.64f, 0.0f)) * 0.7f + new Vector3(0.0f, 0.64f, 0.0f) * 0.3f;
                _isStartAwayFromEdge = true;
                break;      
        }
        SetDropSilkStatus();
    }

    protected override void Awake()
    {
        Init();
        GenerateNewSilk();
        _isStartAwayFromEdge = false;
        _inSpaceSilk.GetComponent<Renderer>().material.color = Color.yellow;
        _goalPoint.GetComponent<Renderer>().material.color = Color.yellow;
        System.Random rand = new System.Random((int)Time.time);

        TypeEventSystem.Instance.Register<AddScoreEvent>(e =>
        {
            SetReachGoalProperties(e.playerID);

        }).UnregisterWhenGameObjectDestroyde(gameObject);

        TypeEventSystem.Instance.Register<DropSilkEvent>(e =>
        {
            DropGoldenSilk(e.dropMode,e.pos);

        }).UnregisterWhenGameObjectDestroyde(gameObject);
        TypeEventSystem.Instance.Register<PickSilkEvent>(e =>
        {
            SetGoalPoint(_inSpaceSilk.transform.position);

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
                _awayFromEdgeStartPos = Vector3.zero;
                _awayFromEdgeEndPos = Vector3.zero;
            }
        }
    }

}
