using UnityEngine;

namespace PaintOnObject
{
    public class MousePainter : Painter
    {
        public Camera cam;

        public LayerMask checkLayer;

        void Update()
        {
            bool click;
            click = Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);

            if (click)
            {
                Vector3 position = Input.mousePosition;
                Ray ray = cam.ScreenPointToRay(position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100.0f, checkLayer))
                {
                    Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
                    transform.position = hit.point;
                    Paintable p = hit.collider.GetComponent<Paintable>();
                    if (p != null)
                    {
                        PaintManager.Instance.Paint(p, hit.point, radius, hardness, strength, color);
                    }
                }
            }

        }
    }
}