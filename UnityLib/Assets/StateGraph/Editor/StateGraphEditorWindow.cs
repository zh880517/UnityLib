using UnityEditor;
using UnityEngine;

public class StateGraphEditorWindow : EditorWindow
{
    public const float LEFT_AREA_WIDTH = 200;
    public const float RIGHT_AREA_WIDTH = 200;
    public const float TOOL_BAR_HEIGHT = 20;
    public bool HideLeftArea;
    public bool HideRightArea;
    public StateGraphView View;
    public BlackboardEditor Blackboard;
    public bool NeedRepaint { get; set; }
    private void OnGUI()
    {
        Vector2 size = position.size;
        using (new GUILayout.AreaScope(new Rect(Vector2.zero, new Vector2(size.x, TOOL_BAR_HEIGHT))))
        {
            using(new GUILayout.HorizontalScope())
            {
                DrawToolBar();
            }
        }
        if (!HideLeftArea)
        {
            using (new GUILayout.AreaScope(new Rect(new Vector2(0, TOOL_BAR_HEIGHT), new Vector2(LEFT_AREA_WIDTH, size.y - TOOL_BAR_HEIGHT))))
            {
                DrawLeftArea();
            }
        }
        float centerStartX = LEFT_AREA_WIDTH;
        float centerWidth = size.x - LEFT_AREA_WIDTH - RIGHT_AREA_WIDTH;
        if (HideLeftArea)
        {
            centerStartX = 0;
            centerWidth += LEFT_AREA_WIDTH;
        }
        if (HideRightArea)
        {
            centerWidth += RIGHT_AREA_WIDTH;
        }
        Rect rect = new Rect(new Vector2(centerStartX, TOOL_BAR_HEIGHT), new Vector2(centerWidth, size.y - TOOL_BAR_HEIGHT));
        using (new GUILayout.AreaScope(rect))
        {
            DrawCenterArea(rect.size);
        }
        using (new GUILayout.AreaScope(new Rect(new Vector2(size.x - RIGHT_AREA_WIDTH, TOOL_BAR_HEIGHT), new Vector2(RIGHT_AREA_WIDTH, size.y - TOOL_BAR_HEIGHT))))
        {
            DrawRightArea();
        }
        if (NeedRepaint)
        {
            NeedRepaint = false;
            Repaint();
        }
    }

    protected virtual void DrawToolBar()
    {
        HideLeftArea = GUILayout.Toggle(HideLeftArea, "隐藏左侧", EditorStyles.toolbarButton);
        HideRightArea = GUILayout.Toggle(HideRightArea, "隐藏右侧", EditorStyles.toolbarButton);
    }

    protected virtual void DrawLeftArea()
    {
        if (View)
        {
            Blackboard.Draw(View);
        }
    }

    protected virtual void DrawRightArea()
    {
        
    }

    protected virtual void DrawCenterArea(Vector2 size)
    {
        if (View)
        {
            bool repaint = View.OnDraw(size);
            if (repaint)
            {
                NeedRepaint = repaint;
            }
        }
    }
}
