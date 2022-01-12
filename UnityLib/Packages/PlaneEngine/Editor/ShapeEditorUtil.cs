using UnityEditor;
using UnityEngine;

namespace PlaneEngine
{
    public static class ShapeEditorUtil
    {
        public static void DefaultInspectorGUI(Editor editor)
        {
            var shape = editor.target as Shape;
            if (editor.DrawDefaultInspector())
            {
                shape.SetDirty();
            }
            var sharpRender = shape.GetComponent<ShapeRender>();
            if (sharpRender)
            {
                if (GUILayout.Button("取消显示"))
                {
                    Undo.DestroyObjectImmediate(sharpRender);
                }
            }
            else
            {
                if (GUILayout.Button("在场景中显示"))
                {
                    Undo.AddComponent<ShapeRender>(shape.gameObject);
                }
            }
        }

        private static T CreateShape<T>(MenuCommand menuCommand) where T : Shape
        {
            GameObject go = ObjectFactory.CreateGameObject(typeof(T).Name);
            T shape = Undo.AddComponent<T>(go);
            var camera = SceneView.lastActiveSceneView.camera;
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            if (!PlaneUtils.RayCastPlaneXZ(ray, out Vector3 pos))
            {
                pos = ray.origin + ray.direction * 10;
            }
            go.transform.position = pos;
            if (menuCommand.context is GameObject parent)
            {
                Undo.SetTransformParent(go.transform, parent.transform, "CreateSharp");
            }
            Selection.activeGameObject = go;
            //EditorGUIUtility.PingObject(go);
            SceneView.lastActiveSceneView.FrameSelected();
            return shape;
        }

        [MenuItem("GameObject/PlaneShape/Box", false, 11)]
        public static void CreateBox(MenuCommand menuCommand)
        {
            CreateShape<BoxShape>(menuCommand);
        }

        [MenuItem("GameObject/PlaneShape/Circle", false, 11)]
        public static void CreateCircle(MenuCommand menuCommand)
        {
            CreateShape<CircleShape>(menuCommand);
        }

        [MenuItem("GameObject/PlaneShape/Polygon", false, 11)]
        public static void CreatePolygon(MenuCommand menuCommand)
        {
            CreateShape<PolyShape>(menuCommand);
        }
        [MenuItem("GameObject/PlaneShape/Line", false, 11)]
        public static void CreateLine(MenuCommand menuCommand)
        {
            var line = CreateShape<LineShape>(menuCommand);
            line.Type = PolyType.Obstacle;
        }

    }
}
