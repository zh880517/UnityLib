using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class StateGraph : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [HideInInspector]
    private ulong IdIndex;
    [HideInInspector]
    public int GroupId;
    public int SerializeVersion { get; private set; } = 1;
    [HideInInspector]
    public List<StateNode> Nodes = new List<StateNode>();
    [HideInInspector]
    public List<StateNodeLink> Links = new List<StateNodeLink>();
    [HideInInspector]
    public StateBlackboard Blackboard = new StateBlackboard();
    [DisplayName("描述"), MultiLineText]
    public string Commit;

    public StateNode FindNode(ulong id)
    {
        return Nodes.Find(obj => obj.ID == id);
    }

    public StateNode AddNode(IStateNode nodeData, Rect bounds)
    {
        string name = nodeData.GetType().Name;
        DisplayNameAttribute disaplayName = nodeData.GetType().GetCustomAttribute<DisplayNameAttribute>();
        if (disaplayName != null && !string.IsNullOrWhiteSpace(disaplayName.Name))
        {
            name = disaplayName.Name;
        }
        StateNode node = new StateNode
        {
            Bounds = bounds,
            ID = ++IdIndex,
            Graph = this,
            Name = name
        };
        node.SetData(nodeData);
        Nodes.Add(node);
        return node;
    }

    public void AddLink(StateNodeRef from, StateNodeRef to, bool isChild)
    {
        if (!from || !to || from == to)
            return;
        if (!IsStack(from.Node))
        {
            Links.RemoveAll(it => it.From == from);
        }
        if (isChild)
        {
            Links.RemoveAll(it => it.To == to);
        }
        else
        {
            Links.RemoveAll(it => (it.From == from && !it.IsChild) || (it.From == to && it.To == from) || (it.To == to && it.IsChild));
        }
        Links.Add(new StateNodeLink { From = from, To = to, IsChild = isChild });
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
    }

    public virtual bool CheckLink(StateNode from, StateNode to, bool isChild)
    {
        if (isChild && (from.Parent || IsStack(to)))
            return false;
        if (!ChechInput(to))
            return false;
        if (isChild && !IsStack(from))
            return false;
        if (!CheckOutput(from) && !isChild)
            return false;
        if (isChild && !CheckChildType(from, to.NodeType))
            return false;
        if (from == to || Links.Exists(obj=>obj.From == from && obj.To == to && obj.IsChild == isChild))
        {
            return false;
        }
        return true;
    }

    public virtual bool CheckDelete(StateNodeRef node)
    {
        return !typeof(INoDelete).IsAssignableFrom(node.Node.NodeType);
    }

    public virtual bool CheckCopy(StateNodeRef node)
    {
        return !typeof(INoCopy).IsAssignableFrom(node.Node.NodeType);
    }

    public virtual bool CheckOutput(StateNodeRef node)
    {
        return typeof(IOutputNode).IsAssignableFrom(node.Node.NodeType);
    }

    public virtual bool ChechInput(StateNodeRef node)
    {
        return typeof(IInputNode).IsAssignableFrom(node.Node.NodeType);
    }

    public virtual bool IsStack(StateNode node)
    {
        return typeof(IStackNode).IsAssignableFrom(node.NodeType);
    }

    public abstract bool CheckChildType(StateNode parent, Type childType);

    public abstract bool CheckTypeValid(Type type);

    public virtual bool CheckReplace(Type src, Type dst)
    {
        if (src == dst)
            return true;
        if (src.BaseType == dst.BaseType || src.BaseType == dst || src == dst.BaseType)
            return true;

        return false;
    }

    protected abstract void OnCreat();

#if UNITY_EDITOR
    public static T LoadOrCreat<T>(string path) where T : StateGraph
    {
        var graph = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        if (graph == null)
        {
            if (System.IO.File.Exists(path))
            {
                Debug.LogErrorFormat("打开文件 {0} 失败，已经存在的文件类型和目标类型 {1} 不匹配", path, typeof(T).FullName);
                return null;
            }
            string dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            graph = CreateInstance<T>();
            graph.OnCreat();
            UnityEditor.AssetDatabase.CreateAsset(graph, path);
        }

        return graph;
    }
#endif
}
