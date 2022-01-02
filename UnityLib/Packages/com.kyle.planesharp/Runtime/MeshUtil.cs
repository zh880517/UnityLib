using UnityEngine;
namespace PlaneSharp
{
    public static class MeshUtil
    {
        private static readonly Vector3[] RectVertices = new Vector3[]
        {
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
        };
        public static void SetMeshByPolygon(Mesh mesh, Vector3[] points)
        {
            Triangulator triangulator = new Triangulator(points);
            mesh.triangles = null;
            Vector2[] uvs = new Vector2[points.Length];
            for (int i=0; i<points.Length; ++i)
            {
                Vector3 pt = points[i];
                uvs[i] = new Vector2(pt.x, pt.z);
            }
            mesh.triangles = triangulator.Triangulate();
            mesh.vertices = points;
            mesh.uv = uvs;
        }

        public static void SetMeshByCircle(Mesh mesh, Vector3 center, float radius)
        {
            radius = Mathf.Max(radius, 0.001f);
            center.y = 0;
            float perimeter = radius * Mathf.PI * 2;
            int segments = Mathf.CeilToInt(perimeter *4);
            if (segments % 2 == 1)
                segments++;
            segments = Mathf.Max(segments, 8);
            int verticesCount = segments + 1;
            int triangleCount = segments*3;
            Vector3[] vertices = new Vector3[verticesCount];
            Vector2[] uvs = new Vector2[verticesCount];
            int[] triangles = new int[triangleCount];
            vertices[0] = center;
            //计算点
            float angleRad = Mathf.Deg2Rad * 360;
            float angleCur = angleRad;
            float angledelta = angleRad / segments;

            for (int i=1; i < verticesCount; ++i)
            {
                vertices[i] = new Vector3(Mathf.Cos(angleCur) * radius, 0, Mathf.Sin(angleCur) * radius) + center;
                angleCur -= angledelta;
            }
            //计算三角形
            for (int i = 0, j=1; i < triangleCount; i+=3, j++)
            {
                triangles[i] = 0;
                triangles[i + 1] = j;
                triangles[i + 2] = j+1;
            }
            triangles[triangleCount - 3] = 0;
            triangles[triangleCount - 2] = verticesCount - 1;
            triangles[triangleCount - 1] = 1;
            for (int i=0; i<vertices.Length; ++i)
            {
                var pt = vertices[i];
                uvs[i] = new Vector2(pt.x, pt.z);
            }
            mesh.triangles = null;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
        }

        public static void SetMeshByBox(Mesh mesh, Vector3 center, Vector2 size)
        {
            center.y = 0;
            size.x = Mathf.Max(size.x, 0.001f);
            size.y = Mathf.Max(size.y, 0.001f);

            Vector3[] vertices = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            int[] triangles = new int[] {0, 1, 2, 0, 2, 3 };

            for (int i=0; i < 4; ++i)
            {
                var pt = Vector3.Scale(RectVertices[i], new Vector3(size.x, 1, size.y)) + center;
                vertices[i] = pt;
                uvs[i] = new Vector2(pt.x, pt.z);
            }
            mesh.triangles = null;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
        }
    }
}
