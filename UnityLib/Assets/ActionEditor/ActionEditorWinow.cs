using FrameLine;
using UnityEditor;
using UnityEngine;

public class ActionEditorWinow : EditorWindow
{
    [SerializeField]
    private ActionAsset asset;
    [SerializeField]
    public FrameLineGUI view;

    public static void Open(ActionAsset action)
    {
        var window = GetWindow<ActionEditorWinow>();
        if (window.asset != action || window.view == null)
        {
            window.asset = action;
            window.view = CreateInstance<FrameLineGUI>();
            window.view.hideFlags = HideFlags.HideAndDontSave;//防止重启虚拟机的时候丢失
            window.view.Window = window;
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
        wantsMouseMove = true;
        using (new GUILayout.AreaScope(new Rect(0, 0, position.size.x, 20)))
        {
            using(new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("测试"))
                {
                    var clip = asset.AddClip(0, 0, new TestAction());
                    view.OnAddClip(clip);
                    view.SelectedClips.Clear();
                    view.SelectedClips.Add(clip);
                    EditorUtility.SetDirty(asset);
                }
                GUILayout.FlexibleSpace();
            }
        }
        Rect rectView = new Rect(0, 20, position.size.x, position.size.y - 20);
        using (new GUILayout.AreaScope(rectView))
        {
            if (view.OnDraw(rectView.size))
            {
                Repaint();
            }
            else
            {
                var type = Event.current.type;
                if (type == EventType.KeyDown || type == EventType.KeyUp)
                {
                    Repaint();
                }
            }
        }
    }
}
