using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateNodeClipboard
{
    public static Dictionary<Type, StateNodeClipboard> Clipboard = new Dictionary<Type, StateNodeClipboard>();
    public class NodeData
    {
        public SerializationData Data;
        public Rect Bounds;
        public string Name;
        public string Comments;
        public ulong Id;
    }

    public struct NodeLink
    {
        public int FromIdx;
        public int ToIdx;
        public bool IsChild;
    }

    public List<NodeData> Datas = new List<NodeData>();
    public List<NodeLink> Links = new List<NodeLink>();

    public bool CopyFrom(StateGraph graph, IEnumerable<StateNodeRef> copyNodes, Vector2 offset)
    {
        List<StateNodeRef> nodes = copyNodes.Where(it => graph.CopyCheck(it)).ToList();
        foreach (var node in nodes)
        {
            NodeData data = new NodeData 
            { 
                Id= node.Id,
                Bounds = node.Node.Bounds,
                Data = TypeSerializerHelper.Serialize(node.Node.NodeData),
                Name = node.Node.Name,
                Comments = node.Node.Comments,
            };
            data.Bounds.position += offset;
            Datas.Add(data);

        }
        foreach (var link in graph.Links)
        {
            if (nodes.Contains(link.From) && nodes.Contains(link.To))
            {
                NodeLink nodeLink = new NodeLink
                {
                    FromIdx = Datas.FindIndex(it => it.Id == link.From.Id),
                    ToIdx = Datas.FindIndex(it => it.Id == link.To.Id),
                    IsChild = link.IsChild
                };
                Links.Add(nodeLink);
            }
        }
        return nodes.Count > 0;
    }

    public List<StateNode> PasteTo(StateGraph graph, Vector2 offset)
    {
        List<StateNode> creatNodes = new List<StateNode>();
        foreach (var data in Datas)
        {
            var node = graph.AddNode(TypeSerializerHelper.Deserialize(data.Data) as IStateNode, new Rect(data.Bounds.position + offset, data.Bounds.size));
            node.Name = data.Name;
            node.Comments = data.Comments;
            creatNodes.Add(node);
        }
        foreach (var link in Links)
        {
            graph.AddLink(creatNodes[link.FromIdx], creatNodes[link.ToIdx], link.IsChild);
        }
        return creatNodes;
    }

}
