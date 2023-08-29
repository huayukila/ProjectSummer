using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPointControl : MonoBehaviour
{
    
    GameObject pointPrefab;
    float dropInterval;
    float dropTimer;
    DropPointManager dpm;
    // Start is called before the first frame update
    void Start()
    {
        pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
        dropInterval = 0.1f;
        dropTimer = 0.0f;
        GameObject temp = GameObject.Find("DropPointManager");
        dpm = temp?.GetComponent<DropPointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        dropTimer += Time.deltaTime;
        if (dropTimer >= dropInterval)
        {
            if(dpm != null)
            {
                GameObject pt = Instantiate(pointPrefab,transform.position,transform.rotation);
                dpm.AddPoint(pt);
            }
            dropTimer = 0.0f;
        }

    }

}


