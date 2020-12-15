﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StateGraphView : ScriptableObject
{
    public const float CHILD_INTERVAL = 5;
    public const float STACK_NODE_WIDTH = 140;
    public const float PIN_WIDTH = 20;
    public const float PIN_HEIGHT = 20;
    public static readonly Vector2 BREAK_LINK_SIZE = new Vector2(10, 10);
    public static readonly Vector2 NODE_SIZE = new Vector2(100, 50);
    public static readonly Vector2 CHILD_OFFSET = new Vector2(10, 20);
    public static readonly Vector2 PIN_SIZE = new Vector2(PIN_WIDTH, PIN_HEIGHT);

    public StateGraph Graph;
    public List<StateNodeRef> Selecteds = new List<StateNodeRef>();
    public GUICanvas Canvas = new GUICanvas();
    public int SelectIndex { get; set; }
    private IViewDragMode DragMode;
    private List<Type> _vaildTypes;
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

    public void Init(StateGraph graph)
    {
        Graph = graph;
        SelectIndex = 0;
        if (graph.Nodes.Count > 0)
        {
            SelectIndex = graph.Nodes.Last().SortIndex;
        }
    }

    public bool OnDraw(Vector2 size)
    {
        Event e = Canvas.OnGUI(size);
        DrawLinkLins();
        DrawNodes();
        if (DragMode != null)
        {
            DragMode.Draw(this);
        }
        OnEvent(e);
        return e.type == EventType.Used;
    }

    public virtual void UpdateBounds(StateNode node)
    {
        if (!node.Parent)
        {
            Vector2 size = NODE_SIZE;
            if (Graph.IsStack(node))
            {
                size.x = STACK_NODE_WIDTH;
                Vector2 pos = node.Bounds.position + CHILD_OFFSET;
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
                Canvas.DrawLinkLines(from, to, Color.white, 5);
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

    protected virtual void DrawStackNode(StateNode node)
    {
        foreach (var link in Graph.Links)
        {
            if (link.IsChild && link.From == node)
            {
                DrawNormalNode(node);
            }
        }
    }

    protected virtual void DrawNormalNode(StateNode node)
    {
        
    }

    private void OnEvent(Event e)
    {
        if (e.type == EventType.MouseDown && e.button <= 1 )
        {
            OnClick(e.alt, e.button == 0);
            e.Use();
            return;
        }
        if (e.type == EventType.MouseUp)
        {
            if (DragMode != null && e.button == 0)
            {
                DragMode = null;
                e.Use();
                return;
            }
            if (e.button == 1)
            {
                OnMenu();
                e.Use();
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
                return;
            }
            if (e.button == 1)
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
            bool control = e.control || (UnityEngine.Application.platform == RuntimePlatform.OSXEditor && e.command);
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

    private void OnClick(bool isAlt, bool isLeft)
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
                if (GetOutputPinRect(hitNode).Contains(Canvas.MouseInWorld))
                {
                    BreakOutputLink(hitNode);
                    return;
                }
                if (GetInputPinRect(hitNode).Contains(Canvas.MouseInWorld))
                {
                    BreakInputLink(hitNode);
                    return;
                }
                if (GetAddChildPinRect(hitNode).Contains(Canvas.MouseInWorld))
                {
                    BreakChildLink(hitNode);
                    return;
                }
            }
            bool hasSelected = Selecteds.Contains(hitNode);
            if (hasSelected && !isLeft)
                return;

            if (!isAlt)
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
                        if (rect.Contains(Canvas.MouseInWorld))
                        {
                            DragMode = new ViewLinkMode(this, hitNode, true, true, rect.center);
                            break;
                        }
                        rect = GetOutputPinRect(hitNode);
                        if (rect.Contains(Canvas.MouseInWorld))
                        {
                            DragMode = new ViewLinkMode(this, hitNode, true, false, rect.center);
                            break;
                        }
                        rect = GetInputPinRect(hitNode); 
                        if (rect.Contains(Canvas.MouseInWorld))
                        {
                            DragMode = new ViewLinkMode(this, hitNode, true, false, rect.center);
                        }
                    } while (false);
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
        return new Rect(pos, new Vector2(PIN_WIDTH, node.Bounds.height - PIN_HEIGHT));
    }

    protected virtual void OnMenu()
    {
        if (Selecteds.Count == 0)
        {
            var dropDown = new StateNodeCreatDropdown(this, StateNodeRef.Empty, false, false);
            dropDown.Show(new Rect(Canvas.MouseInView, new Vector2(150, 20)));
            return;
        }
        var menu = new GenericMenu();
        int copyCount = Selecteds.Count(it => Graph.CheckCopy(it.Node));
        menu.AddItem(new GUIContent("复制"), copyCount > 0, CopyNodes);
        menu.AddItem(new GUIContent("粘贴"), StateNodeClipboard.Clipboard.ContainsKey(Graph.GetType()), PasteFromClipboard);
        menu.AddItem(new GUIContent("Duplicate"), copyCount > 0, ()=>Duplicate());
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("删除"), Graph.CheckDelete(Selecteds[0]), () => DeleteSelectedNode());
        menu.AddSeparator("");
        if (Selecteds.Count == 0)
        {
            var node = Selecteds[0];
            menu.AddItem(new GUIContent("替换"), VaildTypes.Count(it => Graph.CheckReplace(node.Node.NodeType, it)) > 0, () =>
            {
                var dropDown = new StateNodeReplaceDropdown(this, node);
                dropDown.Show(new Rect(Canvas.MouseInView, new Vector2(150, 20)));
            });
            if (Graph.IsStack(node.Node))
            {
                menu.AddItem(new GUIContent("清空组"), Graph.Links.Exists(it=>it.IsChild && it.From == node), ()=>BreakChildLink(node));
            }
        }
    }
}