using System.Collections;
using UnityEngine;

public class BitGrid
{
    public BitArray Data { get; private set; }
    public float Scale { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int ValidCount { get; private set; }

    public void Init(int width, int height, float scale)
    {
        Data = new BitArray(width * height);
        Scale = scale;
        Width = width;
        Height = height;
    }

    private void UpdateCount()
    {
        ValidCount = 0;
        for (int i=0; i<Data.Count; ++i)
        {
            if (Data[i])
                ++ValidCount;
        }
    }

    public int ToIndex(int x, int y)
    {
        return x + y * Width;
    }

    public int ToIndex(Vector2Int pos)
    {
        return pos.x + pos.y * Width;
    }

    public Vector2Int ToPos(int idx)
    {
        return new Vector2Int(idx % Width, idx / Width);
    }

    public int GetX(int idx)
    {
        return idx % Width;
    }

    public int GetY(int idx)
    {
        return idx / Width;
    }

    public bool Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return false;
        return Data.Get(x + y * Width);
    }

    public bool Get(Vector2Int pos)
    {
        return Get(pos.x, pos.y);
    }

    //陨石填充，随机选择一块区域填充
    public void MeteoriteFill(int seed, int radius, int amount)
    {
        if (Data == null)
            return;
        Data.SetAll(false);
        System.Random random = new System.Random(seed);
        for (int i=0; i<amount; ++i)
        {
            int x = random.Next(0, Width);
            int y = random.Next(0, Height);
            int minX = System.Math.Max(0, x - radius);
            int maxX = System.Math.Min(Width, x + radius);
            int minY = System.Math.Max(0, y - radius);
            int maxY = System.Math.Min(Height, y + radius);
            for (int posX = minX; posX<maxX; ++posX)
            {
                for (int posY = minY; posY < maxY; ++posY)
                {
                    int distance = (posX - x) * (posX - x) + (posY - y) * (posY - y);
                    if (distance <= radius*radius)
                    {
                        Data.Set(posX + posY * Width, true);
                    }
                }
            }
        }
        UpdateCount();
    }

}
