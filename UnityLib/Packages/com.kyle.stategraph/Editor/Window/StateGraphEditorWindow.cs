using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class StateGraphEditorWindow<TGraph, TView> : EditorWindow where TGraph :StateGraph where TView:StateGraphView
{
    #region 编辑器配置
    [System.Serializable]
    public class EditorConfig
    {
        public bool HidenLeft;
        public bool HideRight;
        public bool ExportWhenSave = true;
        private string key;

        public void SetHidenLeft(bool hidden)
        {
            if (HidenLeft != hidden)
            {
                HidenLeft = hidden;
                Save();
            }
        }
        public void SetHideRight(bool hidden)
        {
            if (HideRight != hidden)
            {
                HideRight = hidden;
                Save();
            }
        }
        public void SetExportWhenSave(bool export)
        {
            if (ExportWhenSave != export)
            {
                ExportWhenSave = export;
                Save();
            }
        }

        public static EditorConfig Load(string key)
        {
            EditorConfig config = new EditorConfig();
            config.key = key;
            var save = EditorPrefs.GetString(key, "{}");
            EditorJsonUtility.FromJsonOverwrite(save, config);
            return config;
        }

        public void Save()
        {
            string val = EditorJsonUtility.ToJson(this);
            EditorPrefs.SetString(key, val);
        }
    }
    private static EditorConfig _config;
    protected static EditorConfig Config
    {
        get
        {
            if (_config == null)
                _config = EditorConfig.Load(typeof(TGraph).FullName);
            return _config;
        }
    }
    #endregion

    public const float TOOL_BAR_HEIGHT = 20;
    protected float RightAreaRate = 0.2f;
    protected float LeftAreaRate = 0.15f;
    protected float LEFT_AREA_WIDTH = 200;
    protected float RIGHT_AREA_WIDTH = 300;
    public TView SelectedView;
    public BlackboardEditor Blackboard = new BlackboardEditor();
    public List<TView> OpenList = new List<TView>();
    public bool NeedRepaint { get; set; }
    [SerializeField]
    protected StateGraphGroup Group;
    private List<bool> groupFoldout = new List<bool>();

    protected virtual void Open(TGraph graph) 
    {
        if (SelectedView != null && SelectedView.Graph == graph)
        {
            return;
        }
        RegistUndo("open state graph");
        var view = OpenList.Find(it => it.Graph == graph);
        if (view == null)
        {
            view = CreateInstance<TView>();
            view.Init(graph, this);
            OpenList.Add(view);
        }
        SelectedView = view;
        Focus();
        Repaint();
    }

    protected virtual void OnEnable()
    {
        if (SelectedView == null)
        {
            LeftSelectIndx = 1;
        }
        string groupPath = GetGroupPath();
        if (!string.IsNullOrEmpty(groupPath))
        {
            Group = AssetDatabase.LoadAssetAtPath<StateGraphGroup>(groupPath);
            if (Group == null)
            {
                Group = CreateInstance<StateGraphGroup>();
                Group.Groups.Add("Default");
                AssetDatabase.CreateAsset(Group, groupPath);
            }
        }
        EditorApplication.playModeStateChanged -= WaitSave;
    }

    protected string GetSaveKey()
    {
        return typeof(TGraph).FullName;
    }

    protected void RegistUndo(string name)
    {
        Undo.RegisterCompleteObjectUndo(this, name);
    }

    protected virtual string GetGroupPath()
    {
        return null;
    }

    private void OnGUI()
    {
        OpenList.RemoveAll(it => it == null);
        if (SelectedView == null && OpenList.Count > 0)
        {
            SelectedView = OpenList[0];
        }
        Vector2 size = position.size;
        LEFT_AREA_WIDTH = size.x * LeftAreaRate;
        RIGHT_AREA_WIDTH = size.x * RightAreaRate;

        using (new GUILayout.AreaScope(new Rect(Vector2.zero, new Vector2(size.x, TOOL_BAR_HEIGHT))))
        {
            using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                DrawToolBar();
            }
        }
        if (!Config.HidenLeft)
        {
            using (new GUILayout.AreaScope(new Rect(new Vector2(0, TOOL_BAR_HEIGHT), new Vector2(LEFT_AREA_WIDTH, size.y - TOOL_BAR_HEIGHT*2))))
            {
                DrawLeftArea();
            }
        }
        float centerStartX = LEFT_AREA_WIDTH;
        float centerWidth = size.x - LEFT_AREA_WIDTH - RIGHT_AREA_WIDTH;
        if (Config.HidenLeft)
        {
            centerStartX = 0;
            centerWidth += LEFT_AREA_WIDTH;
        }
        if (Config.HideRight)
        {
            centerWidth += RIGHT_AREA_WIDTH;
        }
        if (!Config.HideRight)
        {
            Rect rightRect = new Rect(new Vector2(size.x - RIGHT_AREA_WIDTH, TOOL_BAR_HEIGHT), new Vector2(RIGHT_AREA_WIDTH, size.y - TOOL_BAR_HEIGHT*2));
            using (new GUILayout.AreaScope(rightRect))
            {
                DrawRightArea();
            }
        }
        //底部工具栏
        Rect bottomRect = new Rect(new Vector2(0, size.y - TOOL_BAR_HEIGHT), new Vector2(size.x, TOOL_BAR_HEIGHT));
        using (new GUILayout.AreaScope(bottomRect))
        {
            using (new GUILayout.HorizontalScope())
            {
                LeftAreaRate = EditorGUILayout.Slider("左侧区域", LeftAreaRate, 0.1f, 0.4f);
                if (SelectedView)
                {
                    GUILayout.Label(AssetDatabase.GetAssetPath(SelectedView.Graph));
                    if (EditorUtility.IsDirty(SelectedView.Graph))
                    {
                        GUILayout.Label("*");
                    }
                }
                GUILayout.FlexibleSpace();
                RightAreaRate = EditorGUILayout.Slider("右侧区域", RightAreaRate, 0.1f, 0.4f);
            }
        }

        Rect rect = new Rect(new Vector2(centerStartX, TOOL_BAR_HEIGHT), new Vector2(centerWidth, size.y - TOOL_BAR_HEIGHT * 2));
        using (new GUILayout.AreaScope(rect))
        {
            DrawCenterArea(rect.size);
        }

        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.S)
        {
            Save();
            e.Use();
        }
        if ((e.control || e.command) && (e.keyCode == KeyCode.Z || e.keyCode == KeyCode.Y))
        {
            NeedRepaint = true;
        }

        if (NeedRepaint)
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
        if (SelectedView && GUILayout.Button("导出", EditorStyles.toolbarButton))
        {
            DoExport(SelectedView.Graph as TGraph);
        }
        Config.SetExportWhenSave(GUILayout.Toggle(Config.ExportWhenSave, "保存时导出", EditorStyles.toolbarButton));
        Config.SetHidenLeft(GUILayout.Toggle(Config.HidenLeft, "隐藏左侧", EditorStyles.toolbarButton));
        Config.SetHideRight(GUILayout.Toggle(Config.HideRight, "隐藏右侧", EditorStyles.toolbarButton));
    }

    protected virtual void DoExport(TGraph graph)
    {
    }

    private static readonly string[] leftTabName = new string[] { "黑板", "列表" };

    public int LeftSelectIndx = 0;
    protected virtual void DrawLeftArea()
    {
        LeftSelectIndx = GUILayout.Toolbar(LeftSelectIndx, leftTabName);
        if (LeftSelectIndx == 0)
        {
            if (SelectedView)
            {
                GUI.enabled = !SelectedView.ReadOnly;
                Blackboard.Draw(SelectedView);
                GUI.enabled = true;
            }
        }
        else if (LeftSelectIndx == 1)
        {
            DrawGraphList();
        }
    }

    public Vector2 rightScrollPos;
    protected virtual void DrawRightArea()
    {
        if (SelectedView)
        {
            GUI.enabled = !SelectedView.ReadOnly;
            using (var scroll = new GUILayout.ScrollViewScope(rightScrollPos))
            {
                if (SelectedView.Selecteds.Count > 0)
                {
                    foreach (var nodeRef in SelectedView.Selecteds)
                    {
                        nodeRef.Node.Editor.OnInspectorGUI();
                        GUILayout.Space(1);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    string commit = SelectedView.Graph.Commit;
                    GUILayout.Label("描述：");
                    commit = EditorGUILayout.TextArea(commit);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SelectedView.RegistUndo("modify commit");
                        SelectedView.Graph.Commit = commit;
                    }
                }
                GUILayout.FlexibleSpace();
                rightScrollPos = scroll.scrollPosition;
            }
            GUI.enabled = true;
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
                DrawGraphInfo(SelectedView.Graph);
                if (Group)
                {
                    EditorGUI.BeginChangeCheck();
                    int newGroup = EditorGUILayout.Popup("分组", SelectedView.Graph.GroupId, Group.Names);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SelectedView.RegistUndo("modify group");
                        SelectedView.Graph.GroupId = newGroup;
                    }
                }
            }
        }
        using (var scroll = new GUILayout.ScrollViewScope(graphsScroll))
        {
            using(new GUILayout.VerticalScope())
            {
                //显示所有的同类型Graph
                var list = GetGraphs();
                GUILayout.Label("列表", EditorStyles.boldLabel);
                if (Group)
                {
                    for (int i=0; i<Group.Groups.Count; ++i)
                    {
                        while (groupFoldout.Count < Group.Groups.Count)
                        {
                            groupFoldout.Add(false);
                        }
                        bool foldout = groupFoldout[i];
                        var drawList = list.Where(it => it.GroupId == i && (string.IsNullOrWhiteSpace(graphSearch) || it.name.IndexOf(graphSearch, System.StringComparison.OrdinalIgnoreCase) >= 0));
                        if (drawList.Count() > 0)
                        {
                            groupFoldout[i] = EditorGUILayout.Foldout(foldout, Group.Groups[i], true);
                            if (!groupFoldout[i])
                                continue;
                            foreach (var graph in drawList)
                            {
                                using (new GUILayout.VerticalScope("Box"))
                                {
                                    DrawGraphInfo(graph);
                                    if (GUILayout.Button("打开"))
                                    {
                                        AssetDatabase.OpenAsset(graph);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var graph in list)
                    {
                        //匹配搜索框
                        if (!string.IsNullOrWhiteSpace(graphSearch) && graph.name.IndexOf(graphSearch, System.StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        using (new GUILayout.VerticalScope("Box"))
                        {
                            DrawGraphInfo(graph);
                            if (GUILayout.Button("打开"))
                            {
                                AssetDatabase.OpenAsset(graph);
                            }
                        }
                    }
                }
            }
            graphsScroll = scroll.scrollPosition;
        }

    }

    protected void DrawGraphInfo(StateGraph graph)
    {
        EditorGUILayout.SelectableLabel(graph.name);
        GUILayout.Label(graph.Commit, EditorStyles.wordWrappedLabel);
    }

    protected abstract IReadOnlyCollection<TGraph> GetGraphs();

    protected virtual void OnCloseView(TView view)
    {
        Undo.ClearUndo(view.Graph);
        Undo.ClearUndo(view);
        DestroyImmediate(view);
    }

    private void OnDestroy()
    {
        Save();
        foreach (var view in OpenList)
        {
            OnCloseView(view);
        }
        Undo.ClearUndo(this);
    }

    protected void WaitSave(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            AssetDatabase.SaveAssets();
        }
    }

    protected virtual void Save()
    {
        bool dirty = false;
        foreach (var view in OpenList)
        {
            if (EditorUtility.IsDirty(view.Graph))
            {
                dirty = true;
                if (Config.ExportWhenSave)
                    DoExport(view.Graph as TGraph);
            }
        }
        if (dirty)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.playModeStateChanged += WaitSave;
            }
            else
            {
                AssetDatabase.SaveAssets();
            }
        }
    }

    protected abstract void OpenCreateWizard();
}
