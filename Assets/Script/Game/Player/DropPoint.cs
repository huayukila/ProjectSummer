using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPoint : MonoBehaviour
{
    float destroyInterval;
    DropPointManager dpm;
    // Start is called before the first frame update
    void Start()
    {
        destroyInterval = 5.0f;
        GameObject temp = GameObject.Find("DropPointManager");
        if(temp != null)
        {
            dpm = temp.GetComponent<DropPointManager>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        destroyInterval -= Time.deltaTime;
        if(destroyInterval <= 0.0f)
        {
            dpm.DeletePoint(gameObject);
            Destroy(gameObject);
        }
    }


}
