using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class StateGraph : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [HideInInspector]
    private ulong IdIndex;
    public int SerializeVersion { get; private set; } = 1;
    public List<StateNode> Nodes = new List<StateNode>();
    public List<StateNodeLink> Links = new List<StateNodeLink>();
    public StateBlackboard Blackboard = new StateBlackboard();

    public StateNode FindNode(ulong id)
    {
        return Nodes.Find(obj => obj.ID == id);
    }

    public StateNode AddNode(IStateNode nodeData, Rect bounds)
    {
        string name = nodeData.GetType().Name;
        DisaplayNameAttribute disaplayName = nodeData.GetType().GetCustomAttribute<DisaplayNameAttribute>();
        if (disaplayName != null && !string.IsNullOrWhiteSpace(disaplayName.Name))
        {
            name = disaplayName.Name;
        }
        StateNode node = new StateNode
        {
            Bounds = bounds,
            ID = ++IdIndex,
            NodeData = nodeData,
            Graph = this,
            Name = name
        };

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

        Links.RemoveAll(it => (it.From == from && it.To == to) || (it.From == to && it.To == from) || (it.To == to && it.IsChild));
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
        for (int i=0; i<Nodes.Count; ++i)
        {
            Nodes[i].Serialize();
        }
    }

    public void Deserialize()
    {
        for (int i = 0; i < Nodes.Count; ++i)
        {
            Nodes[i].Deserialize();
        }
    }

    private void OnEnable()
    {
        Deserialize();
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
        if (!CheckOutput(from) || !ChechInput(to))
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
        return node.Id > 1;
    }

    public virtual bool CheckCopy(StateNodeRef node)
    {
        return node.Id > 1;
    }

    public virtual bool CheckOutput(StateNodeRef node)
    {
        return true;
    }

    public virtual bool ChechInput(StateNodeRef node)
    {
        return node.Id > 1;
    }

    public abstract bool IsStack(StateNode node);

    public abstract bool CheckChildType(StateNode parent, Type childType);

    public abstract bool CheckTypeValid(Type type);

    public abstract bool CheckReplace(Type src, Type dst);

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
