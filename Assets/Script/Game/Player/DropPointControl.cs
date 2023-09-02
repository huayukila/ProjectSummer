using UnityEngine;

public class DropPointControl : MonoBehaviour
{
    
    GameObject pointPrefab;
    float dropInterval;
    float dropTimer;
    // Start is called before the first frame update
    void Start()
    {
        pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
        dropInterval = 0.1f;
        dropTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        dropTimer += Time.deltaTime;
        if (dropTimer >= dropInterval)
        {
            GameObject pt = Instantiate(pointPrefab,transform.position,transform.rotation);
            DropPointManager.Instance.AddPoint(pt);
            dropTimer = 0.0f;
        }

    }

}


