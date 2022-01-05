using UnityEditor;
using UnityEngine;
namespace PlaneEngine
{
    [CustomEditor(typeof(LineShape))]
    public class LineShapeEditor : Editor
    {
        private Vector3[] worldPoints;
        public override void OnInspectorGUI()
        {
            ShapeEditorUtil.DefaultInspectorGUI(this);
            GUILayout.Label("Scene视图操作：");
            GUILayout.Label("按Shift点击在鼠标位置创建点");
            GUILayout.Label("按Control点击红色的点删除");
            var shape = target as LineShape;
            if (shape.Points.Count < 2)
            {
                GUILayout.Label("线段的点不能少于2个", "Wizard Error");
            }
        }

        private void OnSceneGUI()
        {
            bool enableEditor = targets.Length == 1;
            var shape = target as LineShape;
            if (worldPoints == null || worldPoints.Length != shape.Points.Count)
                worldPoints = new Vector3[shape.Points.Count];
            Vector3 pos = shape.transform.position;
            pos.y = 0;
            Quaternion rotation = PlaneUtils.TransRotation(shape.transform.rotation);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
            for (int i = 0; i < shape.Points.Count; ++i)
            {
                worldPoints[i] = matrix.MultiplyPoint(shape.Points[i]);
            }
            //计算鼠标点击在XZ平面的点
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (!PlaneUtils.RayCastPlaneXZ(ray, out Vector3 mousePos))
                return;//鼠标没有点击在XZ平面
            var result = PlaneUtils.ClosestPointToLine(mousePos, worldPoints);
            Vector3 closePoint = result.Point;
            int index = result.Index;
            using (new Handles.DrawingScope(Color.green))
            {
                //画边缘线
                for (int i = 0; i < worldPoints.Length; ++i)
                {
                    int nextIndex = i + 1;
                    Vector3 pt = worldPoints[i];
                    if (nextIndex < worldPoints.Length)
                        Handles.DrawLine(pt, worldPoints[nextIndex]);
                    if (enableEditor)
                    {
                        //点位置编辑
                        float handleSize = HandleUtility.GetHandleSize(pt) * 0.05f;
                        EditorGUI.BeginChangeCheck();
                        pt = Handles.FreeMoveHandle(pt, Quaternion.identity, handleSize, new Vector3(1, 0, 1), Handles.DotHandleCap);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(shape, "move polygon point");
                            EditorUtility.SetDirty(shape);
                            pt = matrix.inverse.MultiplyPoint(pt);
                            pt.y = 0;
                            worldPoints[i] = pt;
                            shape.Points[i] = pt;
                            shape.SetDirty();
                        }
                    }
                }
                if (!enableEditor)
                    return;
                if (Event.current.control)
                {
                    if (worldPoints.Length > 2)
                    {
                        int removeIndex = index;
                        if (result.NormalLength >= 0.001f)
                            removeIndex = index + 1;
                        if (index == worldPoints.Length - 2 && result.NormalLength < 0.999f)
                        {
                            removeIndex = index;
                        }
                        int preIndex = removeIndex - 1;
                        int nextIndex = removeIndex + 1;
                        if (preIndex >= 0 && nextIndex < worldPoints.Length)
                        {
                            Handles.DrawLine(worldPoints[preIndex], worldPoints[nextIndex]);
                        }
                        using (new Handles.DrawingScope(Color.red))
                        {
                            if (preIndex >= 0)
                                Handles.DrawAAPolyLine(5, worldPoints[preIndex], worldPoints[removeIndex]);
                            if (nextIndex < worldPoints.Length)
                                Handles.DrawAAPolyLine(5, worldPoints[removeIndex], worldPoints[nextIndex]);

                            float handleSize = HandleUtility.GetHandleSize(worldPoints[removeIndex]) * 0.05f;
                            var pt1 = HandleUtility.WorldToGUIPoint(worldPoints[removeIndex]);
                            var pt2 = HandleUtility.WorldToGUIPoint(mousePos);
                            float pickSize = Vector2.Distance(pt1, pt2) + 0.1f;
                            if (Handles.Button(worldPoints[removeIndex], Quaternion.identity, handleSize, pickSize, Handles.DotHandleCap))
                            {
                                Undo.RecordObject(shape, "delete line point");
                                EditorUtility.SetDirty(shape);
                                shape.Points.RemoveAt(removeIndex);
                                shape.SetDirty();
                                Event.current.Use();
                            }
                        }
                    }
                }
                else if(Event.current.shift)
                {
                    int insertIndex = index + 1;
                    if (index == 0 && result.NormalLength <= 0.001f)
                    {
                        insertIndex = 0;
                    }
                    else if (index == worldPoints.Length - 2 && result.NormalLength >= 0.999f)
                    {
                        insertIndex = worldPoints.Length;
                    }
                    if ((result.SegmentLength * result.NormalLength) > 0.1f && (result.SegmentLength * result.NormalLength) < (result.SegmentLength - 0.1f))
                    {
                        //画距离鼠标最近的点
                        Handles.SphereHandleCap(0, closePoint, Quaternion.identity, 0.05f, Event.current.type);
                    }
                    if (Event.current.shift)
                    {
                        using (new Handles.DrawingScope(Color.white))
                        {
                            int preIndex = insertIndex - 1;
                            if (preIndex >= 0)
                                Handles.DrawLine(mousePos, worldPoints[preIndex]);
                            if (insertIndex < worldPoints.Length)
                                Handles.DrawLine(mousePos, worldPoints[insertIndex]);
                            float handleSize = HandleUtility.GetHandleSize(mousePos) * 0.05f;
                            if (Handles.Button(mousePos, Quaternion.identity, 0.1f, 0.1f, Handles.DotHandleCap))
                            {
                                Undo.RecordObject(shape, "add line point");
                                EditorUtility.SetDirty(shape);

                                var pt = matrix.inverse.MultiplyPoint(mousePos);
                                shape.Points.Insert(insertIndex, pt);
                                shape.SetDirty();
                                Event.current.Use();
                            }
                        }
                    }
                }
            }

            SceneView.RepaintAll();
        }
    }
}