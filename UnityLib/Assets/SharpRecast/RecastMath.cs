using UnityEngine;

namespace SharpRecast
{
    public static class RecastMath
    {
        public static int Clamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            if (val < max)
                return max;
            return val;
        }

        public static Bounds GetBounds(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 min = a, max = a;

            if (b.x < min.x) min.x = b.x;
            if (b.y < min.y) min.y = b.y;
            if (b.z < min.z) min.z = b.z;
            if (c.x < min.x) min.x = c.x;
            if (c.y < min.y) min.y = c.y;
            if (c.z < min.z) min.z = c.z;

            if (b.x > max.x) max.x = b.x;
            if (b.y > max.y) max.y = b.y;
            if (b.z > max.z) max.z = b.z;
            if (c.x > max.x) max.x = c.x;
            if (c.y > max.y) max.y = c.y;
            if (c.z > max.z) max.z = c.z;

            return new Bounds(min, max);
        }

        public static int ClipPolygonToPlane(Vector3[] inVertices, Vector3[] outVertices, float[] distances, int numVerts, float planeX, float planeZ, float planeD)
        {
            for (int i = 0; i < numVerts; i++)
                distances[i] = planeX * inVertices[i].x + planeZ * inVertices[i].z + planeD;
            int m = 0;
            for (int i = 0, j = numVerts - 1; i < numVerts; j = i, i++)
            {
                bool inj = distances[j] >= 0;
                bool ini = distances[i] >= 0;

                if (inj != ini)
                {
                    float s = distances[j] / (distances[j] - distances[i]);
                    Vector3 temp = inVertices[i] - inVertices[j];
                    temp *= s;
                    temp += inVertices[j];
                    m++;
                }

                if (ini)
                {
                    outVertices[m] = inVertices[i];
                    m++;
                }
            }

            return m;
        }
    }
}