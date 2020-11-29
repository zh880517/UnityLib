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

    public void AddLink(StateNodeRef from, StateNodeRef to)
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
        from.Node.Parent = StateNodeRef.Empty;
        to.Node.Parent = StateNodeRef.Empty;
    }

    public void AddChild(StateNodeRef parent, StateNodeRef child, int index)
    {
        var link = Links.Find(item => item.From == parent && item.To == child);
        if (link != null)
        {
            Links.Remove(link);
        }
        else
        {
            link = new StateNodeLink { From = parent, To = child };
        }
        link.IsChild = true;
        int insertIndex = Links.Count;
        for (int i=0; i<Links.Count; ++i)
        {
            var item = Links[i];
            if (item.IsChild && item.From == parent)
            {
                if (index == 0)
                {
                    insertIndex = i;
                    break; ;
                }
                index--;
            }
        }
        Links.Insert(insertIndex, link);
        parent.Node.Parent = StateNodeRef.Empty;
        child.Node.Parent = parent;
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
        for (int i=0; i<Nodes.Count; ++i)
        {
            Nodes[i].SortIndex = i;
        }
    }
}
