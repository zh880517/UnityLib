using System.Collections.Generic;
using UnityEngine;

public class StateGraph : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [HideInInspector]
    private ulong IdIndex;
    public int SerializeVersion { get; private set; } = 1;
    public List<StateNode> Nodes = new List<StateNode>();
    public List<StateNodeLink> Links = new List<StateNodeLink>();

    public StateNode FindNode(ulong id)
    {
        return Nodes.Find(obj => obj.ID == id);
    }

    public StateNode AddNode<T>(T nodeData, Rect bounds) where T : IStateNode
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
        var link = Links.Find(item => item.From == from && item.To == to);
        if (link != null)
        {
            link.IsChild = false;
        }
        else
        {
            Links.Add(new StateNodeLink { From = from, To = to });
        }
        link.IsChild = isChild; 
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

    public void BreakLink(StateNodeRef from, StateNodeRef to)
    {
        Links.RemoveAll(item => item.From == from && item.To == to);
        to.Node.Parent = StateNodeRef.Empty;
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

    public virtual Vector2 GetOutputPin(StateNode node)
    {
        return node.Bounds.position + new Vector2(0, 25);
    }

    public virtual Vector2 GetInputPin(StateNode node)
    {
        return node.Bounds.max - new Vector2(0, 25);
    }

    public virtual bool IsChildNode(StateNode node)
    {
        if (node.NodeData != null && (node.NodeData is IStateAction) || (node.NodeData is IStateBranch))
        {
            return true;
        }
        return false;
    }

    public virtual bool CheckLink(StateNode from, StateNode to, bool isChild)
    {
        if (Links.Exists(obj=>obj.From == from && obj.To == to && obj.IsChild == isChild))
        {
            return false;
        }
        return true;
    }

}
