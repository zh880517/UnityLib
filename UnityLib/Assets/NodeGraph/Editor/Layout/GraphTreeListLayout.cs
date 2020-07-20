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
        throw new System.NotImplementedException();
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
