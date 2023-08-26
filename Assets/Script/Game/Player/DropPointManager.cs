using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPointManager : MonoBehaviour
{
    List<GameObject> points;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        points = new List<GameObject>();
        lr = gameObject.AddComponent<LineRenderer>();
        if(lr != null)
        {
            SetRendererProperties();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(points.Count > 1)
        {
            lr.positionCount = points.Count;
            lr.SetPositions(GameObjectToVector3());
        }

    }

    private void Update()
    {
        
    }

    public void AddPoint(GameObject ob)
    {
        points.Add(ob);
    }

    public void DeletePoint(GameObject ob)
    {
        points.Remove(ob);
    }

    private void SetRendererProperties()
    {
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.startWidth = 0.5f;
        lr.endWidth = 0.5f;
    }

    private Vector3[] GameObjectToVector3()
    {
        Vector3[] temp = new Vector3[points.Count];
        for(int i = 0; i < points.Count; ++i)
        {
            temp[i] = points[i].transform.position;
        }
        return temp;
    }
}
