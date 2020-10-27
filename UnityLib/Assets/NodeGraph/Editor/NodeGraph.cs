using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeGraph : ScriptableObject
{
    public List<GraphNode> Nodes = new List<GraphNode>();
    public List<GraphNodeLink> Links = new List<GraphNodeLink>();

    public GraphNode GetNode(string guid)
    {
        return Nodes.Find(obj => obj.GUID == guid);
    }

    public bool HasParent(GraphNode node)
    {
        return Links.Exists(obj => obj.To.Node == node);
    }

    public GraphNodeRef GetParent(GraphNode node)
    {
        var link = Links.Find(obj => obj.To.Node == node);
        if (link != null)
            return link.From;
        return GraphNodeRef.Empty;
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
        if (node.ChildCount > 0)
        {
            int maxCount = node.MaxChildrenCount;
            for (int i = 0; i < Links.Count; ++i)
            {
                var link = Links[i];
                if (link.From.GUID == node.GUID)
                {
                    if (maxCount == 0)
                    {
                        node.ChildCount--;
                        Links.RemoveAt(i);
                    }
                    else
                    {
                        maxCount--;
                    }
                }
            }

        }
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
        if (parent != null && GetParent(node) == (GraphNodeRef)parent)
            return true;
        if (parent == null || parent.MaxChildrenCount <= 0 || parent.ChildCount >= parent.NodeData.MaxCount)
            return false;

        return true;
    }

    public virtual bool InsertNodeTo(GraphNode node, GraphNode parent, int index)
    {
        BreakLinkToParent(node);
        if (!CheckInstert(node, parent, index))
            return false;
        GraphNodeLink insertLink = new GraphNodeLink { From = parent, To = node };
        parent.ChildCount++;
        int insertIndex = Links.Count;
        for (int i=0; i<Links.Count; ++i)
        {
            var link = Links[i];
            if (link.From.GUID == parent.GUID)
            {
                index--;
                if (index == 0)
                {
                    insertIndex = i + 1;
                }
            }
        }
        Links.Insert(insertIndex, insertLink);
        return true;
    }

    public virtual void FreeNode(GraphNode node, Vector2 pos)
    {
        BreakLinkToParent(node);
        node.Bounds.center = pos;
    }

    public virtual bool DeleteNode(GraphNodeRef node)
    {
        if (!node || node.Node.Graph != this || node.Node.IsRoot)
            return false;
        Undo.RegisterCompleteObjectUndo(this, "delete node");
        Nodes.RemoveAll(obj => obj.GUID == node.GUID);
        BreakLinkToParent(node);
        Links.RemoveAll(obj => obj.From == node);
        return true;
    }

    protected bool BreakLinkToParent(GraphNodeRef node)
    {
        var link = Links.Find(obj => obj.To == node);
        if (link != null)
        {
            link.From.Node.ChildCount--;
            Links.Remove(link);
            return true;
        }
        return false;
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

