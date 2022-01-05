using System.Collections.Generic;
using UnityEngine;
namespace PlaneEngine
{
    [AddComponentMenu("PlaneShape/Polygon")]
    public class PolyShape : Shape
    {
        public List<Vector3> Points = new List<Vector3>() 
        {
            new Vector3(0, 0, 2),
            new Vector3(1.902113f, 0, 0.6180342f),
            new Vector3(1.175571f, 0, -1.618034f),
            new Vector3(-1.17557f, 0, -1.618034f),
            new Vector3(-1.902113f, 0, 0.618034f),
        };

        public override void ToMesh(Mesh mesh)
        {
            MeshUtil.SetMeshByPolygon(mesh, Points.ToArray());
        }
    }
}