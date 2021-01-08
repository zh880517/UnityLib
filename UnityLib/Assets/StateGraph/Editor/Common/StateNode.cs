using System;
using UnityEngine;
[Serializable]
public class StateNode
{
    public Rect Bounds;
    public ulong ID;
    [NonSerialized]
    private IStateNode nodeData;
    public IStateNode Data
    {
        get
        {
            if (nodeData == null)
            {
                Deserialize();
            }
            return nodeData;
        }
        set
        {
            nodeData = value;
        }
    }
    [SerializeField]
    private SerializationData serializeData;
    public int SortIndex;//用来做排序，方便处理渲染顺序
    public StateNodeRef Parent;//可为空，仅为被包含的节点
    public string Name;
    public string Comments;//注释
    [SerializeReference]
    public StateGraph Graph;

    public Type NodeType => Data?.GetType();

    public void Serialize()
    {
        if (nodeData != null)
            serializeData = TypeSerializerHelper.Serialize(nodeData);
    }

    public void Deserialize()
    {
        nodeData = TypeSerializerHelper.Deserialize(serializeData) as IStateNode;
        if (Data == null)
        {
            Debug.LogError("数据为空");
        }
    }

    public static implicit operator StateNodeRef(StateNode exists)
    {
        if (exists == null)
            return StateNodeRef.Empty;
        return StateNodeRef.CreateNodeRef(exists);
    }
}
