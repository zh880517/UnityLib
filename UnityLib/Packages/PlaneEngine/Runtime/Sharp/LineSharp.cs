using System.Collections.Generic;
using UnityEngine;
namespace PlaneEngine
{
    [AddComponentMenu("PlaneSharp/Line")]
    public class LineSharp : Sharp
    {
        public List<Vector3> Points = new List<Vector3>() 
        {
            new Vector3(-1, 0, 0),
            new Vector3(1, 0, 0),
        };
        public override void ToMesh(Mesh mesh)
        {
            MeshUtil.SetMeshByLines(mesh, Points);
        }
    }
}