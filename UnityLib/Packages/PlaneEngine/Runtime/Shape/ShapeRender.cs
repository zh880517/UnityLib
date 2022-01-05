using UnityEngine;

namespace PlaneEngine
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ShapeRender : MonoBehaviour
    {
        private Shape shape;
        private MeshFilter meshFilter;
        private MeshRenderer meshRender;
        void Start()
        {
            shape = GetComponent<Shape>();
            shape.RefreshMesh();
#if UNITY_EDITOR
            meshFilter = UnityEditor.Undo.AddComponent<MeshFilter>(gameObject);
            meshRender = UnityEditor.Undo.AddComponent<MeshRenderer>(gameObject);
#else
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRender = gameObject.AddComponent<MeshRenderer>();
#endif
            meshFilter.sharedMesh = shape.ShowMesh;
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (meshFilter)
                UnityEditor.Undo.DestroyObjectImmediate(meshFilter);
            if (meshRender)
                UnityEditor.Undo.DestroyObjectImmediate(meshRender);
#else
            if (meshFilter)
                DestroyImmediate(meshFilter);
            if (meshRender)
                DestroyImmediate(meshRender);
#endif
        }

#if UNITY_EDITOR
        void Update()
        {
            shape.RefreshMesh();
        }
#endif
    }

}
