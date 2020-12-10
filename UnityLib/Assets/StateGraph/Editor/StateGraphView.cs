using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StateGraphView : ScriptableObject
{
    public static readonly Vector2 NODE_SIZE = new Vector2(100, 50);
    public const float CHILD_INTERVAL = 5;
    public static readonly Vector2 CHILD_OFFSET = new Vector2(10, 20);
    public static readonly Vector2 CHILD_NODE_SIZE = new Vector2(80, 50);

    public StateGraph Graph;
    public List<StateNodeRef> Selecteds = new List<StateNodeRef>();
    public GUICanvas Canvas = new GUICanvas();
    public int SelectIndex { get; set; }
    private IViewDragMode DragMode;

    public void Init(StateGraph graph)
    {
        Graph = graph;
        SelectIndex = 0;
        if (graph.Nodes.Count > 0)
        {
            SelectIndex = graph.Nodes.Last().SortIndex;
        }
    }

    public bool OnDraw(Rect viewArea)
    {
        Event e = Canvas.OnGUI(viewArea);
        DrawLinkLins();
        DrawNodes();
        if (DragMode != null)
        {
            DragMode.Draw(this);
        }
        return e.type == EventType.Used;
    }

    protected virtual void UpdateBounds(StateNode node)
    {
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

    private void DrawLinkLins()
    {
        foreach (var link in Graph.Links)
        {
            if (!link.IsChild)
            {
                Vector2 from = Graph.GetOutputPin(link.From.Node);
                Vector2 to = Graph.GetInputPin(link.To.Node);
                Canvas.DrawLinkLines(from, to, Color.white, 5);
            }
        }
    }

    private void DrawNodes()
    {
        foreach (var node in Graph.Nodes)
        {

        }
    }

    private void OnEvent(Event e)
    {
        
    }

    public void CreateLink(StateNode from, StateNode to, bool isChild)
    {
        if (Graph.CheckLink(from, to, isChild))
        {
            RegistUndo("link");
            var oldLink = Graph.Links.Find(obj => obj.From == from && obj.To == to);
            Graph.AddLink(from, to, isChild);
            if (isChild)
            {
                UpdateBounds(from);
            }
            if (oldLink != null && oldLink.IsChild && oldLink.From != from)
            {
                UpdateBounds(oldLink.From.Node);
            }
        }
    }

    public void RegistUndo(string name)
    {
        Undo.RegisterCompleteObjectUndo(Graph, name);
        Undo.RegisterCompleteObjectUndo(this, name);
        EditorUtility.SetDirty(Graph);
    }
}
