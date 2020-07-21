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
            for (int i = 0; i < parent.Children.Count; ++i)
            {
                if (i == index)
                    break;
                position.y += parent.Children[i].Node.Space;
            }
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
        for (int i = 0; i < node.Children.Count; ++i)
        {
            var child = node.Children[i];
            if (Draging.Nodes.Contains(child))
                continue;
            float spaceHeigh = child.Node.Space;
            if (posy < spaceHeigh * 0.5f)
                return i;
            posy -= spaceHeigh;
        }
        return node.Children.Count;
    }

    protected override void UpdateNodePos()
    {
        float startX = 0;
        foreach (var node in Graph.Nodes)
        {
            if (!node.Parent)
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
            foreach (var rf in node.Children)
            {
                if (!Draging.Nodes.Contains(rf))
                {
                    maxX = GetNodeMaxX(rf.Node, maxX);
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
        for (int i = 0; i < node.Children.Count; ++i)
        {
            var child = node.Children[i];
            if (Draging.Parent.GUID == node.GUID)
            {
                if (i == Draging.Index)
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
                space = NODE_SPACE_HEIGH;
                break;
            }
            space = NODE_SPACE_HEIGH;
            foreach (var child in node.Children)
            {
                space += UpdateNodeSpace(child.Node);
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
