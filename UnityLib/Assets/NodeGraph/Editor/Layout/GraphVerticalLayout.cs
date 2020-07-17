using UnityEngine;

public class GraphVerticalLayout : GraphAutoLayout
{
    public override Rect GetChildPlaceholderRect(GraphNode parent, int index)
    {
        throw new System.NotImplementedException();
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
