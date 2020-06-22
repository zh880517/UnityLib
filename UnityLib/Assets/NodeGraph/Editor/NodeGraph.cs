using System.Collections.Generic;
using UnityEngine;

public class NodeGraph : ScriptableObject
{
    public List<GraphNode> Nodes;

    public GraphNode GetNode(string guid)
    {
        return Nodes.Find(obj => obj.GUID == guid);
    }
}
