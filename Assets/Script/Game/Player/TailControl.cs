using UnityEngine;

public class TailControl : MonoBehaviour
{
    GameObject tailPrefab;   
    GameObject[] tails;
    int tailsCount;
    LineRenderer lr;
    // Start is called before the first frame update
    void Awake()
    {
        tailPrefab = (GameObject)Resources.Load("Prefabs/Tail");
<<<<<<< Updated upstream
        tails = new GameObject[Global.MAX_TAIL_COUNT];
=======
        tails = new GameObject[Global.iMAX_TAIL_COUNT];
>>>>>>> Stashed changes
        tails[0] = gameObject;
        ++tailsCount;
        GenerateTails();
        lr = gameObject.AddComponent<LineRenderer>();
        SetRendererProperties();

    }

    private void GenerateTails()
    {
<<<<<<< Updated upstream
        while(tailsCount < Global.MAX_TAIL_COUNT)
=======
        while(tailsCount < Global.iMAX_TAIL_COUNT)
>>>>>>> Stashed changes
        {
            GameObject tail = Instantiate(tailPrefab);
            tail.transform.localScale = Vector3.one * 0.2f;
            tail.transform.rotation = transform.rotation;
            tail.transform.position = transform.position;
            tails[tailsCount] = tail;
            ++tailsCount;
        }
    }

    private void Update()
    {
        SetLRPoints();
    }
    private void FixedUpdate()
    {
<<<<<<< Updated upstream
        for(int i = 1; i < Global.MAX_TAIL_COUNT; ++i)
=======
        for(int i = 1; i < Global.iMAX_TAIL_COUNT; ++i)
>>>>>>> Stashed changes
        {
            tails[^i].transform.position = tails[^(i+1)].transform.position;
            tails[^i].transform.rotation = tails[^(i+1)].transform.rotation;
        }
    }

    private void SetRendererProperties()
    {
        if (lr != null)
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.yellow;
            lr.startWidth = 0.2f;
            lr.endWidth = 0.5f;
<<<<<<< Updated upstream
            lr.positionCount = Global.MAX_TAIL_COUNT;
        }
    }

    private void SetLRPoints()
    {
        Vector3[] points = new Vector3[Global.MAX_TAIL_COUNT];
        for (int i = 0; i < Global.MAX_TAIL_COUNT; ++i)
        {
            points[i] = tails[i].transform.position;
        }
        lr.SetPositions(points);


=======
            lr.positionCount = Global.iMAX_TAIL_COUNT;
        }
>>>>>>> Stashed changes
    }

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
 
}
