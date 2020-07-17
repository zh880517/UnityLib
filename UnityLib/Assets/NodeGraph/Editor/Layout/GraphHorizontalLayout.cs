using UnityEngine;

public class GraphHorizontalLayout : GraphAutoLayout
{
    public static readonly float NODE_SPACE_HEIGH = NODE_SIZE.y + 10f;
    public static readonly float NODE_SPACE_WIDTH = NODE_SIZE.x + 30f;

    public override Rect GetChildPlaceholderRect(GraphNode parent, int index)
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }

    protected override int GetInsertIndex(GraphNode node, Vector2 mousInWorld)
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdaeNodePos()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateNodeSpace(GraphNode node)
    {
        throw new System.NotImplementedException();
    }
}
