using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class GraphAutoLayout : GraphLayout
{
    public static readonly Vector2 NODE_SIZE = new Vector2(100, 50);
    protected struct NodeArea
    {
        public Rect ChildrenArea;
        public float Space;
    }
    [System.Serializable]
    public class DragNode
    {
        public List<GraphNodeRef> Nodes = new List<GraphNodeRef>();
        public GraphNodeRef Parent;
        public int Index;

        public void Reset()
        {
            Nodes.Clear();
            Parent = GraphNodeRef.Empty;
            Index = -1;
        }
    }
    protected Dictionary<string, NodeArea> NodeAreas = new Dictionary<string, NodeArea>();
    public DragNode Draging;
    protected virtual bool AllowFreeNode => true;
    public override void RefreshLayout()
    {
        NodeAreas.Clear();
        foreach (var node in Graph.Nodes)
        {
            if (!node.Parent)
            {
                UpdateNodeSpace(node);
            }
        }
        UpdaeNodePos();
    }

    public override void Draw(GUICamera camera)
    {
        foreach (var node in Graph.Nodes)
        {
            if (!node.Parent && Draging.Nodes.Contains(node))
            {
                DrawNode(camera, node);
            }
        }
        if (Draging.Nodes.Count > 0)
        {
            DrawDragingNode(camera);
        }
    }

    public override bool OnStartDrag(Vector2 mouseWorldPos)
    {
        Draging.Reset();
        foreach (var node in SelectNodes)
        {
            if (!CheckNodeParentInList(node, SelectNodes))
            {
                Draging.Nodes.Add(node);
            }
        }
        return Draging.Nodes.Count > 0;
    }

    protected bool CheckNodeParentInList(GraphNodeRef node, List<GraphNodeRef> nodes)
    {
        var parent = node.Node.Parent;
        if (!parent)
            return false;
        if (nodes.Contains(parent))
            return true;

        return CheckNodeParentInList(parent, nodes);
    }

    public override void OnDraging(Vector2 mouseWorldPos, Vector2 delta)
    {
        if (Draging.Nodes.Count == 0)
            return;
        var lastParent = Draging.Parent;
        bool matched = false;
        foreach (var node in Graph.Nodes)
        {
            if (!node.Parent)
            {
                if (MatchNodeDrag(node, mouseWorldPos))
                {
                    matched = true;
                    break;
                }
            }
        }
        if (!matched)
        {
            Draging.Parent = GraphNodeRef.Empty;
            Draging.Index = -1;
        }
        if (lastParent != Draging.Parent)
            RefreshLayout();
    }

    public override void OnEndDrag(Vector2 mouseWorldPos)
    {
        if (Draging.Nodes.Count == 0)
            return;
        var nodes = Draging.Nodes.ToArray(); ;
        var parent = Draging.Parent;
        int index = Draging.Index;
        Draging.Reset();
        RefreshLayout();
        if (!parent)
        {

            Undo.RegisterCompleteObjectUndo(this, "insert node");
            foreach (var node in nodes)
            {
                Graph.InsertNodeTo(node.Node, parent.Node, index);
            }
            RefreshLayout();
        }
        else if(AllowFreeNode)
        {
            Undo.RegisterCompleteObjectUndo(this, "move node");
            foreach (var node in nodes)
            {
                Graph.FreeNode(node.Node, mouseWorldPos);
            }
            RefreshLayout();
        }
    }
    
    protected bool MatchNodeDrag(GraphNode node, Vector2 mouseWordDrag)
    {
        if (node.Children.Count + Draging.Nodes.Count > node.MaxChildrenCount)
            return false;
        if (!NodeAreas.TryGetValue(node.GUID, out var areaInfo))
            return false;
        if (!DragAreaCheck(areaInfo.ChildrenArea, mouseWordDrag))
            return false;
        if (areaInfo.ChildrenArea.Contains(mouseWordDrag))
        {
            foreach (var rf in Draging.Nodes)
            {
                if (!Graph.CheckInstert(rf.Node.NodeData.GetType(), node.NodeData.GetType()))
                    return false;
            }
            int index = GetInsertIndex(node, mouseWordDrag);
            if (index == -1)
                return false;
            Draging.Parent = node;
            Draging.Index = index;
            return true;
        }
        else
        {
            foreach (var child in node.Children)
            {
                if (MatchNodeDrag(child.Node, mouseWordDrag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected abstract void UpdateNodeSpace(GraphNode node);
    protected abstract void DrawLine(Rect from, Rect to, Color lineColor, float width);
    protected abstract void UpdaeNodePos();
    protected abstract bool DragAreaCheck(Rect area, Vector2 mousInWorld);
    protected abstract int GetInsertIndex(GraphNode node, Vector2 mousInWorld);
    public abstract Rect GetChildPlaceholderRect(GraphNode parent, int index);

    protected virtual void DrawNode(GUICamera camera, GraphNode node)
    {
        if (Draging.Nodes.Contains(node))
            return;
        Rect selfInView = camera.WorldToScreen(node.Bounds);
        if (node.Parent)
        {
            Rect parentInView = camera.WorldToScreen(node.Parent.Node.Bounds);
            DrawLine(parentInView, selfInView, Color.white, camera.Scale * LineWidth);
        }
        if (camera.ViewBounds.Overlaps(selfInView))
        {
            GUI.Box(selfInView, "", GetNodeStyle(node));
        }
        if (!node.FoldChildren)
        {
            if (Draging.Parent.GUID == node.GUID && Draging.Index >= 0)
            {
                if (Draging.Nodes.Exists(obj=>obj.Node.Parent == Draging.Parent))
                {
                    Rect bounds = GetChildPlaceholderRect(node, Draging.Index);
                    Rect boundsInView = camera.WorldToScreen(bounds);
                    DrawLine(selfInView, boundsInView, Color.yellow, camera.Scale * LineWidth);
                    GUI.Box(boundsInView, "", AutoGraphStyles.NodePlaceholder);
                }
            }
            foreach (var child in node.Children)
            {
                DrawNode(camera, child.Node);
            }
        }
    }

    protected virtual void DrawDragingNode(GUICamera camera)
    {
        Rect rect = new Rect
        {
            size = NODE_SIZE * camera.Scale,
            center = camera.MouseInView
        };
        GUI.Box(rect, Draging.Index.ToString(), Draging.Parent ? AutoGraphStyles.NodeDrag : AutoGraphStyles.FreeNode);
    }

    protected virtual GUIStyle GetNodeStyle(GraphNode node)
    {
        bool selected = IsSelecetd(node);
        if (node.IsFreeNode && node.FoldChildren && selected)
            return AutoGraphStyles.FoldFreeNodeSelect;
        if (node.IsFreeNode && node.FoldChildren)
            return AutoGraphStyles.FoldFreeNode;
        if (node.IsFreeNode)
            return AutoGraphStyles.FreeNode;
        if (node.FoldChildren && selected)
            return AutoGraphStyles.FoldNodeSelecct;
        if (node.FoldChildren)
            return AutoGraphStyles.FoldNode;
        if (selected)
            return AutoGraphStyles.NodeNormalSelect;
        return AutoGraphStyles.NodeNormal;
    }
}
