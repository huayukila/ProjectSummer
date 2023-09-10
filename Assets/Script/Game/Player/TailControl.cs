using UnityEngine;

public class TailControl : MonoBehaviour
{
    private GameObject _tailPrefab;         // �K���̃v���n�u
    private GameObject[] _tails;            // �K���̐߂�����z��
    private LineRenderer _lr;               // �K����`�悷��LineRenderer

    /// <summary>
    /// �K���̐߂��C���X�^���X������
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
    /// LineRenderer�̃v���p�e�B��ݒ肷��
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
    /// LineRenderer�̒��_��ݒ肷��
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
    /// �K��������ۂ�GameObject�ɓ����iHierarchy�����ꂢ�ɂ��邽�߁j
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
    /// �K���̈ʒu�����Z�b�g����
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
    /// �K����Tag��ݒ肷��
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
    /// �K�����A�N�e�B�u�ɐݒ肷��
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
    /// �K�����A�N�e�B�u�ɐݒ肷��
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
    /// �K���̐߂�S���Ԃ�
    /// </summary>
    /// <returns></returns>
    public GameObject[] GetTails() => _tails;

    /// <summary>
    /// �K���̍Ō�̐߂�Ԃ�
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
        // �K���̌�납��S�Ă̐߂̃��[���h���W���X�V����
        for(int i = 1; i < Global.iMAX_TAIL_COUNT; ++i)
        {
            _tails[^i].transform.position = _tails[^(i+1)].transform.position;
            _tails[^i].transform.rotation = _tails[^(i+1)].transform.rotation;
        }
    }

    
}
