using UnityEditor;
using UnityEngine;
namespace PlaneSharp
{
    [CustomEditor(typeof(BoxSharp))]
    public class BoxSharpEditor : Editor
    {
        private static readonly Vector3[] ControlPoints = new Vector3[]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 0, -1),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
        };

        public override void OnInspectorGUI()
        {
            SharpEditorUtil.DefaultInspectorGUI(this);
        }
        private void OnSceneGUI()
        {
            bool enableEditor = targets.Length == 1;
            var sharp = target as BoxSharp;
            sharp.Offset.y = 0;
            Vector3 pos = sharp.transform.position;
            pos.y = 0;
            Quaternion rotation = PlaneUtils.TransRotation(sharp.transform.rotation);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
            using (new Handles.DrawingScope(Color.green, matrix))
            {
                Handles.DrawWireCube(sharp.Offset, new Vector3(sharp.Size.x, 0, sharp.Size.y));
                if (enableEditor)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        Vector3 normal = ControlPoints[i];
                        bool isX = (i % 2 == 0);
                        float size = sharp.Size.y;
                        if (isX)
                            size = sharp.Size.x;
                        size *= 0.5f;
                        Vector3 pt = normal * size + sharp.Offset;
                        EditorGUI.BeginChangeCheck();
                        float handleSize = HandleUtility.GetHandleSize(pt) * 0.05f;
                        pt = Handles.FreeMoveHandle(pt, rotation, handleSize, normal, Handles.DotHandleCap);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(sharp, "modify box");
                            EditorUtility.SetDirty(sharp);
                            Vector3 otherSide = normal * (-1 * size) + sharp.Offset;
                            float newSize = Vector3.Distance(pt, otherSide) * 0.5f;
                            sharp.Offset += normal * (newSize - size) * 0.5f;
                            if (isX)
                            {
                                sharp.Size.x = newSize * 2;
                            }
                            else
                            {
                                sharp.Size.y = newSize * 2;
                            }
                            sharp.SetDirty();
                        }
                    }
                }
            }
        }
    }

}
