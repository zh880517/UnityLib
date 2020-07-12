using UnityEngine;

public static class AutoGraphStyles
{
    private static bool inited;
    public static GUIStyle NodeNormal { get; private set; }
    public static GUIStyle NodeNormalSelect { get; private set; }
    public static GUIStyle NodePlaceholder { get; private set; }
    public static GUIStyle NodeDrag { get; set; }
    public static GUIStyle FreeNode { get; private set; }
    public static GUIStyle FreeNodeSelect { get; private set; }

    public static GUIStyle FoldNode { get; private set; }
    public static GUIStyle FoldNodeSelecct { get; private set; }
    public static GUIStyle FoldFreeNode { get; private set; }
    public static GUIStyle FoldFreeNodeSelect { get; private set; }

    public static void Init()
    {
        if (!inited)
        {
            NodeNormal = GUI.skin.GetStyle("flow node 0");
            NodeNormalSelect = GUI.skin.GetStyle("flow node 0 on");
            NodePlaceholder = GUI.skin.GetStyle("flow node 1");
            NodeDrag = GUI.skin.GetStyle("flow node 1 on");
            FreeNode = GUI.skin.GetStyle("flow node 4");
            FreeNodeSelect = GUI.skin.GetStyle("flow node 4 on");
            FoldNode = GUI.skin.GetStyle("flow node 2");
            FoldNodeSelecct = GUI.skin.GetStyle("flow node 2 on");
            FoldFreeNode = GUI.skin.GetStyle("flow node 5");
            FoldFreeNodeSelect = GUI.skin.GetStyle("flow node 5");
            inited = true;
        }
    }

    public static void UpdateScale(float scale)
    {
        Init();
    }
}
