using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailRender : MonoBehaviour
{
    Vector3[] points;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = gameObject.AddComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetLRPoints();
        if(lr != null)
        {
            lr.SetPositions(points);
        }
    }

    private void SetLRPoints()
    {
        TailControl tempTails = GetComponentInParent<TailControl>();
        if (tempTails != null)
        {
            GameObject[] temp = tempTails.GetTailsObject();
            points = new Vector3[temp.Length];
            lr.positionCount = points.Length;
            for (int i = 0; i < temp.Length; ++i)
            {
                points[i] = temp[i].transform.position;
            }
        }
        
    }
}
