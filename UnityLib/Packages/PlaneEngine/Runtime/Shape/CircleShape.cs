using UnityEngine;

namespace PlaneEngine
{
    [AddComponentMenu("PlaneShape/Circle")]
    public class CircleShape : Shape
    {
        public Vector3 Offset;
        public float Radius = 1;

        public override void ToMesh(Mesh mesh)
        {
            MeshUtil.SetMeshByCircle(mesh, Offset, Radius);
        }
    }

}
