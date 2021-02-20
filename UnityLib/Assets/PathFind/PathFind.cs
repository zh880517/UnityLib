using System.Collections.Generic;
using UnityEngine;

public class PathFind
{
    //1 2 3
    //8   4
    //7 6 5
    public static readonly Vector2Int[] SurroundOffSet = new Vector2Int[]
    {
        new Vector2Int(-1, 1),//↖ 
        new Vector2Int(0, 1),//↑
        new Vector2Int(1, 1),//↗
        new Vector2Int(1, 0),//→
        new Vector2Int(1, -1),//↘
        new Vector2Int(0, -1),//↓
        new Vector2Int(-1, -1),//↙
        new Vector2Int(-1, 0),//←
    };
    public const int OBLIQUE = 14;
    public const int STEP = 10;
    public static readonly int[] SurroundCost = new int[]
    {
        OBLIQUE,
        STEP,
        OBLIQUE,
        STEP,
        OBLIQUE,
        STEP,
        OBLIQUE,
        STEP,
    };

    private readonly HashSet<int> closeSet = new HashSet<int>();
    private readonly Heap<PathNode> openList = new Heap<PathNode>();
    private readonly Dictionary<int, PathNode> nodeCache = new Dictionary<int, PathNode>();
    private readonly List<Vector2Int> findResult = new List<Vector2Int>();
    public  BitGrid Grid { get; private set; }

    public IReadOnlyList<Vector2Int> Result { get { return findResult.AsReadOnly(); } }
    
    public PathFind(BitGrid grid)
    {
        Grid = grid;
    }
    
    public bool FindPath(Vector2Int start, Vector2Int end)
    {
        Reset();
        //如果直接可以通过就不做寻路
        if (CheckWalkAble(start, end))
        {
            findResult.Add(end);
            findResult.Add(start);
            return true;
        }
        if (start == end)
        {
            return true; ;
        }
        PathNode endNode = GetNode(end.x, end.y);
        PathNode startNode = GetNode(start.x, start.y);
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            var currentNode = openList.RemoveFirst();
            closeSet.Add(currentNode.Index);
            if (endNode == currentNode)
            {
                break;
            }
            Vector2Int currPos = new Vector2Int(Grid.GetX(currentNode.Index), Grid.GetY(currentNode.Index));
            for (int i=0; i<SurroundOffSet.Length; ++i)
            {
                var offset = SurroundOffSet[i];
                Vector2Int pos = offset + currPos;
                if (!Grid.Get(pos.x, pos.y))
                    continue;
                var neighbour = GetNode(pos.x, pos.y);
                if (closeSet.Contains(neighbour.Index))
                    continue;
                int cost = SurroundCost[i];
                int G = currentNode.G + cost;
                if (openList.Contains(neighbour))
                {
                    if (G < neighbour.G)
                    {
                        neighbour.Parent = currentNode;
                        neighbour.G = G;
                    }
                }
                else
                {
                    neighbour.Parent = currentNode;
                    neighbour.G = G;
                    neighbour.H = CalcH(pos, end);
                    openList.Add(neighbour);
                }
            }
        }
        if (endNode.Parent != null)
        {
            findResult.Add(end);
            var prePos = Grid.ToPos(endNode.Parent.Index);
            findResult.Add(prePos);
            Vector2Int normal = prePos - end;
            var preNode = endNode.Parent.Parent;
            //平滑路径处理
            while (preNode != null)
            {
                Vector2Int curPos = Grid.ToPos(preNode.Index);
                Vector2Int newNormal = curPos - prePos;
                if (newNormal == normal)
                {
                    findResult[findResult.Count - 1] = curPos;
                }
                else
                {
                    findResult.Add(curPos);
                    normal = newNormal;
                }
                prePos = curPos;
                preNode = preNode.Parent;
            }
            findResult.Reverse();
            return true;
        }
        return false;
    }

    public bool HasTest(int index)
    {
        if (nodeCache.TryGetValue(index, out var node))
        {
            return node.Parent != null;
        }
        return false;
    }

    //路径平滑
    public void FloydPath(List<Vector2Int> path)
    {
        for (int i=0; i<findResult.Count; ++i)
        {
            Vector2Int last = findResult[i];
            path.Add(last);
            for (int j=path.Count - 2; j>=0; --j)
            {
                int len = path.Count - 1 - j - 1;
                if (len > 0 && CheckWalkAble(last, path[j]))
                {
                    path.RemoveRange(j + 1, len);
                }
            }
        }
    }

    public bool CheckWalkAble(Vector2Int from, Vector2Int to)
    {
        if (from == to)
            return true;
        Vector2Int offset = to - from;
        if (offset.x == 0)
        {
            int absY = System.Math.Abs(offset.y);
            int increY = offset.y / absY;
            for (int i=1; i< absY; ++i)
            {
                if (!Grid.Get(from.x, from.y + increY*i))
                    return false;
            }
        }
        else if (offset.y == 0)
        {
            int absX = System.Math.Abs(offset.x);
            int increX = offset.x / absX;
            for (int i = 1; i < absX; ++i)
            {
                if (!Grid.Get(from.x + increX*i, from.y))
                    return false;
            }
        }
        else
        {
            //判断直线相交的格子是否可行走
            int absX = System.Math.Abs(offset.x);
            int absY = System.Math.Abs(offset.y);
            int increX = offset.x / absX;
            int increY = offset.y / absY;
            //直线方程, 斜截式
            float k = offset.y / (float)offset.x;
            float b = -k * from.x + from.y;
            int minY = System.Math.Min(from.y, to.y);
            int maxY = System.Math.Max(from.y, to.y);
            for (int i=1; i<absX; ++i)
            {
                int x = from.x + increX * i;
                int y = Mathf.CeilToInt(k * x + b);
                if (!Grid.Get(x, y))
                    return false;
                int offsetY = y + increY;
                if (offsetY <= maxY && offsetY >= minY && !Grid.Get(x, offsetY))
                    return false;
            }
            int minX = System.Math.Min(from.x, to.x);
            int maxX = System.Math.Max(from.x, to.x);
            for (int i = 1; i < absY; ++i)
            {
                int y = from.y + increY * i;
                int x = Mathf.CeilToInt((y-b)/k);
                if (!Grid.Get(x, y))
                    return false;
                int offsetX = x + increX;
                if (offsetX <= maxX && offsetX >= minX && !Grid.Get(offsetX, y))
                    return false;
            }

        }
        return true;
    }

    private int CalcH(Vector2Int from, Vector2Int to)
    {
        int x = System.Math.Abs(from.x - to.x);
        int y = System.Math.Abs(from.y - to.y);
        return STEP * (x + y) + (OBLIQUE - 2 * STEP) * System.Math.Min(x, y);//快速曼哈顿算法
        //return (int)Vector2Int.Distance(from, to)*10;//比较精确的距离计算，慢速
    }

    private void Reset()
    {
        openList.Reset(Grid.ValidCount);
        closeSet.Clear();
        findResult.Clear();
        foreach (var kv in nodeCache)
        {
            kv.Value.Reset();
        }
    }

    private PathNode GetNode(int x, int y)
    {
        return GetNode(Grid.ToIndex(x, y));
    }

    private PathNode GetNode(int idx)
    {
        if (!nodeCache.TryGetValue(idx, out var node))
        {
            node = new PathNode(idx);
            nodeCache.Add(idx, node);
        }

        return node;
    }
}
