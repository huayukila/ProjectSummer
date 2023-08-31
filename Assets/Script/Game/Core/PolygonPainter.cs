using System.Collections.Generic;
using UnityEngine;
public class PolygonPainter : MonoBehaviour
{
    public LayerMask checkLayer;

    public Paintable p;

    private List<Vector3> verts = new List<Vector3>();

    private void Start()
    {
    }
    private void Update()
    {
        transform.position = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, checkLayer))
            {
                verts.Add(hit.point);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PolygonPaintManager.Instance.Paint(p, verts.ToArray());
            verts.Clear();
        }
    }
}
