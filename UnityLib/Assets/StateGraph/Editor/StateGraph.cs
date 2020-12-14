using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateGraph : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [HideInInspector]
    private ulong IdIndex;
    public int SerializeVersion { get; private set; } = 1;
    public List<StateNode> Nodes = new List<StateNode>();
    public List<StateNodeLink> Links = new List<StateNodeLink>();
    public StateGraphBlackboard Blackboard = new StateGraphBlackboard();

    public StateNode FindNode(ulong id)
    {
        return Nodes.Find(obj => obj.ID == id);
    }

    public StateNode AddNode(IStateNode nodeData, Rect bounds)
    {
        StateNode node = new StateNode
        {
            Bounds = bounds,
            ID = ++IdIndex,
            NodeData = nodeData,
            Graph = this,
        };

        return node;
    }

    public void AddLink(StateNodeRef from, StateNodeRef to, bool isChild)
    {
        if (!from || !to || from != to)
            return;
        if (!IsStack(from.Node))
        {
            Links.RemoveAll(it => it.From == from);
        }
        Links.RemoveAll(item => item.To == to && item.From == from);
        Links.Add(new StateNodeLink { From = from, To = to, IsChild = isChild });
        from.Node.Parent = StateNodeRef.Empty;
        if (isChild)
        {
            to.Node.Parent = from;
        }
        else
        {
            to.Node.Parent = StateNodeRef.Empty;
        }
    }

    public void DeleteNode(StateNodeRef node)
    {
        for (int i=Links.Count-1; i>=0; --i)
        {
            var link = Links[i];
            if (link.From == node)
            {
                link.To.Node.Parent = StateNodeRef.Empty;
                Links.RemoveAt(i);
                continue;
            }
            if (link.To == node)
            {
                Links.RemoveAt(i);
                continue;
            }
        }
        Nodes.Remove(node.Node);
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        SerializeVersion++;
        for (int i = 0; i < Nodes.Count; ++i)
        {
            Nodes[i].SortIndex = i;
        }
    }

    public virtual bool CheckLink(StateNode from, StateNode to, bool isChild)
    {
        if (from == to || Links.Exists(obj=>obj.From == from && obj.To == to && obj.IsChild == isChild))
        {
            return false;
        }
        return true;
    }

    public virtual bool CheckDelete(StateNodeRef node)
    {
        return node.Id > 1;
    }

    public virtual bool CheckCopy(StateNodeRef node)
    {
        return node.Id > 1;
    }

    public abstract bool IsStack(StateNode node);

    public abstract bool CheckChildType(StateNode parent, Type childType);

    public abstract bool CheckTypeValid(Type type);

    public abstract bool CheckReplace(Type src, Type dst);
}
