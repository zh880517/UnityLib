using System;
using UnityEngine;
[Serializable]
public class StateNode
{
    public Rect Bounds;
    [HideInInspector]
    public ulong ID;
    public SerializationData data;
    public IStateNode NodeData { get; set; }
    [HideInInspector]
    [SerializeReference]
    public StateGraph Graph;
    public int SortIndex;//用来做排序，方便处理渲染顺序
    public StateNodeRef Parent;//可为空，仅为被包含的节点
    public string Name;
    public string Comments;//注释

    public Type NodeType => NodeData?.GetType();

    public static implicit operator StateNodeRef(StateNode exists)
    {
        if (exists == null)
            return StateNodeRef.Empty;
        return StateNodeRef.CreateNodeRef(exists);
    }
}
