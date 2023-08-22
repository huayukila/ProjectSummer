using UnityEngine;

public class RayCheck
{
    public Vector3 checkOffset;
    public float checkDistance;
    public LayerMask checkLayer;

    public bool Shoot(Vector3 origion, Vector3 dir, out RaycastHit hit)
    {
        Ray ray = new Ray(origion + checkOffset, dir);
        return Physics.Raycast(ray, out hit, checkDistance, checkLayer);
    }

    public void DrawGizmos(Vector3 origion, float radius)
    {
        Gizmos.DrawSphere(origion + checkOffset, radius);
    }

}
