using System.Collections.Generic;
using UnityEngine;

public class ViewNormalMoveMode : IViewDragMode
{
    private Vector2 Start;
    private List<StateNodeRef> movedNodes = new List<StateNodeRef>();

    public ViewNormalMoveMode(StateGraphView view, Vector2 ptInWorld)
    {
        view.RegistUndo("move");
        Start = ptInWorld;
        //如果选择的节点有父节点则记录父节点，否则记录自己，方便后续移动处理
        foreach (var node in view.Selecteds)
        {
            AddNode(view, node);
        }
    }

    private void AddNode(StateGraphView view, StateNodeRef nodeRef)
    {
        if (movedNodes.Contains(nodeRef))
            return;
        movedNodes.Add(nodeRef);
        if (nodeRef.Node.Parent)
        {
            AddNode(view, nodeRef.Node.Parent);
        }
        else
        {
            foreach (var link in view.Graph.Links)
            {
                if (link.IsChild && link.From == nodeRef)
                {
                    if (!movedNodes.Contains(link.To))
                        movedNodes.Add(link.To);
                }
            }
        }
    }

    public void Draw(StateGraphView view)
    {
        
    }

    public void OnDrag(StateGraphView view, Vector2 ptInWorld)
    {
        Vector2 offset = ptInWorld - Start;
        Start = ptInWorld;
        foreach (var node in movedNodes)
        {
            node.Node.Bounds.position += offset;
        }
    }

    public void OnDragEnd(StateGraphView view, Vector2 ptInWorld)
    {
    }

}
