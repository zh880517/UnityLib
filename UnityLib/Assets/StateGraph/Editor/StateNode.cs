using System;
using UnityEngine;
[Serializable]
public class StateNode : ISerializationCallbackReceiver 
{
    [HideInInspector]
    public Rect Bounds;
    [HideInInspector]
    public ulong ID;
    [SerializeField]
    [HideInInspector]
    private SerializationData data;
    public IStateNode NodeData;
    [HideInInspector]
    public StateGraph Graph;
    public int SortIndex;//用来做排序，方便处理渲染顺序
    public StateNodeRef Parent;//可为空，仅为被包含的节点

    public static implicit operator StateNodeRef(StateNode exists)
    {
        if (exists == null)
            return StateNodeRef.Empty;
        return StateNodeRef.CreateNodeRef(exists.Graph, exists.ID);
    }

    public void OnAfterDeserialize()
    {
        NodeData = TypeSerializerHelper.Deserialize(data) as IStateNode;
    }

    public void OnBeforeSerialize()
    {
        data = TypeSerializerHelper.Serialize(NodeData);
    }
}
