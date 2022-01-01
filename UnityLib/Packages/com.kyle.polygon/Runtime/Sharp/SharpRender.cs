using UnityEngine;

namespace Polygon
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class SharpRender : MonoBehaviour
    {
        private Sharp sharp;
        private MeshFilter meshFilter;
        private MeshRenderer meshRender;
        void Start()
        {
            sharp = GetComponent<Sharp>();
            sharp.RefreshMesh();
            meshRender = gameObject.AddComponent<MeshRenderer>();
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = sharp.ShowMesh;
        }

        private void OnDestroy()
        {
            if (meshFilter)
                DestroyImmediate(meshFilter);
            if (meshRender)
                DestroyImmediate(meshRender);
        }

#if UNITY_EDITOR
        void Update()
        {
            sharp.RefreshMesh();
        }
#endif
    }

}
