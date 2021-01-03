using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StateGraphView : ScriptableObject
{
    public const float CHILD_INTERVAL = 3;//容器节点的子节点空隙
    public const float NODE_WIDTH = 120;//普通节点的宽度
    public const float NODE_HEIGHT = 50;//普通节点的宽度
    public const float STACK_TOP_HEIGHT = NODE_HEIGHT*0.5f;//容器节点顶部预留高度
    public const float STACK_BOTTOM_HEIGHT = STACK_TOP_HEIGHT;//容器节点底部预留高度
    public const float STACK_LEFT_WIDTH = 20;//容器节点左侧预留宽度
    public const float STACK_NODE_WIDTH = STACK_LEFT_WIDTH + CHILD_INTERVAL + NODE_WIDTH;//容器节点宽度
    public const float PIN_WIDTH = 20;//连接点宽度
    public static readonly Vector2 NODE_SIZE = new Vector2(NODE_WIDTH, NODE_HEIGHT);//普通节点的大小
    public static readonly Vector2 PIN_SIZE = new Vector2(PIN_WIDTH, PIN_WIDTH);//连接点的大小
    public static readonly Color NormalNodeColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    public static readonly Color StackNodeColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public static readonly Color StackBoardColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    public static readonly Color CircleWireColor = new Color(0, 1, 1, 1);
    public static readonly Color CircleSoldColor = new Color(0, 1, 0.74f, 1);
    public static readonly GUIRenderFontStyle DefultFontStyle = new GUIRenderFontStyle(12, null, Color.white, false, TextAnchor.MiddleLeft);

    public StateGraph Graph;
    public List<StateNodeRef> Selecteds = new List<StateNodeRef>();
    public GUICanvas Canvas = new GUICanvas();
    public int SelectIndex { get; set; }
    private IViewDragMode DragMode;
    private List<Type> _vaildTypes;
    [SerializeField]
    private EditorWindow editorWindow;//用来做Undo操作
    private bool ignoreRightUp = false;
    public List<Type> VaildTypes
    {
        get
        {
            if (_vaildTypes == null)
            {
                _vaildTypes = new List<Type>();
                foreach (var kv in TypeSerializerHelper.TypeGUIDs)
                {
                    if (Graph.CheckTypeValid(kv.Value))
                    {
                        _vaildTypes.Add(kv.Value);
                    }
                }
            }
            return _vaildTypes;
        }
    }

    public void Init(StateGraph graph, EditorWindow window)
    {
        editorWindow = window;
        Graph = graph;
        SelectIndex = 0;
        if (graph.Nodes.Count > 1)
        {
            SelectIndex = graph.Nodes.Last().SortIndex;
        }
        foreach (var node in Graph.Nodes)
        {
            UpdateBounds(node);
        }
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDestroy()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        Graph.Deserialize();
        if (editorWindow)
        {
            editorWindow.Repaint();
        }
    }

    public bool OnDraw(Vector2 size)
    {
        Event e = Canvas.OnGUI(size);
        EventType eType = e.type;
        DrawLinkLins();
        DrawNodes();
        if (DragMode != null)
        {
            DragMode.Draw(this);
        }
        OnEvent(e);
        return e.type == EventType.Used 
            && eType != EventType.Layout 
            && eType != EventType.Repaint 
            && eType != EventType.Used;
    }

    public virtual void UpdateBounds(StateNode node)
    {
        if (!node.Parent)
        {
            Vector2 size = NODE_SIZE;
            if (Graph.IsStack(node))
            {
                size.x = STACK_NODE_WIDTH;
                size.y = STACK_TOP_HEIGHT + CHILD_INTERVAL + STACK_BOTTOM_HEIGHT;
                Vector2 pos = node.Bounds.position + new Vector2(STACK_LEFT_WIDTH, STACK_TOP_HEIGHT + CHILD_INTERVAL);
                foreach (var link in Graph.Links)
                {
                    if (link.IsChild && link.From.Node == node)
                    {
                        link.To.Node.Bounds = new Rect(pos, NODE_SIZE);
                        pos.y += (NODE_SIZE.y + CHILD_INTERVAL);
                        size.y += (NODE_SIZE.y + CHILD_INTERVAL);
                    }
                }
            }
            node.Bounds.size = size;
        }
    }

    public StateNode HitTest(Vector2 ptInWorld)
    {
        for (int i=Graph.Nodes.Count-1; i>=0; --i)
        {
            var node = Graph.Nodes[i];
            if (node.Bounds.Contains(ptInWorld))
            {
                return node;
            }
        }
        return null;
    }

    private void DrawLinkLins()
    {
        foreach (var link in Graph.Links)
        {
            if (!link.IsChild)
            {
                Vector2 from = GetOutputPinRect(link.From.Node).center;
                Vector2 to = GetInputPinRect(link.To.Node).center;
                Canvas.DrawLinkLines(from, to, Color.white, 3);
            }
        }
    }

    private void DrawNodes()
    {
        foreach (var node in Graph.Nodes)
        {
            if (!node.Parent)
            {
                if (Graph.IsStack(node))
                {
                    DrawStackNode(node);
                }
                else
                {
                    DrawNormalNode(node);
                }
            }
        }
    }

    private static readonly List<StateNodeLink> childLinkTmp = new List<StateNodeLink>();

    protected virtual void DrawStackNode(StateNode node)
    {
        if (!Canvas.DrawRect(node.Bounds, StackNodeColor, true, true, Selecteds.Contains(node)))
            return;
        Rect topBound = new Rect(node.Bounds.position, new Vector2(STACK_NODE_WIDTH, STACK_TOP_HEIGHT));
        //画顶部区域，包含文字、输入、添加子节点
        if (Canvas.DrawRect(topBound, StackBoardColor, true, false))
        {
            topBound.position += new Vector2(PIN_WIDTH, 0);
            topBound.width -= PIN_WIDTH * 2;
            Canvas.DrawText(topBound, node.Name, node.Comments, DefultFontStyle);
            if (Graph.ChechInput(node))
            {
                Vector2 pos = GetInputPinRect(node).center;
                Canvas.DrawCircle(pos, CircleWireColor, 6, true);
                if (Graph.Links.Exists(it => it.To == node))
                {
                    Canvas.DrawCircle(pos, CircleSoldColor, 4, false);
                }

                Rect addRect = GetAddChildPinRect(node);
                Canvas.DrawText(addRect, "+", null, DefultFontStyle);
            }
        }
        childLinkTmp.Clear();
        {//左侧区域
            Rect centerLeftRect = node.Bounds;
            centerLeftRect.position += new Vector2(0, STACK_TOP_HEIGHT);
            centerLeftRect.height -= (STACK_BOTTOM_HEIGHT + STACK_TOP_HEIGHT + 1);
            centerLeftRect.width = STACK_LEFT_WIDTH;
            Canvas.DrawRect(centerLeftRect, StackBoardColor, false, false);
        }
        foreach (var link in Graph.Links)
        {
            if (link.IsChild && link.From == node)
            {
                childLinkTmp.Add(link);
            }
        }
        for (int i=0; i<childLinkTmp.Count; ++i)
        {
            var link = childLinkTmp[i];
            DrawNormalNode(link.To.Node);
            if (childLinkTmp.Count > 1)
            {
                Rect btnSize = link.To.Node.Bounds;
                btnSize.position -= new Vector2(STACK_LEFT_WIDTH - 1, -3);
                btnSize.width = STACK_LEFT_WIDTH;
                btnSize.height = NODE_HEIGHT * 0.5f;
                if (i > 0)
                {
                    //上移按钮
                    if (GUI.Button(btnSize, "▲", StateGraphEditorStyles.TxtButtonStyle.GetStyle(Canvas.Scale)))
                    {
                        ChildNodeMoveUp(link.To);
                    }
                }
                if (i < childLinkTmp.Count - 1)
                {
                    btnSize.position += new Vector2(0, NODE_HEIGHT * 0.5f);
                    //下移按钮
                    if (GUI.Button(btnSize, "▼", StateGraphEditorStyles.TxtButtonStyle.GetStyle(Canvas.Scale)))
                    {
                        ChildNodeMoveDown(link.To);
                    }
                }
            }
        }
        Rect bottomRect = new Rect(node.Bounds.xMin, node.Bounds.yMax - STACK_BOTTOM_HEIGHT, STACK_NODE_WIDTH, STACK_BOTTOM_HEIGHT);
        if (Canvas.DrawRect(bottomRect, StackBoardColor, false, true))
        {
            if (Graph.CheckOutput(node))
            {
                Vector2 pos = GetOutputPinRect(node).center;
                Canvas.DrawCircle(pos, CircleWireColor, 6, true);
                if (Graph.Links.Exists(it => !it.IsChild && it.From == node))
                {
                    Canvas.DrawCircle(pos, CircleSoldColor, 4, false);
                }
            }
        }
    }

    protected virtual void DrawNormalNode(StateNode node)
    {
        bool isChildNode = node.Parent;
        if (Canvas.DrawRect(node.Bounds, NormalNodeColor, true, true , Selecteds.Contains(node)))
        {
            Rect txtBound = node.Bounds;
            txtBound.width -= PIN_WIDTH * 2;
            txtBound.center = node.Bounds.center;
            Canvas.DrawText(txtBound, node.Name, node.Comments, DefultFontStyle);
            if (Graph.ChechInput(node))
            {
                Vector2 pos = GetInputPinRect(node).center;
                Canvas.DrawCircle(pos, CircleWireColor, 6, true);
                if (isChildNode || Graph.Links.Exists(it => it.To == node))
                {
                    Canvas.DrawCircle(pos, CircleSoldColor, 4, false);
                }
            }
            if (Graph.CheckOutput(node))
            {
                Vector2 pos = GetOutputPinRect(node).center;
                Canvas.DrawCircle(pos, CircleWireColor, 6, true);
                if (isChildNode || Graph.Links.Exists(it => it.From == node))
                {
                    Canvas.DrawCircle(pos, CircleSoldColor, 4, false);
                }
            }
        }
    }

    private void OnEvent(Event e)
    {
        if (e.type == EventType.MouseDown && e.button <= 1 )
        {
            OnClick(e.control, e.button == 0);
            e.Use();
            return;
        }
        if (e.type == EventType.MouseUp)
        {
            if (DragMode != null && e.button == 0)
            {
                DragMode.OnDragEnd(this, Canvas.MouseInWorld);
                DragMode = null;
                e.Use();
                return;
            }
            if (e.button == 1)
            {
                e.Use();
                if (!ignoreRightUp)
                {
                    OnMenu();
                }
                ignoreRightUp = false;
                return;
            }
        }
        if (e.type == EventType.MouseDrag)
        {
            if (DragMode != null && e.button == 0)
            {
                DragMode.OnDrag(this, Canvas.MouseInWorld);
                e.Use();
                return;
            }
            if (e.button == 2 ||(e.button == 1 && Selecteds.Count == 0))
            {
                //移动View
                Canvas.Pan(-e.delta);
                e.Use();
                ignoreRightUp = true;
                return;
            }
            if (e.button == 0)
            {
                if (Selecteds.Count > 0)
                {
                    DragMode = new ViewNormalMoveMode(this, Canvas.PointInWorld);
                }
                else
                {
                    DragMode = new ViewAreaSelectMode(this, Canvas.PointInWorld);
                }
            }
        }
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Delete)
            {
                if (Selecteds.Count > 0)
                {
                    if (DeleteSelectedNode())
                        e.Use();
                }
                return;
            }
            bool control = e.control || (Application.platform == RuntimePlatform.OSXEditor && e.command);
            if (control && e.keyCode == KeyCode.D)
            {
                if (Duplicate())
                {
                    e.Use();
                    return;
                }
            }
            if (control && e.keyCode == KeyCode.C)
            {
                CopyNodes();
                e.Use();
                return;
            }
            if (control && e.keyCode == KeyCode.V)
            {
                PasteFromClipboard();
                e.Use();
                return;
            }
            if (control && e.keyCode == KeyCode.A)
            {
                Selecteds.Clear();
                foreach (var node in Graph.Nodes)
                {
                    Selecteds.Add(node);
                }
                e.Use();
                return;
            }
        }
    }

    private void OnClick(bool isCtrl, bool isLeft)
    {
        var hitNode = HitTest(Canvas.MouseInWorld);
        if (hitNode == null)
        {
            Selecteds.Clear();
        }
        else
        {
            if (!isLeft)
            {
                if (Graph.CheckOutput(hitNode) && GetOutputPinRect(hitNode).Contains(Canvas.MouseInWorld))
                {
                    BreakOutputLink(hitNode);
                    ignoreRightUp = true;
                    return;
                }
                if (Graph.ChechInput(hitNode) && GetInputPinRect(hitNode).Contains(Canvas.MouseInWorld))
                {
                    BreakInputLink(hitNode);
                    ignoreRightUp = true;
                    return;
                }
                if (Graph.IsStack(hitNode) && GetAddChildPinRect(hitNode).Contains(Canvas.MouseInWorld))
                {
                    BreakChildLink(hitNode);
                    ignoreRightUp = true;
                    return;
                }
            }
            bool hasSelected = Selecteds.Contains(hitNode);
            if (hasSelected && !isLeft)
                return;

            if (!isCtrl)
            {
                if (hasSelected && Selecteds.Count > 0)
                {
                    Selecteds.Clear();
                    Selecteds.Add(hitNode);
                    MoveNodeToBack(hitNode);
                    do
                    {
                        if (!isLeft)
                            break;
                        Rect rect = GetAddChildPinRect(hitNode);
                        if ( Graph.IsStack(hitNode) && rect.Contains(Canvas.MouseInWorld))
                        {
                            DragMode = new ViewLinkMode(hitNode, true, true, rect.center);
                            break;
                        }
                        rect = GetOutputPinRect(hitNode);
                        if (Graph.CheckOutput(hitNode) && rect.Contains(Canvas.MouseInWorld))
                        {
                            DragMode = new ViewLinkMode(hitNode, true, false, rect.center);
                            break;
                        }
                        rect = GetInputPinRect(hitNode); 
                        if (Graph.ChechInput(hitNode) && rect.Contains(Canvas.MouseInWorld))
                        {
                            DragMode = new ViewLinkMode(hitNode, true, false, rect.center);
                        }
                    } while (false);
                }
                else
                {
                    if (!hasSelected)
                    {
                        Selecteds.Clear();
                        Selecteds.Add(hitNode);
                        MoveNodeToBack(hitNode);
                    }
                }
            }
            else
            {
                if (hasSelected)
                {
                    Selecteds.Remove(hitNode);
                }
                else
                {
                    Selecteds.Add(hitNode);
                    MoveNodeToBack(hitNode);
                }

            }
        }
    }

    private void MoveNodeToBack(StateNode node)
    {
        if (node.Parent)
        {
            node.Parent.Node.SortIndex = ++SelectIndex;
            foreach (var link in Graph.Links)
            {
                if (link.IsChild && link.From == node.Parent)
                {
                    link.To.Node.SortIndex = ++SelectIndex;
                }
            }
        }
        else
        {
            node.SortIndex = ++SelectIndex;
            foreach (var link in Graph.Links)
            {
                if (link.IsChild && link.From == node)
                {
                    link.To.Node.SortIndex = ++SelectIndex;
                }
            }
        }
        SortNodes();
    }

    public void SortNodes()
    {
        Graph.Nodes.Sort((a, b) => a.SortIndex - b.SortIndex);
        SelectIndex = Graph.Nodes.Count;
        for (int i=0; i<Graph.Nodes.Count; ++i)
        {
            Graph.Nodes[i].SortIndex = i;
        }
    }

    public void CreateLink(StateNode from, StateNode to, bool isChild, bool registUndo = true)
    {
        if (Graph.CheckLink(from, to, isChild))
        {
            RegistUndo("link");
            var oldLink = Graph.Links.Find(obj => obj.From == from && obj.To == to);
            Graph.AddLink(from, to, isChild);
            if (isChild)
            {
                UpdateBounds(from);
            }
            if (oldLink != null && oldLink.IsChild && oldLink.From != from)
            {
                UpdateBounds(oldLink.From.Node);
            }
            MoveNodeToBack(from);
        }
    }

    public void BreakInputLink(StateNodeRef node)
    {
        var link = Graph.Links.Find(it => it.To == node);
        if (link != null)
        {
            RegistUndo("break input link");
            node.Node.Parent = StateNodeRef.Empty;
            Graph.Links.Remove(link);
            UpdateBounds(link.From.Node);
            if (link.IsChild)
            {
                node.Node.Bounds.position += new Vector2(link.From.Node.Bounds.width, 0);
            }
        }
    }

    public void BreakOutputLink(StateNodeRef node)
    {
        var link = Graph.Links.Find(it => !it.IsChild && it.From == node);
        if (link != null)
        {
            RegistUndo("break input link");
            Graph.Links.Remove(link);
            UpdateBounds(node.Node);
        }
    }

    public void BreakChildLink(StateNodeRef node)
    {
        if (Graph.Links.Exists(it=>it.IsChild && it.From == node))
        {
            RegistUndo("break input link");
            for (int i=Graph.Links.Count; i>=0; --i)
            {
                var link = Graph.Links[i];
                if (link.IsChild && link.From == node)
                {
                    Graph.Links.RemoveAt(i);
                    link.To.Node.Bounds.position += new Vector2(node.Node.Bounds.width, 0);
                }
            }
            UpdateBounds(node.Node);
        }
    }

    public bool DeleteSelectedNode()
    {
        var deletes = Selecteds.Where(it => Graph.CheckDelete(it)).ToArray();
        Selecteds.RemoveAll(it => Graph.CheckDelete(it));
        if (deletes.Length > 0)
        {
            RegistUndo("delete select node");
            foreach (var node in deletes)
            {
                Graph.DeleteNode(node);
            }
            return true;
        }
        return false;
    }

    public bool Duplicate()
    {
        StateNodeClipboard clipboard = new StateNodeClipboard();
        if (clipboard.CopyFrom(Graph, Selecteds, Vector2.zero))
        {
            RegistUndo("Duplicate");
            var newNodes = clipboard.PasteTo(Graph, new Vector2(50, 50));
            Selecteds.Clear();
            foreach (var node in newNodes)
            {
                node.SortIndex = ++SelectIndex;
                Selecteds.Add(node);
                UpdateBounds(node);
            }
            SortNodes();
            return true;
        }
        return false;
    }

    public void CopyNodes()
    {
        StateNodeClipboard clipboard = new StateNodeClipboard();
        clipboard.CopyFrom(Graph, Selecteds, -Canvas.Position);
        StateNodeClipboard.Clipboard.Remove(Graph.GetType());
        StateNodeClipboard.Clipboard.Add(Graph.GetType(), clipboard);
    }

    public void ChildNodeMoveUp(StateNodeRef childNode)
    {
        int oldIndex = Graph.Links.FindIndex(it => it.IsChild && it.To == childNode);
        if (oldIndex > 0)
        {
            int swapIndex = -1;
            for (int i = oldIndex - 1; i >= 0; --i)
            {
                var link = Graph.Links[i];
                if (link.IsChild && link.From == childNode.Node.Parent)
                {
                    swapIndex = i;
                    break;
                }
            }
            if (swapIndex >= 0)
            {
                RegistUndo("move child node up");
                var link = Graph.Links[swapIndex];
                Graph.Links[swapIndex] = Graph.Links[oldIndex];
                Graph.Links[oldIndex] = link;
                //重新排序， 刷新位置
                int oldSortIndex = link.To.Node.SortIndex;
                link.To.Node.SortIndex = Graph.Links[swapIndex].To.Node.SortIndex;
                Graph.Links[swapIndex].To.Node.SortIndex = oldSortIndex;
                SortNodes();
                UpdateBounds(link.From.Node);
            }
        }
    }

    public void ChildNodeMoveDown(StateNodeRef childNode)
    {
        int oldIndex = Graph.Links.FindIndex(it => it.IsChild && it.To == childNode);
        if (oldIndex >= 0)
        {
            int swapIndex = -1;
            for (int i = oldIndex + 1; i < Graph.Links.Count; ++i)
            {
                var link = Graph.Links[i];
                if (link.IsChild && link.From == childNode.Node.Parent)
                {
                    swapIndex = i;
                    break;
                }
            }
            if (swapIndex >= 0)
            {
                RegistUndo("move child node down");
                var link = Graph.Links[swapIndex];
                Graph.Links[swapIndex] = Graph.Links[oldIndex];
                Graph.Links[oldIndex] = link;

                //重新排序， 刷新位置
                int oldSortIndex = link.To.Node.SortIndex;
                link.To.Node.SortIndex = Graph.Links[swapIndex].To.Node.SortIndex;
                Graph.Links[swapIndex].To.Node.SortIndex = oldSortIndex;
                SortNodes();
                UpdateBounds(link.From.Node);
            }
        }
    }

    public void PasteFromClipboard()
    {
        if (StateNodeClipboard.Clipboard.TryGetValue(Graph.GetType(), out var clipboard))
        {
            if (clipboard.Datas.Count > 0)
            {
                RegistUndo("paste node");
                var newNodes = clipboard.PasteTo(Graph, Canvas.Position + new Vector2(50, 50));
                Selecteds.Clear();
                foreach (var node in newNodes)
                {
                    node.SortIndex = ++SelectIndex;
                    Selecteds.Add(node);
                }
                SortNodes();
            }
        }
    }

    public void RepleaceNode(StateNode node, Type type)
    {
        if (Graph.CheckReplace(node.NodeData.GetType(), type))
        {
            RegistUndo("replace node");
            string json = EditorJsonUtility.ToJson(node.NodeData);
            node.NodeData = Activator.CreateInstance(type) as IStateNode;
            EditorJsonUtility.FromJsonOverwrite(json, node.NodeData);
        }
    }

    public void RegistUndo(string name)
    {
        Undo.RegisterCompleteObjectUndo(Graph, name);
        Undo.RegisterCompleteObjectUndo(this, name);
        if (editorWindow)
            Undo.RegisterCompleteObjectUndo(editorWindow, name);
        EditorUtility.SetDirty(Graph);
    }

    public Rect GetOutputPinRect(StateNode node)
    {
        return new Rect(node.Bounds.max - PIN_SIZE, PIN_SIZE);
    }

    public Rect GetInputPinRect(StateNode node)
    {
        return new Rect(node.Bounds.position, PIN_SIZE);
    }

    public Rect GetAddChildPinRect(StateNode node)
    {
        Vector2 pos = new Vector2(node.Bounds.xMax - PIN_WIDTH, node.Bounds.yMin);
        return new Rect(pos, new Vector2(PIN_WIDTH, PIN_WIDTH));
    }

    protected virtual void OnMenu()
    {
        if (Selecteds.Count == 0)
        {
            var dropDown = new StateNodeCreatDropdown(this, StateNodeRef.Empty, false, false);
            dropDown.Show(new Rect(Canvas.MouseInView, new Vector2(250, 20)));
            return;
        }
        var menu = new GenericMenu();
        int copyCount = Selecteds.Count(it => Graph.CheckCopy(it.Node));

        AddMenuItem(menu, "复制", copyCount > 0, CopyNodes);
        AddMenuItem(menu, "粘贴", StateNodeClipboard.Clipboard.ContainsKey(Graph.GetType()), PasteFromClipboard);
        AddMenuItem(menu, "Duplicate", copyCount > 0, ()=>Duplicate());
        menu.AddSeparator("");
        AddMenuItem(menu, "删除", Graph.CheckDelete(Selecteds[0]), () => DeleteSelectedNode());
        if (Selecteds.Count == 0)
        {
            menu.AddSeparator("");
            var node = Selecteds[0];
            AddMenuItem(menu, "替换", VaildTypes.Count(it => Graph.CheckReplace(node.Node.NodeType, it)) > 0, () =>
            {
                var dropDown = new StateNodeReplaceDropdown(this, node);
                dropDown.Show(new Rect(Canvas.MouseInView, new Vector2(250, 0)));
            });
            if (Graph.IsStack(node.Node))
            {
                AddMenuItem(menu, "清空组", Graph.Links.Exists(it=>it.IsChild && it.From == node), ()=>BreakChildLink(node));
            }
        }
        menu.ShowAsContext();
    }

    protected void AddMenuItem(GenericMenu menu, string name, bool active, GenericMenu.MenuFunction func)
    {
        if (active)
        {
            menu.AddItem(new GUIContent(name), false, func);
        }
        else
        {
            menu.AddDisabledItem(new GUIContent(name));
        }
    }
}
