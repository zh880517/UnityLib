using UnityEngine;

namespace PlaneEngine
{
    [DisallowMultipleComponent]
    public abstract class Sharp : MonoBehaviour, ISerializationCallbackReceiver
    {
        public PolyType Type;
        public Mesh ShowMesh { get; private set; }
        private bool isDirty = true;

        public void SetDirty()
        {
            isDirty = true;
            if (GetComponent<SharpRender>() == null && ShowMesh)
            {
                RefreshMesh();
            }
        }

        public abstract void ToMesh(Mesh mesh);

        public void RefreshMesh()
        {
            if (ShowMesh == null)
            {
                ShowMesh = new Mesh();
            }
            if (isDirty)
            {
                ToMesh(ShowMesh);
                ShowMesh.RecalculateNormals();
                isDirty = false;
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            RefreshMesh();
            if (GetComponent<SharpRender>())
                return;
            Vector3 pos = transform.position;
            pos.y = 0;
            Color color = Gizmos.color;
            switch (Type)
            {
                case PolyType.Area:
                    Gizmos.color = new Color(0, 1, 0, 0.5f);
                    break;
                case PolyType.Hollow:
                    pos.y = 0.01f;
                    Gizmos.color = new Color(1f, 0.921568632f, 0.0156862754f, 0.5f);
                    break;
                case PolyType.Obstacle:
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    pos.y = 0.02f;
                    break;
            }
            Gizmos.DrawMesh(ShowMesh, pos, PlaneUtils.TransRotation(transform.rotation));
            Gizmos.color = color;
        }
#endif
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            isDirty = true;
        }
    }
}
