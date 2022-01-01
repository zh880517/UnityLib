using UnityEditor;
using UnityEngine;

namespace Polygon
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
                    Object.DestroyImmediate(sharpRender);
                }
            }
            else
            {
                if (GUILayout.Button("在场景中显示"))
                {
                    sharp.SetVisableInScene();
                }
            }
        }
    }
}
