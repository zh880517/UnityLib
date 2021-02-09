using System.Collections.Generic;
using UnityEngine;

public class BitGridTest : MonoBehaviour
{
    [Header("地图设置")]
    [Range(10, 200)]
    public int Width = 10;
    [Range(10, 200)]
    public int Height = 10;
    [Range(0.1f, 2f)]
    public float Scale = 0.25f;
    [Header("随机生成设置")]
    public int Seed = 50;
    [Range(1, 50)]
    public int MeteoriteRadius = 5;
    [Range(1, 100)]
    public int MeteoriteCount = 20;
    [Header("测试点")]
    public Vector2Int StartPos;
    public Vector2Int EndPos;

    public bool ShowSmooth = true;
    public bool ShowPath = true;
    public bool ShowTestGrid = true;
    public bool ShowLink = true;


    public BitGrid Grid { get; private set; }
    public PathFind Finder { get; private set; }

    public List<Vector2Int> SmoothPoints { get; private set; } = new List<Vector2Int>();

    [ContextMenu("生成")]
    public void Gen()
    {
        Grid = new BitGrid();
        Grid.Init(Width, Height, Scale);
        Grid.MeteoriteFill(Seed, MeteoriteRadius, MeteoriteCount);
        Finder = new PathFind(Grid);
        TestPath();
    }

    public void TestPath()
    {
        if (Grid.Get(StartPos) && Grid.Get(EndPos))
        {
            Finder.FindPath(StartPos, EndPos);
            SmoothPoints.Clear();
            Finder.FloydPath(SmoothPoints);
            Debug.LogFormat("直线联通 {0}", Finder.CheckWalkAble(StartPos, EndPos));
            if (SmoothPoints.Count < 0)
            {
                Debug.LogError("路径平滑计算错误");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Color color = Gizmos.color;
        if (Grid != null)
        {
            Gizmos.color = new Color(0, 0.9f, 1, 0.5f);
            DrawGird();
            if (ShowTestGrid)
            {
                Gizmos.color = new Color(0.9f, 0, 0, 0.5f);
                DrawFindTestGrid();
            }
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ToPos(StartPos), Grid.Scale * 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ToPos(EndPos), Grid.Scale * 0.5f);
            if (Finder.Result.Count > 0)
            {
                if (ShowPath)
                {
                    Gizmos.color = Color.blue;
                    DrawPoints(Finder.Result);
                }
                if (ShowSmooth)
                {
                    Gizmos.color = Color.cyan;
                    DrawPoints(SmoothPoints);
                }
            }
            if (ShowLink)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(ToPos(StartPos), ToPos(EndPos));
            }
        }
        Gizmos.color = color;
    }

    private void DrawPoints(IReadOnlyList<Vector2Int> result)
    {
        for (int i = 1; i < result.Count; ++i)
        {
            Vector2Int point = result[i];
            Vector3 pos = ToPos(point);
            Gizmos.DrawLine(ToPos(result[i - 1]), pos);

            Gizmos.DrawSphere(pos, Grid.Scale * 0.25f);
        }
    }

    private Vector3 ToPos(Vector2Int gridPos)
    {
        return new Vector3((gridPos.x + 0.5f) * Grid.Scale, 0.2f, (gridPos.y + 0.5f) * Grid.Scale);
    }

    private void DrawGird()
    {
        for (int i = 0; i < Grid.Width; ++i)
        {
            for (int j = 0; j < Grid.Height; ++j)
            {
                if (Grid.Get(i, j))
                {
                    Gizmos.DrawCube(new Vector3(Scale * (i + 0.5f), 0.2f, Scale * (j + 0.5f)), new Vector3(Scale, 0.2f, Scale));
                }
            }
        }
    }

    private void DrawFindTestGrid()
    {
        for (int i = 0; i < Grid.Width; ++i)
        {
            for (int j = 0; j < Grid.Height; ++j)
            {
                if (Grid.Get(i, j) && Finder.HasTest(Grid.ToIndex(i, j)))
                {
                    Gizmos.DrawCube(new Vector3(Scale * (i + 0.5f), 0.2f, Scale * (j + 0.5f)), new Vector3(Scale, 0.2f, Scale));
                }
            }
        }
    }
}
