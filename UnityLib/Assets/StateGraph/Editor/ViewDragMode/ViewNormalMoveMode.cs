using UnityEditor;
using UnityEngine;

public class ViewNormalMoveMode : IViewDragMode
{
    private Vector2 Start;

    public ViewNormalMoveMode(StateGraphView view, Vector2 ptInWorld)
    {
        view.RegistUndo("move");
        Start = ptInWorld;
    }

    public void Draw(StateGraphView view)
    {
        
    }

    public void OnDrag(StateGraphView view, Vector2 ptInWorld)
    {
        Vector2 offset = ptInWorld - Start;

        //先移动选择的节点
        foreach (var node in view.Selecteds)
        {
            node.Node.Bounds.position += offset;
        }
        //再移动选择节点的子节点
        foreach (var node in view.Graph.Nodes)
        {
            if (node.Parent && view.Selecteds.Contains(node.Parent))
            {
                node.Bounds.position += offset;
            }
        }
    }

    public void OnDragEnd(StateGraphView view, Vector2 ptInWorld)
    {
    }

}
