using System.Collections.Generic;
using UnityEngine;
namespace PlaneSharp
{
    [AddComponentMenu("Polygon/Polygon")]
    public class PolySharp : Sharp
    {
        public List<Vector3> Points = new List<Vector3>() 
        {
            new Vector3(0, 0, 1),
            new Vector3(-0.9510565f, 0, 0.309017f),
            new Vector3(-0.5877852f, 0, -0.8090171f),
            new Vector3(0.5877854f, 0, -0.8090169f),
            new Vector3(0.9510565f, 0, 0.3090171f),
        };

        public override void ToMesh(Mesh mesh)
        {
            MeshUtil.SetMeshByPolygon(mesh, Points.ToArray());
        }
    }
}