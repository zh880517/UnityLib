using UnityEngine;

namespace SharpRecast
{
    public static class RecastBuilder
    {
        public static Heightfield Creat(Bounds b, float size)
        {
            return new Heightfield(b, size);
        }

        public static void AddMesh(Heightfield heightfield, Mesh mesh, Matrix4x4 matrix)
        {
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;
            for (int i=0; i<triangles.Length; i+=3)
            {
                Vector3 v1 = vertices[triangles[i]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];
                heightfield.RasterizeTriangle(matrix.MultiplyPoint(v1), matrix.MultiplyPoint(v2), matrix.MultiplyPoint(v3));
            }
        }

        public static void AddMeshByRender(Heightfield heightfield, MeshRenderer renderer)
        {
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
                return;
            if (!renderer.bounds.Intersects(heightfield.Bounds))
                return;
            AddMesh(heightfield, meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix);
        }
    }

}
