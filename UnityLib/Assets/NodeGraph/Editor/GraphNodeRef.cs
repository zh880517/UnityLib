using UnityEngine;
[System.Serializable]
public struct GraphNodeRef
{
    [SerializeField]
    private string guid;
    [SerializeField]
    private NodeGraph graph;
    private GraphNode node;

    public static GraphNodeRef Empty = new GraphNodeRef();

    public static GraphNodeRef CreateNodeRef(NodeGraph graph, string guid)
    {
        return new GraphNodeRef { graph = graph, guid = guid };
    }

    public static implicit operator bool(GraphNodeRef exists)
    {
        return exists.Node != null;
    }

    public GraphNode Node
    {
        get
        {
            if (node == null && !string.IsNullOrEmpty(guid))
            {
                node = graph.GetNode(guid);
            }
            return node;
        }
    }
}
