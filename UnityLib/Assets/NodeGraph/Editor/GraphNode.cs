using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GraphNode : ISerializationCallbackReceiver
{
    public Rect Bounds;
    public bool Selected;
    public string GUID;
    public Rect ChildrenArea { get; set; }
    public float Space { get; set; }
    public bool IsFreeNode { get { return !Parent && (NodeData == null || NodeData.IsRoot); } }
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
