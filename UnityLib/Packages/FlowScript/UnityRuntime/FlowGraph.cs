using System.Collections.Generic;
using UnityEngine;
namespace Flow
{
    [System.Serializable]
    public class SubGraphBind
    {
        public string NodeGUID;
        public string GraphGUID;
    }
    public class FlowGraph : ScriptableObject
    {
        public List<FlowNode> Nodes = new List<FlowNode>();
        public List<FlowSubGraph> SubGraphs = new List<FlowSubGraph>();
        public List<FlowNodeViewData> NodeViews = new List<FlowNodeViewData>();
        public List<SubGraphBind> GraphBinds = new List<SubGraphBind>();
        public List<FlowEdgeData> Edges = new List<FlowEdgeData>();

        public FlowNode FindNode(string guid)
        {
            return Nodes.Find(it => it.GUID == guid);
        }

        public bool HasNode(string guid)
        {
            return Nodes.Exists(it => it.GUID == guid);
        }

        public FlowSubGraph FindSubGraph(string guid)
        {
            return SubGraphs.Find(it => it.GUID == guid);
        }
        public FlowSubGraph FindBindSubGraph(FlowNode node)
        {
            foreach (var  bind in GraphBinds)
            {
                if (bind.NodeGUID == node.GUID)
                {
                    return FindSubGraph(bind.GraphGUID);
                }
            }
            return null;
        }
    }
}