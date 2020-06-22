using UnityEngine;
[System.Serializable]
public class GraphNodeRef
{
    [SerializeField]
    private string guid;
    [SerializeField]
    private NodeGraph graph;
    private GraphNode node;

    public static GraphNodeRef CreateNodeRef(NodeGraph graph, string guid)
    {
        return new GraphNodeRef { graph = graph, guid = guid };
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
