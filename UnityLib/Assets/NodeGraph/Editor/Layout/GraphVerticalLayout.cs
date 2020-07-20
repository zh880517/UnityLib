using UnityEngine;

public class GraphVerticalLayout : GraphAutoLayout
{
    public static readonly float NODE_SPACE_HEIGH = NODE_SIZE.y + 30f;
    public static readonly float NODE_SPACE_WIDTH = NODE_SIZE.x + 10f;
    protected override Rect GetChildPlaceholderRect(GraphNode parent, int index)
    {
        Rect rect = new Rect(Vector2.zero, NODE_SIZE);
        if (NodeAreas.TryGetValue(parent.GUID, out var area))
        {
            Vector2 position = area.position;
            for (int i = 0; i < parent.Children.Count; ++i)
            {
                if (i == index)
                    break;
                position.x += parent.Children[i].Node.Space;
            }
        }
        return rect;
    }

    protected override bool DragAreaCheck(Rect area, Vector2 mousInWorld)
    {
        if (area.xMin > mousInWorld.x || area.xMax < mousInWorld.x || mousInWorld.y < area.yMin)
        {
            return false;
        }
        return true;
    }

    protected override void DrawLine(Rect from, Rect to, Color lineColor, float width)
    {
        Vector2 pointStart = from.max - new Vector2(from.width * 0.5f, 0);
        Vector2 pointEnd = to.min + new Vector2(to.width * 0.5f, 0);

        Vector2 point1 = pointStart + new Vector2(0, 10);
        Vector2 point2 = new Vector2(pointEnd.x, point1.y);

        GraphRenderHelper.DrawLine(lineColor, width, pointStart, point1, point2, pointEnd);
        GraphRenderHelper.DrawArrow(lineColor, 1, point2 - pointEnd, pointEnd);
    }

    protected override int GetInsertIndex(GraphNode node, Rect areaInfo, Vector2 mousInWorld)
    {
        float posX = (mousInWorld - areaInfo.position).x;
        for (int i = 0; i < node.Children.Count; ++i)
        {
            var child = node.Children[i];
            if (Draging.Nodes.Contains(child))
                continue;
            float spaceWidth = child.Node.Space;
            if (posX < spaceWidth * 0.5f)
                return i;
            posX -= spaceWidth;
        }
        return node.Children.Count;
    }

    protected override void UpdateNodePos()
    {
        float width = 0;
        foreach (var node in Graph.Nodes)
        {
            if (node.IsRoot)
            {
                width += node.Space;
            }
        }
        float startX = width * -0.5f;
        foreach (var node in Graph.Nodes)
        {
            if (!node.Parent)
            {
                if (node.IsRoot)
                {
                    Vector2 pos = new Vector2(startX + node.Space * 0.5f - NODE_SIZE.x * 0.5f, 0);
                    node.Bounds = new Rect(pos, NODE_SIZE);
                    startX += node.Space;
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
            position = node.Bounds.position + new Vector2(-node.Space * 0.5f + NODE_SPACE_WIDTH * 0.5f, NODE_SPACE_HEIGH),
            size = new Vector2(node.Space, NODE_SPACE_HEIGH)
        };
        NodeAreas.Add(node.GUID, childrenArea);
        Vector2 startPos = childrenArea.position;
        for (int i = 0; i < node.Children.Count; ++i)
        {
            var child = node.Children[i];
            if (Draging.Parent.GUID == node.GUID)
            {
                if (i == Draging.Index)
                {
                    startPos.x += NODE_SPACE_WIDTH;
                }
            }
            if (!Draging.Nodes.Contains(child))
            {
                Vector2 pos = startPos;
                startPos.x += child.Node.Space;

                pos.x += (child.Node.Space * 0.5f - NODE_SIZE.x * 0.5f);
                child.Node.Bounds = new Rect(pos, NODE_SIZE);
                UpdateNodeChildPos(child.Node);
            }
        }
    }

    protected override float UpdateNodeSpace(GraphNode node)
    {
        float space = 0;
        do
        {
            if (Draging.Nodes.Contains(node))
                break;
            if (node.FoldChildren || node.Children.Count == 0)
            {
                space = NODE_SPACE_WIDTH;
                break;
            }

            foreach (var child in node.Children)
            {
                space += UpdateNodeSpace(child.Node);
            }
            if (Draging.Parent == (GraphNodeRef)node)
            {
                space += NODE_SPACE_WIDTH;
            }
        } while (false);
        node.Space = space;
        return space;
    }
}
