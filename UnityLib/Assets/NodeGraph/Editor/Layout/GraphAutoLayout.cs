using System.Collections.Generic;
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
    public struct DragNode
    {
        public GraphNodeRef Node;
        public GraphNodeRef Parent;
        public int Index;
    }
    protected Dictionary<string, NodeArea> NodeAreas = new Dictionary<string, NodeArea>();
    public DragNode Draging;
    public override bool EnableMultDrag => false;
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
            if (!node.Parent && Draging.Node.GUID != node.GUID)
            {
                DrawNode(camera, node);
            }
        }
        if (Draging.Node)
        {
            DrawDragingNode(camera);
        }
    }

    protected abstract void UpdateNodeSpace(GraphNode node);

    protected abstract void DrawLine(Rect from, Rect to, Color lineColor, float width);

    protected abstract void UpdaeNodePos();
    public abstract Rect GetChildPlaceholderRect(GraphNode parent, int index);

    protected virtual void DrawNode(GUICamera camera, GraphNode node)
    {
        if (Draging.Node.GUID == node.GUID)
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
                int index = Draging.Index;
                if (Draging.Node.Node.Parent == Draging.Parent)
                {
                    if (index >= Draging.Node.Node.IndexOfParent())
                    {
                        index++;
                    }
                    Rect bounds = GetChildPlaceholderRect(node, index);
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
