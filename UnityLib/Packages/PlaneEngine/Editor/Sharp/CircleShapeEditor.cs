using UnityEditor;
using UnityEngine;

namespace PlaneEngine
{
    [CustomEditor(typeof(CircleShape))]
    public class CircleShapeEditor : Editor
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
            ShapeEditorUtil.DefaultInspectorGUI(this);
        }

        private void OnSceneGUI()
        {
            bool enableEditor = targets.Length == 1;
            var shape = target as CircleShape;
            shape.Offset.y = 0;
            Vector3 pos = shape.transform.position + shape.Offset;
            pos.y = 0;
            using(new Handles.DrawingScope(Color.green))
            {
                Handles.DrawWireDisc(pos, Vector3.up, shape.Radius);
                if (enableEditor)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        Vector3 normal = ControlPoints[i];
                        Vector3 pt = normal * shape.Radius + pos;
                        EditorGUI.BeginChangeCheck();
                        float handleSize = HandleUtility.GetHandleSize(pt) * 0.05f;
                        pt = Handles.FreeMoveHandle(pt, Quaternion.identity, handleSize, normal, Handles.DotHandleCap);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(shape, "modify circle");
                            EditorUtility.SetDirty(shape);
                            Vector3 otherSide = normal * (-1 * shape.Radius) + pos;
                            float radius = Vector3.Distance(pt, otherSide) * 0.5f;
                            shape.Offset = shape.Offset + normal * (radius - shape.Radius);
                            shape.Radius = radius;
                            shape.SetDirty();
                        }
                    }
                }
            }
        }
    }

}
