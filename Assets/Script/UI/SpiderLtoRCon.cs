using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLtoRCon : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= 1200)
        {
            transform.Translate(40.0f, 0, 0);
        }
    }
}
