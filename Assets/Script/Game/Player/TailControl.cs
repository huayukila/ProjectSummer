using UnityEngine;

public class TailControl : MonoBehaviour
{
    GameObject tailPrefab;   
    GameObject[] tails;
    int tailsCount;
    LineRenderer lr;

    /// <summary>
    /// �K���̐߂��C���X�^���X������
    /// </summary>
    private void GenerateTails()
    {
        while(tailsCount < Global.iMAX_TAIL_COUNT)
        {
            GameObject tail = Instantiate(tailPrefab);
            tail.transform.localScale = Vector3.one * 0.2f;
            tail.transform.rotation = transform.rotation;
            tail.transform.position = transform.position;
            tails[tailsCount] = tail;
            ++tailsCount;
        }
    }

    /// <summary>
    /// LineRenderer�̃v���p�e�B��ݒ肷��
    /// </summary>
    private void SetRendererProperties()
    {
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.yellow;
        lr.startWidth = 0.2f;
        lr.endWidth = 0.5f;
        lr.positionCount = Global.iMAX_TAIL_COUNT;
    }

    /// <summary>
    /// LineRenderer�̒��_��ݒ肷��
    /// </summary>
    private void SetLRPoints()
    {
        Vector3[] points = new Vector3[Global.iMAX_TAIL_COUNT];
        for (int i = 0; i < Global.iMAX_TAIL_COUNT; ++i)
        {
            points[i] = tails[i].transform.position;
        }
        lr.SetPositions(points);

    }

    public GameObject[] GetTails() => tails;

    public GameObject GetTipTail() => tails[tails.Length - 1];
    void Awake()
    {
        tailPrefab = (GameObject)Resources.Load("Prefabs/Tail");
        tails = new GameObject[Global.iMAX_TAIL_COUNT];
        tails[0] = gameObject;
        ++tailsCount;
        GenerateTails();
        lr = gameObject.AddComponent<LineRenderer>();
        SetRendererProperties();

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
            tails[^i].transform.position = tails[^(i+1)].transform.position;
            tails[^i].transform.rotation = tails[^(i+1)].transform.rotation;
        }
    }



}
