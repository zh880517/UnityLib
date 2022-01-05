using UnityEditor;
using UnityEngine;

namespace PlaneEngine
{
    [CustomEditor(typeof(CircleSharp))]
    public class CircleSharpEditor : Editor
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
            var sharp = target as CircleSharp;
            sharp.Offset.y = 0;
            Vector3 pos = sharp.transform.position + sharp.Offset;
            pos.y = 0;
            using(new Handles.DrawingScope(Color.green))
            {
                Handles.DrawWireDisc(pos, Vector3.up, sharp.Radius);
                if (enableEditor)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        Vector3 normal = ControlPoints[i];
                        Vector3 pt = normal * sharp.Radius + pos;
                        EditorGUI.BeginChangeCheck();
                        float handleSize = HandleUtility.GetHandleSize(pt) * 0.05f;
                        pt = Handles.FreeMoveHandle(pt, Quaternion.identity, handleSize, normal, Handles.DotHandleCap);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(sharp, "modify circle");
                            EditorUtility.SetDirty(sharp);
                            Vector3 otherSide = normal * (-1 * sharp.Radius) + pos;
                            float radius = Vector3.Distance(pt, otherSide) * 0.5f;
                            sharp.Offset = sharp.Offset + normal * (radius - sharp.Radius);
                            sharp.Radius = radius;
                            sharp.SetDirty();
                        }
                    }
                }
            }
        }
    }

}
