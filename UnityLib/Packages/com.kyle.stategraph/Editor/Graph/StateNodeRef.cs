using System;
using UnityEngine;
[Serializable]
public struct StateNodeRef : IEquatable<StateNodeRef>
{
    [SerializeField]
    private ulong id;
    [SerializeField]
    [SerializeReference]
    private StateGraph graph;
    private int version;
    [NonSerialized]
    public StateNode node;
    public static StateNodeRef Empty = new StateNodeRef();
    
    public static StateNodeRef CreateNodeRef(StateNode node)
    {
        return new StateNodeRef { graph = node.Graph, id = node.ID, node = node };
    }

    public StateNode Node
    {
        get
        {
            if (id > 0 &&(node == null || version != graph.SerializeVersion) )
            {
                node = graph.FindNode(id);
                version = graph.SerializeVersion;
            }
            return node;
        }
    }

    public ulong Id => id;

    public bool Equals(StateNodeRef other)
    {
        return other.graph == graph && other.id == id;
    }

    public override bool Equals(object obj)
    {
        if (obj is StateNodeRef nodeRef)
        {
            return Equals(nodeRef);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public static bool operator == (StateNodeRef lhs, StateNodeRef rhs)
    {
        return lhs.Equals(rhs);
    }
    public static bool operator !=(StateNodeRef lhs, StateNodeRef rhs)
    {
        return !lhs.Equals(rhs);
    }
    public static implicit operator bool(StateNodeRef exists)
    {
        return exists.Id != 0;
    }
}
