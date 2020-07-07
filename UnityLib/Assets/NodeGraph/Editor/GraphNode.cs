using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GraphNode : ISerializationCallbackReceiver
{
    public Rect Bounds;
    public bool Selected;
    public string GUID;
    public Rect ChildrenArea { get; set; }
    public float Space { get; set; }
    public bool IsFreeNode { get { return !Parent && (NodeData == null || NodeData.IsRoot); } }
    public bool IsRoot => NodeData != null && NodeData.IsRoot;
    public int MaxChildrenCount { get { return NodeData == null ? 0 : NodeData.MaxCount; } }
    [SerializeField]
    private JsonElement jsonData;
    public NodeGraph Graph;
    public BaseNode NodeData;
    public GraphNodeRef Parent;
    public List<GraphNodeRef> Children = new List<GraphNodeRef>();

    public static GraphNode CreateByType(Type nodeType)
    {
        try
        {

            GraphNode node = new GraphNode();
            node.GUID = Guid.NewGuid().ToString();
            node.NodeData = (BaseNode)Activator.CreateInstance(nodeType);
            return node;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void OnAfterDeserialize()
    {
        NodeData = JsonSerializer.DeserializeNode(jsonData);
    }

    public void OnBeforeSerialize()
    {
        jsonData = JsonSerializer.SerializeNode(NodeData);
    }

    public static implicit operator GraphNodeRef(GraphNode exists)
    {
        if (exists == null)
            return GraphNodeRef.Empty;
        return GraphNodeRef.CreateNodeRef(exists.Graph, exists.GUID);
    }
}
