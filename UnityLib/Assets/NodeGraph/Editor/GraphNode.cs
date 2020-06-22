using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GraphNode : ISerializationCallbackReceiver
{
    public float Space;
    public Rect Bounds;
    public Rect ChildrenArea;
    public bool Selected;
    public bool FreeNode;
    public string GUID;

    [SerializeField]
    private JsonElement jsonData;
    public INodeData NodeData;
    public GraphNodeRef Parent;
    public List<GraphNodeRef> Children = new List<GraphNodeRef>();

    public void OnAfterDeserialize()
    {
        NodeData = JsonSerializer.DeserializeNode(jsonData);
    }

    public void OnBeforeSerialize()
    {
        jsonData = JsonSerializer.SerializeNode(NodeData);
    }
}
