using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    [System.Serializable]
    public class FlowSubGraph
    {
        public FlowGraph Owner;
        public string GUID;
        public string Name;
        public bool AllowStageNode;
        public Vector3 Position;
        public float Scale;
        public List<FlowNodeRef> Nodes = new List<FlowNodeRef>();
    }
}