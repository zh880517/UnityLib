using UnityEditor;

namespace Flow
{
    public static class GraphUtil
    {
        public static void RegisterUndo(FlowGraph graph, string name )
        {
            Undo.RegisterCompleteObjectUndo(graph, name);
            EditorUtility.SetDirty(graph);
        }

        public static FlowSubGraph CreateSubGraph(FlowGraph graph, bool allowStageNode)
        {
            FlowSubGraph subGraph = new FlowSubGraph
            {
                Owner = graph,
                AllowStageNode = allowStageNode,
                GUID = System.Guid.NewGuid().ToString(),
            };
            graph.SubGraphs.Add(subGraph);
            return subGraph;
        }

        public static void BindSubGraphToNode(FlowSubGraph subGraph, FlowNode node)
        {
            //第一个为主Graph，不能和节点绑定
            if (subGraph.Owner.SubGraphs[0].GUID == subGraph.GUID)
                return;
            //节点不再当前Graph中，也不能绑定
            if (!subGraph.Owner.HasNode(node.GUID))
                return;
            //如果已经存在绑定关系，也不能绑定
            if (subGraph.Owner.GraphBinds.Exists(it => it.NodeGUID == node.GUID || it.GraphGUID == subGraph.GUID))
                return;
            SubGraphBind bind = new SubGraphBind { NodeGUID = node.GUID, GraphGUID = subGraph.GUID };
            subGraph.Owner.GraphBinds.Add(bind);
        }

        public static void RemoveNode(FlowGraph graph, string guid)
        {
            var index = graph.Nodes.FindIndex(it => it.GUID == guid);
            if (index < 0)
                return;
            graph.Nodes.RemoveAt(index);
            graph.NodeViews.RemoveAll(it=>it.NodeGUID == guid);
            graph.Edges.RemoveAll(it => it.FromNode == guid || it.ToNode == guid);
            foreach (var sunbGraph in graph.SubGraphs)
            {
                sunbGraph.Nodes.RemoveAll(it => it.GUID == guid);
            }
            var bind = graph.GraphBinds.Find(it => it.NodeGUID == guid);
            if (bind != null)
            {
                RemoveSubGraph(graph, bind.GraphGUID);
            }
        }

        public static void RemoveSubGraph(FlowGraph graph, string subGraphID)
        {
            int index = graph.SubGraphs.FindIndex(it => it.GUID == subGraphID);
            if (index < 0)
                return;
            var subGraph = graph.SubGraphs[index];
            graph.SubGraphs.RemoveAt(index);
          
            int bindIndex = graph.GraphBinds.FindIndex(it => it.GraphGUID == subGraphID);
            if (bindIndex >= 0)
            {
                graph.GraphBinds.RemoveAt(bindIndex);
            }
            if (subGraph.Nodes.Count > 0)
            {
                //此处防止循环调用
                var nodes = subGraph.Nodes.ToArray();
                foreach (var node in nodes)
                {
                    RemoveNode(graph, node.GUID);
                }
            }
        }

    }
}