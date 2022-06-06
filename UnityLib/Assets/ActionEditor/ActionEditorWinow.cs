using FrameLine;
using UnityEditor;
using UnityEngine;

public class ActionEditorWinow : EditorWindow
{
    [SerializeField]
    private ActionAsset asset;
    [SerializeField]
    public FrameLineView view;

    public static void Open(ActionAsset action)
    {
        var window = GetWindow<ActionEditorWinow>();
        if (window.asset != action || window.view == null)
        {
            
            window.asset = action;
            window.view = CreateInstance<FrameLineView>();
            window.view.hideFlags = HideFlags.HideAndDontSave;//防止冲去虚拟机的时候丢失
            window.view.Asset = action;
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(view);
    }

    private void OnGUI()
    {
        if (view == null)
            return;
        if (view.OnDraw(position.size))
        {
            Repaint();
        }
    }
}
