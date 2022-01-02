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
    }
}
