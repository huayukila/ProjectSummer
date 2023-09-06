using UnityEngine;

public class TailControl : MonoBehaviour
{
    GameObject _tailPrefab;
    GameObject[] _tails;
    int _tailsCount;
    LineRenderer _lr;

    /// <summary>
    /// 尻尾の節をインスタンス化する
    /// </summary>
    private void GenerateTails()
    {
        while (_tailsCount < Global.iMAX_TAIL_COUNT)
        {
            GameObject tail = Instantiate(_tailPrefab);
            tail.name = name;
            tail.transform.localScale = Vector3.one * 0.2f;
            tail.transform.rotation = transform.rotation;
            tail.transform.position = transform.position;
            _tails[_tailsCount++] = tail;
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

    private void SetTailsParent()
    {
        GameObject tailParent = new GameObject(name + "s");
        foreach (GameObject t in _tails)
        {
            t.transform.parent = tailParent.transform;
        }
    }

    private void ResetTransform(Vector3 pos)
    {
        foreach(GameObject t in _tails)
        {
            t.transform.position = pos;
        }
    }

    public void SetTailsTag(string tag)
    {
        foreach(GameObject t in _tails)
        {
            t.tag = tag;
        }
    }

    public void SetDeactive()
    {
        foreach (GameObject ob in _tails)
        {
            ob.SetActive(false);
        }
        _lr.enabled = false;
    }

    public void SetActive(Vector3 pos)
    {
        foreach (GameObject ob in _tails)
        {
            ob.SetActive(true);
        }
        ResetTransform(pos);
        SetLRPoints();
        _lr.enabled = true;

    }
    public GameObject[] GetTails() => _tails;

    public GameObject GetTipTail() => _tails[_tails.Length - 1];
    private void Awake()
    {
        _tailPrefab = (GameObject)Resources.Load("Prefabs/Tail");
        _tails = new GameObject[Global.iMAX_TAIL_COUNT];
        _tails[0] = gameObject;
        ++_tailsCount;
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
