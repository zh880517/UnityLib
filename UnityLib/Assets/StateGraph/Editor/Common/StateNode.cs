using System;
using UnityEngine;
[Serializable]
public class StateNode
{
    public Rect Bounds;
    public ulong ID;
    public SerializationData data;
    [NonSerialized]
    public IStateNode NodeData;
    public SerializationData SaveData;
    [SerializeReference]
    public StateGraph Graph;
    public int SortIndex;//用来做排序，方便处理渲染顺序
    public StateNodeRef Parent;//可为空，仅为被包含的节点
    public string Name;
    public string Comments;//注释

    public Type NodeType => NodeData?.GetType();

    public void Serialize()
    {
        SaveData = TypeSerializerHelper.Serialize(NodeData);
    }

    public void Deserialize()
    {
        NodeData = TypeSerializerHelper.Deserialize(SaveData) as IStateNode;
    }

    public static implicit operator StateNodeRef(StateNode exists)
    {
        if (exists == null)
            return StateNodeRef.Empty;
        return StateNodeRef.CreateNodeRef(exists);
    }
}
