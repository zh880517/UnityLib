﻿using UnityEngine;

public class GraphHorizontalLayout : GraphAutoLayout
{
    public static readonly float NODE_SPACE_HEIGH = NODE_SIZE.y + 10f;
    public static readonly float NODE_SPACE_WIDTH = NODE_SIZE.x + 30f;

    protected override Rect GetChildPlaceholderRect(GraphNode parent, int index)
    {
        Rect rect = new Rect(Vector2.zero, NODE_SIZE);
        if (NodeAreas.TryGetValue(parent.GUID, out var area))
        {
            Vector2 position = area.position;
            int idx = 0;
            foreach (var link in Graph.Links)
            {
                if (link.From.Node != parent)
                    continue;
                if (idx == index)
                    break;
                position.y = link.To.Node.Space;
                idx++;
            }
            rect.position = position;
        }
        return rect;
    }

    protected override bool DragAreaCheck(Rect area, Vector2 mousInWorld)
    {
        if (area.yMin > mousInWorld.y || area.yMax < mousInWorld.y || mousInWorld.x < area.xMin)
        {
            return false;
        }
        return true;
    }

    protected override void DrawLine(Rect from, Rect to, Color lineColor, float width)
    {
        Vector2 pointStart = from.max - new Vector2(0, from.height * 0.5f);
        Vector2 pointEnd = to.min + new Vector2(0, to.height*0.5f);

        Vector2 point1 = pointStart + new Vector2(10, 0);
        Vector2 point2 = new Vector2(point1.x, pointEnd.y);

        GraphRenderHelper.DrawLine(lineColor, width, pointStart, point1, point2, pointEnd);
        GraphRenderHelper.DrawArrow(lineColor, 1, point2 - pointEnd, pointEnd);
    }

    protected override int GetInsertIndex(GraphNode node, Rect areaInfo, Vector2 mousInWorld)
    {
        float posy = (mousInWorld - areaInfo.position).y;
        int index = 0;
        foreach (var link in Graph.Links)
        {
            if (link.From.Node != node)
                continue;
            var child = link.To;
            if (Draging.Nodes.Contains(child))
                continue;
            float spaceHeigh = child.Node.Space;
            if (posy < spaceHeigh * 0.5f)
                return index;
            posy -= spaceHeigh;
            ++index;
        }
        return node.ChildCount;
    }

    protected override void UpdateNodePos()
    {
        float heigh = 0;
        foreach (var node in Graph.Nodes)
        {
            if (node.IsRoot)
            {
                heigh += node.Space;
            }
        }
        float startY = heigh * -0.5f;
        foreach (var node in Graph.Nodes)
        {
            if (!Graph.HasParent(node))
            {
                if (node.IsRoot)
                {
                    Vector2 pos = new Vector2(0, startY + node.Space * 0.5f - NODE_SIZE.y*0.5f);
                    node.Bounds = new Rect(pos, NODE_SIZE);
                    startY += node.Space;
                }
                UpdateNodeChildPos(node);
            }
        }
    }

    protected void UpdateNodeChildPos(GraphNode node)
    {
        if (node.MaxChildrenCount == 0)
            return;
        if (node.FoldChildren)
            return;
        if (Draging.Nodes.Contains(node))
            return;

        Rect childrenArea = new Rect
        {
            position = node.Bounds.position + new Vector2(NODE_SPACE_WIDTH, -node.Space * 0.5f + NODE_SPACE_HEIGH * 0.5f),
            size = new Vector2(NODE_SPACE_WIDTH, node.Space)
        };
        NodeAreas.Add(node.GUID, childrenArea);
        Vector2 startPos = childrenArea.position;
        int index = 0;
        foreach (var link in Graph.Links)
        {
            if (link.From.Node != node)
                continue;
            var child = link.To;
            if (Draging.Parent.GUID == node.GUID)
            {
                if (index == Draging.Index)
                {
                    startPos.y += NODE_SPACE_HEIGH;
                }
            }
            if (!Draging.Nodes.Contains(child))
            {
                Vector2 pos = startPos;
                startPos.y += child.Node.Space;

                pos.y += (child.Node.Space * 0.5f - NODE_SIZE.y * 0.5f);
                child.Node.Bounds = new Rect(pos, NODE_SIZE);
                UpdateNodeChildPos(child.Node);
            }
            ++index;
        }
    }

    protected override float UpdateNodeSpace(GraphNode node)
    {
        float space = 0;
        do 
        {
            if (Draging.Nodes.Contains(node))
                break;
            if (node.FoldChildren || node.ChildCount == 0)
            {
                space = NODE_SPACE_HEIGH;
                break;
            }
            foreach (var link in Graph.Links)
            {
                if (link.From.Node != node)
                    continue;
                space += UpdateNodeSpace(link.To.Node);
            }
            if (Draging.Parent == (GraphNodeRef)node)
            {
                space += NODE_SPACE_HEIGH;
            }
        } while (false);
        node.Space = space;
        return space;
    }
}
