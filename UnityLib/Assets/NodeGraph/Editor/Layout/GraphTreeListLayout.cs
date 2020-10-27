using UnityEngine;

public class GraphTreeListLayout : GraphAutoLayout
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
                position.y += link.To.Node.Space;
                ++idx;
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
        Vector2 pointEnd = to.min + new Vector2(0, to.height * 0.5f);

        Vector2 center = new Vector2(pointStart.x, pointEnd.y);

        GraphRenderHelper.DrawLine(lineColor, width, pointStart, center, pointEnd);
    }

    protected override int GetInsertIndex(GraphNode node, Rect areaInfo, Vector2 mousInWorld)
    {
        float posy = (mousInWorld - areaInfo.position).y;
        int idx = 0;
        foreach (var link in Graph.Links)
        {
            if (link.From.Node != node)
                continue;
            if (Draging.Nodes.Contains(link.To))
                continue;
            float spaceHeigh = link.To.Node.Space;
            if (posy < spaceHeigh * 0.5f)
                return idx;
            posy -= spaceHeigh;
            ++idx;
        }
        return node.ChildCount;
    }

    protected override void UpdateNodePos()
    {
        float startX = 0;
        foreach (var node in Graph.Nodes)
        {
            if (!Graph.HasParent(node))
            {
                if (node.IsRoot)
                {
                    Vector2 pos = new Vector2(startX, 0);
                    node.Bounds = new Rect(pos, NODE_SIZE);
                    startX += node.Space;
                }
                UpdateNodeChildPos(node);
                float maxX = GetNodeMaxX(node, float.MinValue);
                startX += (maxX + NODE_SIZE.x * 0.5f);
            }
        }
    }

    protected float GetNodeMaxX(GraphNode node, float maxX)
    {
        if (node.Bounds.xMax > maxX)
        {
            maxX = node.Bounds.xMax;
        }
        if (!node.FoldChildren)
        {
            foreach (var link in Graph.Links)
            {
                if (link.From.Node != node)
                    continue;
                if (!Draging.Nodes.Contains(link.To))
                {
                    maxX = GetNodeMaxX(link.To.Node, maxX);
                }
            }
        }
        return maxX;
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
            position = node.Bounds.position + new Vector2(NODE_SIZE.x*0.5f, NODE_SPACE_HEIGH),
            size = new Vector2(NODE_SPACE_WIDTH, node.Space)
        };
        NodeAreas.Add(node.GUID, childrenArea);
        Vector2 startPos = childrenArea.position;
        int index = 0;
        foreach (var link in Graph.Links)
        {
            if (link.From.Node != node)
                continue;
            if (Draging.Parent.GUID == node.GUID)
            {
                if (index == Draging.Index)
                {
                    startPos.y += NODE_SPACE_HEIGH;
                }
            }
            if (!Draging.Nodes.Contains(link.To))
            {
                Vector2 pos = startPos;
                startPos.y += link.To.Node.Space;

                pos.y += (link.To.Node.Space * 0.5f - NODE_SIZE.y * 0.5f);
                link.To.Node.Bounds = new Rect(pos, NODE_SIZE);
                UpdateNodeChildPos(link.To.Node);
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
            space = NODE_SPACE_HEIGH;
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
