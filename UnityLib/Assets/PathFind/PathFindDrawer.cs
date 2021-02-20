using System.Collections.Generic;
using UnityEngine;

public static class PathFindDrawer
{
    public static void DrawGrid(BitGrid grid)
    {
        Vector3 size = new Vector3(grid.Scale, 0.2f, grid.Scale);
        for (int i = 0; i < grid.Width; ++i)
        {
            for (int j = 0; j < grid.Height; ++j)
            {
                if (grid.Get(i, j))
                {
                    Gizmos.DrawCube(new Vector3(grid.Scale * (i + 0.5f), 0.2f, grid.Scale * (j + 0.5f)), size);
                }
            }
        }
    }

    public static void DrawFindTestGrid(PathFind finder)
    {
        float scale = finder.Grid.Scale;
        Vector3 size = new Vector3(scale, 0.2f, scale);
        for (int i = 0; i < finder.Grid.Width; ++i)
        {
            for (int j = 0; j < finder.Grid.Height; ++j)
            {
                if (finder.Grid.Get(i, j) && finder.HasTest(finder.Grid.ToIndex(i, j)))
                {
                    Gizmos.DrawCube(new Vector3(scale * (i + 0.5f), 0.2f, scale * (j + 0.5f)), size);
                }
            }
        }
    }

    public static void DrawPathPoint(IReadOnlyList<Vector2Int> result, float gridScale, Vector3 girdPos)
    {
        for (int i = 1; i < result.Count; ++i)
        {
            Vector2Int point = result[i];
            Vector3 pos = ToPos(point, gridScale, girdPos);
            Gizmos.DrawLine(ToPos(result[i - 1], gridScale, girdPos), pos);

            Gizmos.DrawSphere(pos, gridScale * 0.25f);
        }
    }

    public static Vector3 ToPos(Vector2Int gridPos, float scale, Vector3 offset)
    {
        return new Vector3((gridPos.x + 0.5f) * scale, 0.2f, (gridPos.y + 0.5f) * scale) + offset;
    }
}
