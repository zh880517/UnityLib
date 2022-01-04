using UnityEditor;
using UnityEngine;

namespace PlaneSharp
{
    public static class SharpEditorUtil
    {
        public static void DefaultInspectorGUI(Editor editor)
        {
            var sharp = editor.target as Sharp;
            if (editor.DrawDefaultInspector())
            {
                sharp.SetDirty();
            }
            var sharpRender = sharp.GetComponent<SharpRender>();
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
                    Undo.AddComponent<SharpRender>(sharp.gameObject);
                }
            }
        }

        private static T CreateSharp<T>(MenuCommand menuCommand) where T : Sharp
        {
            GameObject go = ObjectFactory.CreateGameObject(typeof(T).Name);
            T sharp = Undo.AddComponent<T>(go);
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
            return sharp;
        }

        [MenuItem("GameObject/PlaneSharp/Box", false, 11)]
        public static void CreateBox(MenuCommand menuCommand)
        {
            CreateSharp<BoxSharp>(menuCommand);
        }

        [MenuItem("GameObject/PlaneSharp/Circle", false, 11)]
        public static void CreateCircle(MenuCommand menuCommand)
        {
            CreateSharp<CircleSharp>(menuCommand);
        }

        [MenuItem("GameObject/PlaneSharp/Polygon", false, 11)]
        public static void CreatePolygon(MenuCommand menuCommand)
        {
            CreateSharp<PolySharp>(menuCommand);
        }
        [MenuItem("GameObject/PlaneSharp/Line", false, 11)]
        public static void CreateLine(MenuCommand menuCommand)
        {
            var line = CreateSharp<LineSharp>(menuCommand);
            line.Type = PolyType.Obstacle;
        }

    }
}
