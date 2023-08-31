using System.Collections.Generic;
using UnityEngine;

public class TailRender : MonoBehaviour
{
    List<Vector3> points;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector3>(Global.MAX_TAIL_COUNT);
        lr = gameObject.AddComponent<LineRenderer>();
        SetRendererProperties();
    }

    // Update is called once per frame
    void Update()
    {
        SetLRPoints();
        lr?.SetPositions(points.ToArray());
    }

    private void SetLRPoints()
    {
        TailControl tempTails = GetComponent<TailControl>();
        if (tempTails != null)
        {
            GameObject[] temp = tempTails.GetTailsObject();
            points.Clear();
            for (int i = 0; i < temp.Length; ++i)
            {
                points.Add(temp[i].transform.position);
            }
            lr.positionCount = points.Count;
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
        }
    }
}
