using UnityEngine;

public class ViewLinkMode : IViewDragMode
{
    private Vector2 startPos;
    private Vector2 currentPos;
    private StateNodeRef node;
    private readonly bool isOut;
    private readonly bool isChild;

    public ViewLinkMode(StateNodeRef node, bool isOut, bool isChild, Vector2 pos)
    {
        this.node = node;
        this.isOut = isOut;
        this.isChild = isChild;
        startPos = pos;
        currentPos = pos;
    }

    public void Draw(StateGraphView view)
    {
        if (Vector2.Distance(startPos, currentPos) > 1)
        {
            view.Canvas.DrawStraightLine(startPos, currentPos, Color.yellow, 3);
            var currNode = view.HitTest(currentPos);
            if (currNode != null && currNode != node)
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
                if (view.Graph.CheckLink(from, to, isChild))
                {
                    view.Canvas.DrawArea(currNode.Bounds, Color.yellow);
                }
            }
        }

    }

    public void OnDrag(StateGraphView view, Vector2 ptInWorld)
    {
        currentPos = ptInWorld;
    }

    public void OnDragEnd(StateGraphView view, Vector2 ptInWorld)
    {
        var currNode = view.HitTest(ptInWorld);
        if (currNode == node)
        {
            if(!isChild)
                return;
            currNode = null;
        }
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
            view.SelectNode(currNode);
        }
        else
        {
            //此处弹出创建节点下拉窗口
            StateNodeCreatDropdown dropDown = new StateNodeCreatDropdown(view, node, isOut, isChild);
            dropDown.Show(new Rect(view.Canvas.MouseInView, new Vector2(250, 0)));
        }
    }
}
