using UnityEngine;

namespace PlaneSharp
{
    [AddComponentMenu("Polygon/Circle")]
    public class CircleSharp : Sharp
    {
        public Vector3 Offset;
        public float Radius = 1;

        public override void ToMesh(Mesh mesh)
        {
            MeshUtil.SetMeshByCircle(mesh, Offset, Radius);
        }
    }

}
