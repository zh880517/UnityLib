using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateGraphView : ScriptableObject
{
    public static readonly Vector2 NODE_SIZE = new Vector2(100, 50);
    public const float CHILD_INTERVAL = 5;
    public static readonly Vector2 CHILD_OFFSET = new Vector2(10, 20);
    public static readonly Vector2 CHILD_NODE_SIZE = new Vector2(80, 50);

    public StateGraph Graph;
    public List<StateNodeRef> Selecteds = new List<StateNodeRef>();
    private int selectIndex;//

    public void Init(StateGraph graph)
    {
        Graph = graph;
        selectIndex = 0;
        if (graph.Nodes.Count > 0)
        {
            selectIndex = graph.Nodes.Last().SortIndex;
        }
    }

    protected virtual void UpdateBounds()
    {
        for (int i=Graph.Nodes.Count-1; i>=0; --i)
        {
            var node = Graph.Nodes[i];
            if (!node.Parent)
            {
                Vector2 size = NODE_SIZE;
                Vector2 pos = node.Bounds.position + CHILD_OFFSET;
                foreach (var link in Graph.Links)
                {
                    if (link.IsChild && link.From.Node == node)
                    {
                        link.To.Node.Bounds = new Rect(pos, CHILD_NODE_SIZE);
                        size.y += CHILD_NODE_SIZE.y;
                        pos.y += (CHILD_NODE_SIZE.y + CHILD_INTERVAL);
                    }
                }
                node.Bounds.size = size;
            }
        }
    }

    public StateNode HitTest(Vector2 ptInWorld)
    {
        for (int i=Graph.Nodes.Count-1; i>=0; --i)
        {
            var node = Graph.Nodes[i];
            if (node.Bounds.Contains(ptInWorld))
            {
                return node;
            }
        }
        return null;
    }

    public void RectSelect(Rect bounds)
    {
        foreach (var node in Graph.Nodes )
        {
            if (node.Bounds.Overlaps(bounds) && !Selecteds.Contains(node))
            {
                Selecteds.Add(node);
                node.SortIndex = selectIndex++;
                foreach (var link in Graph.Links)
                {
                    if (link.IsChild && link.From == node)
                    {
                        link.To.Node.SortIndex = selectIndex++;
                    }
                }
            }
        }
    }

}
