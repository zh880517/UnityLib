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
        using(new GUILayout.AreaScope(new Rect(0, 0, position.size.x, 20)))
        {
            using(new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("测试"))
                {
                    asset.AddClip(0, new TestAction());
                    asset.UpdateAllTrack();
                    EditorUtility.SetDirty(asset);
                }
                GUILayout.FlexibleSpace();
            }
        }
        Rect rectView = new Rect(0, 20, position.size.x, position.size.x - 20);
        using (new GUILayout.AreaScope(rectView))
        {
            if (view.OnDraw(rectView.size))
            {
                Repaint();
            }
        }
    }
}
