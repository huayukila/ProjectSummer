using UnityEngine;

namespace Math
{
    public static class VectorMath
    {
        public static float PointOfLine(Vector3 point, Vector3 endp1, Vector3 endp2)
        {
            return (endp1.x - point.x) * (endp2.z - point.z) - (endp2.x - point.x) * (endp1.z - point.z);
        }

        public static bool IsParallel(Vector3 l1e1, Vector3 l1e2, Vector3 l2e1, Vector3 l2e2)
        {
            Vector3 line1 = l1e2 - l1e1;
            Vector3 line2 = l2e2 - l2e1;
            return Mathf.Abs((line1.x * line2.z) - (line1.z * line2.x)) <= 0.001f;
        }

        public static Vector3 GetCrossPoint(Vector3 l1e1, Vector3 l1e2, Vector3 l2e1, Vector3 l2e2)
        {
            float x = 0.0f;
            float z = 0.0f;
            if (l1e1.x == l1e2.x)
            {
                x = l1e1.x;
                z = (l2e2.z - l2e1.z) / (l2e2.x - l2e1.x) * (x - l2e1.x) + l2e1.z;
            }
            else if (l2e1.x == l2e2.x)
            {
                x = l2e1.x;
                z = (l1e2.z - l1e1.z) / (l1e2.x - l1e1.x) * (x - l1e1.x) + l1e1.z;
            }
            else
            {
                float a1 = (l1e2.z - l1e1.z) / (l1e2.x - l1e1.x);
                float a2 = (l2e2.z - l2e1.z) / (l2e2.x - l2e1.x);

                x = (a1 * l1e1.x - a2 * l2e1.x - l1e1.z + l2e1.z) / (a1 - a2);
                z = a1 * x - a1 * l1e1.x + l1e1.z;
            }

            return new(x, 0.64f, z);
        }

        //TODO need understanding
        public static bool InPolygon(Vector3 point, Vector3[] Vertexs)
        {
            int vertCount = Vertexs.Length;
            bool ret = false;
            if (vertCount >= 3)
            {
                for (int i = 0, j = vertCount - 1; i < vertCount; j = i++)
                {
                    if (((Vertexs[i].z > point.z) != (Vertexs[j].z > point.z)) &&
                        (point.x < (Vertexs[j].x - Vertexs[i].x) * (point.z - Vertexs[i].z) / (Vertexs[j].z - Vertexs[i].z) + Vertexs[i].x))
                        ret = !ret;
                }
            }
            return ret;
        }
    }

}
