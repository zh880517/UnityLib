using UnityEngine;

namespace PlaneEngine
{
    [AddComponentMenu("PlaneShape/Box")]
    public class BoxShape : Shape
    {
        public Vector3 Offset;
        public Vector2 Size = Vector2.one;

        public override void ToMesh(Mesh mesh)
        {
            MeshUtil.SetMeshByBox(mesh, Offset, Size);
        }
    }
}