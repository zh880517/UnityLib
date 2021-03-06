﻿using System;
using UnityEngine;
[Serializable]
public class GraphNode : ISerializationCallbackReceiver
{
    [HideInInspector]
    public Rect Bounds;
    [HideInInspector]
    public bool Selected;
    [HideInInspector]
    public string GUID;
    [HideInInspector]
    public bool FoldChildren;
    [HideInInspector]
    public float Space;
    public bool IsRoot => NodeData != null && NodeData.IsRoot;
    public int MaxChildrenCount { get { return NodeData == null ? 0 : NodeData.MaxCount; } }
    [SerializeField]
    [HideInInspector]
    private JsonElement jsonData;
    [HideInInspector]
    public NodeGraph Graph;
    public BaseNode NodeData;
    public int ChildCount;

    public static GraphNode CreateByType(Type nodeType)
    {
        try
        {

            GraphNode node = new GraphNode
            {
                GUID = Guid.NewGuid().ToString(),
                NodeData = (BaseNode)Activator.CreateInstance(nodeType)
            };
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
