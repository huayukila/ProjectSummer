using System.Collections.Generic;
using UnityEngine;

public class DropPointManager : Singleton<DropPointManager>
{
    List<GameObject> points;
    LineRenderer lr;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        points = new List<GameObject>(100);
        lr = gameObject.AddComponent<LineRenderer>();
        SetRendererProperties();
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
        if (lr != null)
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.startWidth = 0.5f;
            lr.endWidth = 0.5f;
        }
    }

    private Vector3[] GameObjectToVector3()
    {
        var temp = points;
        Vector3[] pos = new Vector3[temp.Count];
        for(int i = 0; i < temp.Count; ++i)
        {
            pos[i] = temp[i].transform.position;
        }
        return pos;
    }

    public List<Vector3> GetPaintablePoints(GameObject pt)
    {
        List<Vector3> temp = new List<Vector3>();
        foreach(GameObject ob in points)
        {
            if (ob != pt)
            {
                temp.Add(ob.transform.position);
            }
            else
            {
                break;
            }            
        }
        
        return temp;
    }
}
