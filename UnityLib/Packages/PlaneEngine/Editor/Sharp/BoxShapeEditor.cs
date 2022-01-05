using UnityEditor;
using UnityEngine;
namespace PlaneEngine
{
    [CustomEditor(typeof(BoxShape))]
    public class BoxShapeEditor : Editor
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
            var shape = target as BoxShape;
            shape.Offset.y = 0;
            Vector3 pos = shape.transform.position;
            pos.y = 0;
            Quaternion rotation = PlaneUtils.TransRotation(shape.transform.rotation);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
            using (new Handles.DrawingScope(Color.green, matrix))
            {
                Handles.DrawWireCube(shape.Offset, new Vector3(shape.Size.x, 0, shape.Size.y));
                if (enableEditor)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        Vector3 normal = ControlPoints[i];
                        bool isX = (i % 2 == 0);
                        float size = shape.Size.y;
                        if (isX)
                            size = shape.Size.x;
                        size *= 0.5f;
                        Vector3 pt = normal * size + shape.Offset;
                        EditorGUI.BeginChangeCheck();
                        float handleSize = HandleUtility.GetHandleSize(pt) * 0.05f;
                        pt = Handles.FreeMoveHandle(pt, rotation, handleSize, normal, Handles.DotHandleCap);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(shape, "modify box");
                            EditorUtility.SetDirty(shape);
                            Vector3 otherSide = normal * (-1 * size) + shape.Offset;
                            float newSize = Vector3.Distance(pt, otherSide) * 0.5f;
                            shape.Offset += normal * (newSize - size);
                            if (isX)
                            {
                                shape.Size.x = newSize * 2;
                            }
                            else
                            {
                                shape.Size.y = newSize * 2;
                            }
                            shape.SetDirty();
                        }
                    }
                }
            }
        }
    }

}
