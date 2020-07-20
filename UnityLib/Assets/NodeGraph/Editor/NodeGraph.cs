using System;
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

    public virtual bool CheckReplace(Type nodeType, Type replaceType)
    {
        return nodeType.IsSubclassOf(typeof(BaseNode)) && replaceType.IsSubclassOf(typeof(BaseNode));
    }

    public virtual bool Replace(GraphNode node, Type replaceType)
    {
        //默认根节点无法替换
        if (node.IsRoot)
            return false;
        if (!replaceType.IsSubclassOf(typeof(BaseNode)))
            return false;
        Type nodeType = node.NodeData.GetType();
        if (nodeType != replaceType && !CheckReplace(nodeType, replaceType))
            return false;
        BaseNode newNode;
        try
        {
            newNode = (BaseNode)Activator.CreateInstance(replaceType);
        }
        catch (Exception )
        {
            return false;
        }
        Undo.RegisterCompleteObjectUndo(this, "replace node");
        node.NodeData = newNode;
        return true;
    }

    public virtual bool CheckInstert(Type nodeType, Type parentType)
    {
        return true;
    }

    public virtual bool CheckInstert(GraphNode node, GraphNode parent, int index)
    {
        if (node.IsRoot)
            return false;
        if (parent != null && node.Parent == (GraphNodeRef)parent)
            return true;
        if (parent == null || parent.MaxChildrenCount <= 0 || parent.Children.Count >= parent.NodeData.MaxCount)
            return false;

        return true;
    }

    public virtual bool InsertNodeTo(GraphNode node, GraphNode parent, int index)
    {
        if (!CheckInstert(node, parent, index))
            return false;
        if (node.Parent)
        {
            node.Parent.Node.Children.Remove(node);
        }
        node.Parent = parent;
        parent.Children.Insert(index, node);
        return true;
    }

    public virtual void FreeNode(GraphNode node, Vector2 pos)
    {
        if (node.IsFreeNode)
        {
            if (node.Parent)
            {
                node.Parent.Node.Children.Remove(node);
                node.Parent = GraphNodeRef.Empty;
            }
        }
        node.Bounds.center = pos;
    }

    public virtual bool DeleteNode(GraphNodeRef node)
    {
        if (!node || node.Node.Graph != this || node.Node.IsRoot)
            return false;
        Undo.RegisterCompleteObjectUndo(this, "delete node");
        Nodes.RemoveAll(obj => obj.GUID == node.GUID);
        if (node.Node.Parent)
        {
            node.Node.Parent.Node.Children.Remove(node);
        }
        foreach (var child in node.Node.Children)
        {
            child.Node.Parent = GraphNodeRef.Empty;
        }
        return true;
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

    public virtual void ToCollection<TNode>(NodeCollection<TNode> collection) where TNode : BaseNode
    {

    }
}

