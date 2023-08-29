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
        destroyInterval = 3.0f;

        GameObject temp = GameObject.Find("DropPointManager");
        //temp‚ªnull‚¾‚Á‚½‚çŒã‚ëÀs‚µ‚Ü‚¹‚ñB
        dpm = temp?.GetComponent<DropPointManager>();

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
