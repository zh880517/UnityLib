using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeGraph : ScriptableObject
{
    public List<GraphNode> Nodes;

    public GraphNode GetNode(string guid)
    {
        return Nodes.Find(obj => obj.GUID == guid);
    }

    protected virtual void OnCreate()
    {

    }

    public static TGraph Require<TGraph>(string path) where TGraph : NodeGraph
    {
        TGraph graph = AssetDatabase.LoadAssetAtPath<TGraph>(path);
        if (graph == null)
        {
            graph = CreateInstance<TGraph>();
            graph.OnCreate();
            AssetDatabase.CreateAsset(graph, path);
        }
        return graph;
    }

    public virtual void ToCollection<TNode>(NodeCollection<TNode> collection) where TNode : INodeData
    {

    }
}
