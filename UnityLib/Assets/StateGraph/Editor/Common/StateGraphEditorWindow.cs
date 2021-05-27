using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class StateGraphEditorWindow<TGraph, TView> : EditorWindow where TGraph :StateGraph where TView:StateGraphView
{
    public const float LEFT_AREA_WIDTH = 200;
    public const float RIGHT_AREA_WIDTH = 300;
    public const float TOOL_BAR_HEIGHT = 20;
    public bool HideLeftArea;
    public bool HideRightArea;
    public TView SelectedView;
    public BlackboardEditor Blackboard = new BlackboardEditor();
    public List<TView> OpenList = new List<TView>();
    public bool NeedRepaint { get; set; }

    protected virtual void Open(TGraph graph) 
    {
        if (SelectedView != null && SelectedView.Graph != graph)
        {
            return;
        }
        var view = OpenList.Find(it => it.Graph == graph);
        if (view == null)
        {
            view = CreateInstance<TView>();
            view.Init(graph, this);
            OpenList.Add(view);
        }
        SelectedView = view;
    }

    private void OnGUI()
    {
        OpenList.RemoveAll(it => it == null);
        if (SelectedView == null && OpenList.Count > 0)
        {
            SelectedView = OpenList[0];
        }
        Vector2 size = position.size;
        if (!EditorUtility.IsDirty(SelectedView.Graph))
        {
            titleContent.text = $"技能:{SelectedView.Graph.name}";
        }
        else
        {
            titleContent.text = $"技能:{SelectedView.Graph.name}*";
        }
        using (new GUILayout.AreaScope(new Rect(Vector2.zero, new Vector2(size.x, TOOL_BAR_HEIGHT))))
        {
            using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
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
        if (!HideRightArea)
        {
            Rect rightRect = new Rect(new Vector2(size.x - RIGHT_AREA_WIDTH, TOOL_BAR_HEIGHT), new Vector2(RIGHT_AREA_WIDTH, size.y - TOOL_BAR_HEIGHT));
            //GUI.DrawTexture(rightRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, new Color(0.17f, 0.17f, 0.17f, 1), 0, 0);
            using (new GUILayout.AreaScope(rightRect))
            {
                DrawRightArea();
            }
        }

        if (NeedRepaint || Event.current.type == EventType.KeyUp)
        {
            NeedRepaint = false;
            GUI.FocusControl("");
            Repaint();
        }
    }

    protected virtual void DrawToolBar()
    {
        if (GUILayout.Button("创建", EditorStyles.toolbarButton))
        {
            OpenCreateWizard();
        }
        GUILayout.FlexibleSpace();
        HideLeftArea = GUILayout.Toggle(HideLeftArea, "隐藏左侧", EditorStyles.toolbarButton);
        HideRightArea = GUILayout.Toggle(HideRightArea, "隐藏右侧", EditorStyles.toolbarButton);
    }

    protected virtual void DrawLeftArea()
    {
        if (SelectedView)
        {
            Blackboard.Draw(SelectedView);
        }
    }

    protected virtual void DrawRightArea()
    {
        foreach (var nodeRef in SelectedView.Selecteds)
        {
            nodeRef.Node.Editor.OnInspectorGUI();
        }
    }

    protected virtual void DrawCenterArea(Vector2 size)
    {
        //画底框
        Rect rect = new Rect(Vector2.zero, size);
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, new Color(0.17f, 0.17f, 0.17f, 1), 0, 0);
        if (SelectedView)
        {
            bool repaint = SelectedView.OnDraw(size);
            if (repaint)
            {
                NeedRepaint = repaint;
            }
            GUI.Label(rect, AssetDatabase.GetAssetPath(SelectedView.Graph), StateGraphEditorStyles.FileNameStyle.GetStyle(1));
        }
    }

    private Vector2 graphsScroll;
    private string graphSearch;
    protected virtual void DrawGraphList()
    {
        using (new GUILayout.VerticalScope("Box"))
        {
            graphSearch = GUILayout.TextField(graphSearch, EditorStyles.toolbarSearchField);
            if (SelectedView != null)
            {
                GUILayout.Label("当前显示:", EditorStyles.boldLabel);
                GUILayout.Label(SelectedView.Graph.name, EditorStyles.boldLabel);
                if (GUILayout.Button("关闭"))
                {
                    CloseSelect();
                }
            }
        }
        using (var scroll = new GUILayout.ScrollViewScope(graphsScroll))
        {
            using(new GUILayout.VerticalScope())
            {
                if (OpenList.Count > 0)
                {
                    GUILayout.Label("当前打开:", EditorStyles.boldLabel);
                }
                for (int i=0; i<OpenList.Count; ++i)
                {
                    var view = OpenList[i];
                    if (view == SelectedView)
                        continue;
                    //匹配搜索框
                    if (!string.IsNullOrWhiteSpace(graphSearch) && view.Graph.name.IndexOf(graphSearch, System.StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    using (new GUILayout.VerticalScope("Box"))
                    {
                        GUILayout.Label(view.Graph.name, EditorStyles.boldLabel);
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("关闭"))
                            {
                                CloseView(view);
                                --i;
                                continue;
                            }
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("切换显示"))
                            {
                                SelectedView = view;
                            }
                        }
                    }
                }
                //显示所有的同类型Graph
                var list = GetGraphs();
                foreach (var graph in list)
                {
                    //过滤已经打开的Graph
                    if (OpenList.Exists(it => it.Graph == graph))
                        continue;
                    //匹配搜索框
                    if (!string.IsNullOrWhiteSpace(graphSearch) && graph.name.IndexOf(graphSearch, System.StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    using (new GUILayout.VerticalScope("Box"))
                    {
                        GUILayout.Label(graph.name, EditorStyles.boldLabel);
                        if (GUILayout.Button("打开"))
                        {
                            AssetDatabase.OpenAsset(graph);
                        }
                    }
                }
            }
            graphsScroll = scroll.scrollPosition;
        }

    }

    protected abstract IReadOnlyCollection<TGraph> GetGraphs();

    protected virtual void OnCloseView(TView view)
    {
        Undo.ClearUndo(view.Graph);
        Undo.ClearUndo(view);
        DestroyImmediate(view);
    }

    protected void CloseSelect()
    {
        if (SelectedView)
        {
            int idx = OpenList.IndexOf(SelectedView);
            OpenList.RemoveAt(idx);
            OnCloseView(SelectedView);
            SelectedView = null;
            idx--;
            if (idx < 0)
            {
                idx = 0;
            }
            if (idx <= OpenList.Count - 1)
            {
                SelectedView = OpenList[idx];
            }
        }
    }

    protected void CloseView(TView view)
    {
        int idx = OpenList.IndexOf(view);
        OpenList.RemoveAt(idx);
        OnCloseView(view);
    }

    private void OnDestroy()
    {
        foreach (var view in OpenList)
        {
            OnCloseView(view);
        }
        Undo.ClearUndo(this);
        AssetDatabase.SaveAssets();
    }

    protected abstract void OpenCreateWizard();
}
