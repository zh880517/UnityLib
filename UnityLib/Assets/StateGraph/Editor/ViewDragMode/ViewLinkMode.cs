using UnityEngine;

public class ViewLinkMode : IViewDragMode
{
    private Vector2 startPos;
    private Vector2 currentPos;
    private StateNodeRef node;
    private bool isOut;
    private bool isChild;

    public ViewLinkMode(StateGraphView view, StateNodeRef node, bool isOut, bool isChild)
    {
        this.node = node;
        this.isOut = isOut;
        this.isChild = isChild;
        if (isOut)
        {
            startPos = view.Graph.GetOutputPin(node.Node);
        }
        else
        {
            startPos = view.Graph.GetInputPin(node.Node);
        }
    }

    public void Draw(StateGraphView view)
    {
        view.Canvas.DrawLinkLines(startPos, currentPos, Color.yellow, 5);
    }

    public void OnDrag(StateGraphView view, Vector2 ptInWorld)
    {
        currentPos = ptInWorld;
    }

    public void OnDragEnd(StateGraphView view, Vector2 ptInWorld)
    {
        var currNode = view.HitTest(ptInWorld);
        if (currNode != null)
        {
            StateNode from;
            StateNode to;
            if (isOut)
            {
                from = node.Node;
                to = currNode;
            }
            else
            {
                to = node.Node;
                from = currNode;
            }
            view.CreateLink(from, to, isChild);
        }
        else
        {
            //此处弹出创建节点下拉窗口
        }
    }
}
