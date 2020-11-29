using System;
using UnityEngine;
[Serializable]
public struct StateNodeRef : IEquatable<StateNodeRef>
{
    [SerializeField]
    private ulong id;
    [SerializeField]
    private StateGraph graph;
    private int version;
    private StateNode node;
    public static StateNodeRef Empty = new StateNodeRef();
    
    public static StateNodeRef CreateNodeRef(StateGraph graph, ulong id)
    {
        return new StateNodeRef { graph = graph, id = id };
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
        return exists.Node != null;
    }
}
