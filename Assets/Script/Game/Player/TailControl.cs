using UnityEngine;

public class TailControl : MonoBehaviour
{
    private GameObject _tailPrefab;         // 尻尾のプレハブ
    private GameObject[] _tails;            // 尻尾の節を入れる配列
    private LineRenderer _lr;               // 尻尾を描画するLineRenderer

    /// <summary>
    /// 尻尾の節をインスタンス化する
    /// </summary>
    private void GenerateTails()
    {
        int tailsIndex = 1;
        while (tailsIndex < Global.iMAX_TAIL_COUNT)
        {
            GameObject tail = Instantiate(_tailPrefab);
            tail.name = name;
            tail.transform.localScale = Vector3.one * 0.2f;
            tail.transform.rotation = transform.rotation;
            tail.transform.position = transform.position;
            _tails[tailsIndex++] = tail;
        }
    }

    /// <summary>
    /// LineRendererのプロパティを設定する
    /// </summary>
    private void SetRendererProperties()    
    {
        _lr.material = new Material(Shader.Find("Sprites/Default"));
        _lr.startColor = Color.red;
        _lr.endColor = Color.yellow;
        _lr.startWidth = 0.2f;
        _lr.endWidth = 0.5f;
        _lr.positionCount = Global.iMAX_TAIL_COUNT;
    }

    /// <summary>
    /// LineRendererの頂点を設定する
    /// </summary>
    private void SetLRPoints()
    {
        Vector3[] points = new Vector3[Global.iMAX_TAIL_COUNT];
        for (int i = 0; i < Global.iMAX_TAIL_COUNT; ++i)
        {
            points[i] = _tails[i].transform.position;
        }
        _lr.SetPositions(points);

    }

    /// <summary>
    /// 尻尾を空っぽのGameObjectに入れる（Hierarchyをきれいにするため）
    /// </summary>
    private void SetTailsParent()
    {
        GameObject tailParent = new GameObject(name + "s");
        foreach (GameObject t in _tails)
        {
            t.transform.parent = tailParent.transform;
        }
    }

    /// <summary>
    /// 尻尾の位置をリセットする
    /// </summary>
    /// <param name="pos"></param>
    private void ResetTransform(Vector3 pos)
    {
        foreach(GameObject t in _tails)
        {
            t.transform.position = pos;
        }
    }

    /// <summary>
    /// 尻尾のTagを設定する
    /// </summary>
    /// <param name="tag"></param>
    public void SetTailsTag(string tag)
    {
        foreach(GameObject t in _tails)
        {
            t.tag = tag;
        }
    }

    /// <summary>
    /// 尻尾を非アクティブに設定する
    /// </summary>
    public void SetDeactiveProperties()
    {
        foreach (GameObject ob in _tails)
        {
            ob.SetActive(false);
        }
        _lr.enabled = false;
    }

    /// <summary>
    /// 尻尾をアクティブに設定する
    /// </summary>
    /// <param name="pos"></param>
    public void SetActiveProperties(Vector3 pos)
    {
        foreach (GameObject ob in _tails)
        {
            ob.SetActive(true);
        }
        ResetTransform(pos);
        SetLRPoints();
        _lr.enabled = true;

    }

    /// <summary>
    /// 尻尾の節を全部返す
    /// </summary>
    /// <returns></returns>
    public GameObject[] GetTails() => _tails;

    /// <summary>
    /// 尻尾の最後の節を返す
    /// </summary>
    /// <returns></returns>
    public GameObject GetTipTail() => _tails[_tails.Length - 1];
    private void Awake()
    {
        _tailPrefab = (GameObject)Resources.Load("Prefabs/Tail");
        _tails = new GameObject[Global.iMAX_TAIL_COUNT];
        _tails[0] = gameObject;
        _lr = gameObject.AddComponent<LineRenderer>();

        GenerateTails();
        SetRendererProperties();
        SetTailsParent();
    }
    private void Update()
    {
        SetLRPoints();
    }
    private void FixedUpdate()
    {
        // 尻尾の後ろから全ての節のワールド座標を更新する
        for(int i = 1; i < Global.iMAX_TAIL_COUNT; ++i)
        {
            _tails[^i].transform.position = _tails[^(i+1)].transform.position;
            _tails[^i].transform.rotation = _tails[^(i+1)].transform.rotation;
        }
    }

    
}
