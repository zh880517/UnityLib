using UnityEditor;
using UnityEngine;
namespace MeshPainter
{
    public class MeshPaintMenu
    {
        const string MenuItemName = "GameObject/模型刷";
        [MenuItem(MenuItemName, true, 1)]
        static bool CheckEnable()
        {
            if (Selection.objects.Length == 1 && Selection.activeObject is GameObject go)
            {
                return go.GetComponent<MeshRenderer>() != null;
            }
            return false;
        }
        [MenuItem(MenuItemName, false, 1)]
        static void EditorMeshTexture()
        {
            EditorWindow.GetWindow<MeshPaintEditor>();
        }

        [MenuItem("Window/模型刷")]
        static void OpenEditor()
        {
            EditorWindow.GetWindow<MeshPaintEditor>();
        }

        [MenuItem("Window/模型刷配置")]
        static void OpenConfigEditor()
        {
            EditorWindow.GetWindow<MeshPainterConfigEditor>();
        }
    }
}