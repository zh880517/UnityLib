using UnityEditor;
using UnityEngine;
namespace PlaneSharp
{
    [CustomEditor(typeof(PolySharp))]
    public class PolySharpEditor : Editor
    {
        private Vector3[] worldPoints;
        public override void OnInspectorGUI()
        {
            SharpEditorUtil.DefaultInspectorGUI(this);
            GUILayout.Label("Scene视图操作：");
            GUILayout.Label("按Shift点击在鼠标位置创建点");
            GUILayout.Label("按Control点击红色的点删除");
            var sharp = target as PolySharp;
            if (sharp.Points.Count < 3)
            {
                GUILayout.Label("多边形的点不能少于3个", "Wizard Error");
            }
        }
        private void OnSceneGUI()
        {
            var sharp = target as PolySharp;
            if (worldPoints == null || worldPoints.Length != sharp.Points.Count)
                worldPoints = new Vector3[sharp.Points.Count];
            Vector3 pos = sharp.transform.position;
            pos.y = 0;
            Vector3 eulerAngles = sharp.transform.rotation.eulerAngles;
            Quaternion rotation = Quaternion.Euler(0, eulerAngles.y, 0);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
            for (int i = 0; i < sharp.Points.Count; ++i)
            {
                worldPoints[i] = matrix.MultiplyPoint(sharp.Points[i]);
            }
            //计算鼠标点击在XZ平面的点
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            float dot1 = Vector3.Dot(ray.direction, Vector3.up);
            if (dot1 == 0)//鼠标没有点击在XZ平面
                return;
            float d = Vector3.Dot(-ray.origin, Vector3.up) / dot1;
            Vector3 mousePos = d * ray.direction + ray.origin;
            
            var result = PolygonUtil.ClosestPointToPolyLine(mousePos, worldPoints);
            Vector3 closePoint = result.Point;
            int index = result.Index;

            using (new Handles.DrawingScope(Color.green))
            {
                //画边缘线
                for (int i=0; i< worldPoints.Length; ++i)
                {
                    int nextIndex = i + 1;
                    if (nextIndex == sharp.Points.Count)
                        nextIndex = 0;
                    Vector3 pt = worldPoints[i];
                    Handles.DrawLine(pt, worldPoints[nextIndex]);
                    //点位置编辑
                    float handleSize = HandleUtility.GetHandleSize(pt) * 0.09f;
                    EditorGUI.BeginChangeCheck();
                    pt = Handles.FreeMoveHandle(pt, Quaternion.identity, handleSize, new Vector3(1, 0, 1), Handles.SphereHandleCap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(sharp, "move polygon point");
                        EditorUtility.SetDirty(sharp);
                        pt = matrix.inverse.MultiplyPoint(pt);
                        pt.y = 0;
                        worldPoints[i] = pt;
                        sharp.Points[i] = pt;
                        sharp.SetDirty();
                    }
                }
                if (Event.current.control)
                {//移除点
                    if (worldPoints.Length > 3)
                    {
                        int removeIndex = index + 1;
                        if (removeIndex == worldPoints.Length)
                            removeIndex = 0;

                        int preIndex = removeIndex - 1;
                        if (preIndex < 0)
                            preIndex = worldPoints.Length - 1;
                        int nextIndex = removeIndex + 1;
                        if (nextIndex == worldPoints.Length)
                            nextIndex = 0;
                        Handles.DrawLine(worldPoints[preIndex], worldPoints[nextIndex]);
                        using (new Handles.DrawingScope(Color.red))
                        {
                            Handles.DrawAAPolyLine(5, worldPoints[preIndex], worldPoints[removeIndex]);
                            Handles.DrawAAPolyLine(5, worldPoints[nextIndex], worldPoints[removeIndex]);
                            float handleSize = HandleUtility.GetHandleSize(worldPoints[removeIndex]) * 0.1f;
                            var pt1 = HandleUtility.WorldToGUIPoint(worldPoints[removeIndex]);
                            var pt2 = HandleUtility.WorldToGUIPoint(mousePos);
                            float pickSize = Vector2.Distance(pt1, pt2) + 0.1f;
                            if (Handles.Button(worldPoints[removeIndex], Quaternion.identity, handleSize, pickSize, Handles.SphereHandleCap))
                            {
                                Undo.RecordObject(sharp, "delete polygon point");
                                EditorUtility.SetDirty(sharp);
                                sharp.Points.RemoveAt(removeIndex);
                                sharp.SetDirty();
                                Event.current.Use();
                            }
                        }
                    }
                }
                else if(Event.current.shift && (result.SegmentLength * result.NormalLength) > 0.1f && (result.SegmentLength * result.NormalLength) < (result.SegmentLength - 0.1f))
                {//添加点

                    //画距离鼠标最近的点
                    Handles.SphereHandleCap(0, closePoint, Quaternion.identity, 0.1f, Event.current.type);

                    int insertIndex = index + 1;
                    if (insertIndex == worldPoints.Length)
                        insertIndex = 0;
                    using (new Handles.DrawingScope(Color.white))
                    {
                        Handles.DrawLine(mousePos, worldPoints[index]);
                        Handles.DrawLine(mousePos, worldPoints[insertIndex]);
                        if (Handles.Button(mousePos, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                        {
                            Undo.RecordObject(sharp, "add polygon point");
                            EditorUtility.SetDirty(sharp);

                            var pt = matrix.inverse.MultiplyPoint(mousePos);
                            sharp.Points.Insert(insertIndex, pt);
                            sharp.SetDirty();
                            Event.current.Use();
                        }
                    }
                }
            }
            SceneView.RepaintAll();
        }
    }
}