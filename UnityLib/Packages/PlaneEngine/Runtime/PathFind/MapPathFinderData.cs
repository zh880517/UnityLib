using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlaneEngine
{
    public class MapPathFinderData
    {
        public Dictionary<int, PathNode> Nodes { get; private set; } = new Dictionary<int, PathNode>();
        //当前寻路数据
        public FTBinaryHeap<PathNode> OpenHeap { get; private set; } = new FTBinaryHeap<PathNode>();
        public List<PathNode> CloseList { get; private set; } = new List<PathNode>();
        public List<PathNode> Path { get; private set; } = new List<PathNode>();
        public float Radius { get; private set; }
//         public bool Find(Vector2 start, Vector2 end, List<Vector2> path)
//         {
// 
//         }
// 
//         public PathNode GetNode(Vector2 worldPos)
//         {
//             var pos = map.WorldPosFloorGridPoint(worldPos);
//             Nodes.TryGetValue(pos.x << 16 | pos.y, out var node);
//             return node;
//         }
// 
//         private Vector2Int PosToIndex(Vector2 pos)
//         {
// 
//         }
    }
}