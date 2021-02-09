public class PathNode : IHeapItem<PathNode>
{
    public int HeapIndex { get; set; }

    public int Index { get; private set; }
    public PathNode Parent;

    public int G;
    public int H;

    public int F { get { return G + H; } }

    public PathNode(int idx)
    {
        Index = idx;
    }

    public void Reset()
    {
        Parent = null;
        G = 0;
        H = 0;
    }

    public int CompareTo(PathNode other)
    {
        int compare = F.CompareTo(other.F);
        if (compare == 0)
            compare = H.CompareTo(other.H);
        return -compare;
    }
}
