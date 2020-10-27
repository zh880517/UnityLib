using System.Collections.Generic;
using System.Linq;
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
    protected Dictionary<string, Rect> NodeAreas = new Dictionary<string, Rect>();
    protected DragNode Draging;
    protected virtual bool AllowFreeNode => true;
    public override void RefreshLayout()
    {
        NodeAreas.Clear();
        foreach (var node in Graph.Nodes)
        {
            if (!Graph.Links.Exists(obj=>obj.To.Node == node))
            {
                UpdateNodeSpace(node);
            }
        }
        UpdateNodePos();
    }

    public override void Draw(GUICamera camera)
    {
        foreach (var node in Graph.Nodes)
        {
            if (!Graph.HasParent(node) && Draging.Nodes.Contains(node))
            {
                DrawNode(camera, node);
            }
        }
        if (Draging.Nodes.Count > 0)
        {
            DrawDragingNode(camera);
        }
    }

    public override void OnMouseDown(Event e, Vector2 mouseWorldPos)
    {
        if (e.button == 2)
            return;

        if (!e.control)
        {
            selectNodes.Clear();
        }
        for (int i=Graph.Nodes.Count - 1; i>=0; --i)
        {
            var node = Graph.Nodes[i];
            if (node.Bounds.Contains(mouseWorldPos))
            {
                GraphNodeRef nodeRef = node;
                if (!e.control)
                {
                    selectNodes.Add(nodeRef);
                    break;
                }
                if (selectNodes.Contains(nodeRef))
                {
                    selectNodes.Remove(nodeRef);
                }
                else
                {
                    selectNodes.Add(nodeRef);
                }
            }
        }
    }

    public override bool OnStartDrag(Vector2 mouseWorldPos)
    {
        Draging.Reset();
        foreach (var node in selectNodes)
        {
            if (!CheckNodeParentInList(node, selectNodes))
            {
                Draging.Nodes.Add(node);
            }
        }
        return Draging.Nodes.Count > 0;
    }

    public override void OnDraging(Vector2 mouseWorldPos, Vector2 delta)
    {
        if (Draging.Nodes.Count == 0)
            return;
        var lastParent = Draging.Parent;
        bool matched = false;
        foreach (var node in Graph.Nodes)
        {
            if (!Graph.HasParent(node))
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
                Graph.InsertNodeTo(node.Node, parent.Node, index++);
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

    protected bool CheckNodeParentInList(GraphNodeRef node, List<GraphNodeRef> nodes)
    {
        var parent = Graph.GetParent(node.Node);
        if (!parent)
            return false;
        if (nodes.Contains(parent))
            return true;

        return CheckNodeParentInList(parent, nodes);
    }

    protected bool MatchNodeDrag(GraphNode node, Vector2 mouseWordDrag)
    {
        if (node.ChildCount + Draging.Nodes.Count > node.MaxChildrenCount)
            return false;
        if (!NodeAreas.TryGetValue(node.GUID, out var areaInfo))
            return false;
        if (!DragAreaCheck(areaInfo, mouseWordDrag))
            return false;
        if (areaInfo.Contains(mouseWordDrag))
        {
            foreach (var rf in Draging.Nodes)
            {
                if (!Graph.CheckInstert(rf.Node.NodeData.GetType(), node.NodeData.GetType()))
                    return false;
            }
            int index = GetInsertIndex(node, areaInfo, mouseWordDrag);
            if (index == -1)
                return false;
            Draging.Parent = node;
            Draging.Index = index;
            return true;
        }
        else
        {
            foreach (var link in Graph.Links)
            {
                if (link.From.GUID == node.GUID)
                {
                    if (MatchNodeDrag(link.To.Node, mouseWordDrag))
                    {
                        return true;
                    }
                }
            }
            
        }
        return false;
    }

    protected abstract float UpdateNodeSpace(GraphNode node);
    protected abstract void DrawLine(Rect from, Rect to, Color lineColor, float width);
    protected abstract void UpdateNodePos();
    protected abstract bool DragAreaCheck(Rect area, Vector2 mousInWorld);
    protected abstract int GetInsertIndex(GraphNode node, Rect areaInfo, Vector2 mousInWorld);
    protected abstract Rect GetChildPlaceholderRect(GraphNode parent, int index);

    protected virtual void DrawNode(GUICamera camera, GraphNode node)
    {
        if (Draging.Nodes.Contains(node))
            return;
        Rect selfInView = camera.WorldToScreen(node.Bounds);
        if (Graph.HasParent(node))
        {
            Rect parentInView = camera.WorldToScreen(Graph.GetParent(node).Node.Bounds);
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
                if (Draging.Nodes.Exists(obj=> Graph.GetParent(obj.Node) == Draging.Parent))
                {
                    Rect bounds = GetChildPlaceholderRect(node, Draging.Index);
                    Rect boundsInView = camera.WorldToScreen(bounds);
                    DrawLine(selfInView, boundsInView, Color.yellow, camera.Scale * LineWidth);
                    GUI.Box(boundsInView, "", AutoGraphStyles.NodePlaceholder);
                }
            }
            foreach (var link in Graph.Links)
            {
                if (link.From.Node != node)
                    continue;
                DrawNode(camera, link.To.Node);
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
        bool isFreeNode = Graph.HasParent(node);
        if (isFreeNode && node.FoldChildren && selected)
            return AutoGraphStyles.FoldFreeNodeSelect;
        if (isFreeNode && node.FoldChildren)
            return AutoGraphStyles.FoldFreeNode;
        if (isFreeNode)
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
