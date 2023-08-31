using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPoint : MonoBehaviour
{
    float destroyInterval;
    // Start is called before the first frame update
    void Start()
    {
        destroyInterval = 3.0f;

    }

    // Update is called once per frame
    void Update()
    {
        destroyInterval -= Time.deltaTime;
        if(destroyInterval <= 0.0f)
        {
            DropPointManager.Instance.DeletePoint(gameObject);
            Destroy(gameObject);
        }
    }


}
