using System;
using UnityEngine;
[Serializable]
public class StateNode: ISerializationCallbackReceiver
{
    public Rect Bounds;
    public string Name;
    public string Comments;//注释
    public ulong ID;
    [SerializeReference]
    public StateGraph Graph;
    [SerializeField]
    private SerializationData serializeData;
    public bool Reversal;//反转，处理输入输出口
    public bool ShowReversal => Reversal && !Parent;
    public int SortIndex;//用来做排序，方便处理渲染顺序
    public StateNodeRef Parent;//可为空，仅为被包含的节点

    public Type NodeType => Data?.GetType();

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
    }
    private StateNodeEditor editor;
    public StateNodeEditor Editor
    {
        get
        {
            if (editor == null)
            {
                editor = new StateNodeEditor(this);
            }
            return editor;
        }
    }

    public void SetData(IStateNode nodeData)
    {
        editor = null;
        this.nodeData = nodeData;
        OnBeforeSerialize();
    }

    public void Deserialize()
    {
        nodeData = TypeSerializerHelper.Deserialize(serializeData) as IStateNode;
        if (nodeData == null)
        {
            Debug.LogErrorFormat("反序列化时数据为空 => {0}", serializeData);
        }
    }

    public void OnBeforeSerialize()
    {
        if (nodeData != null)
            serializeData = TypeSerializerHelper.Serialize(nodeData);
    }

    public void OnAfterDeserialize()
    {
        nodeData = null;
    }

    public static implicit operator StateNodeRef(StateNode exists)
    {
        if (exists == null)
            return StateNodeRef.Empty;
        return StateNodeRef.CreateNodeRef(exists);
    }
}
