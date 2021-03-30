using UnityEngine;
namespace SharpRecast
{
    public class Voxelization
    {
        public ThreeDimensionMask Voxel { get; private set; }
        public Bounds Bounds { get; private set; }
        public Vector3Int Size { get; private set; }
        public float CellSize { get; private set; }

        public Voxelization(Bounds b, float size)
        {
            CellSize = size;
            Bounds = b;
            Size = Vector3Int.CeilToInt(b.size / size);
            Voxel = new ThreeDimensionMask(Size.x, Size.y, Size.z);
        }

        public void VoxelTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 boundsMin = Bounds.min;
            Vector3 boundsMax = Bounds.max;
            RecastMath.GetMinMax(a, b, c, out Vector3 triMin, out Vector3 triMax);
            //计算重叠AABB
            Vector3 min = triMin/* - boundsMin*/;
            Vector3 max = triMax/* - Bounds.min*/;
            min.x = Mathf.Max(min.x, boundsMin.x);
            min.y = Mathf.Max(min.y, boundsMin.y);
            min.z = Mathf.Max(min.z, boundsMin.z);
            max.x = Mathf.Min(max.x, boundsMax.x);
            max.y = Mathf.Min(max.y, boundsMax.y);
            max.z = Mathf.Min(max.z, boundsMax.z);
            Vector3 size = max - min;
            //无重叠则不处理
            if (size.x < 0 || size.y < 0 || size.z < 0)
                return;

            float invCellSize = 1f / CellSize;

            float half = CellSize * 0.5f;
            float sqrHalf = half * half;
            Vector3 halfSize = new Vector3(half, half, half);
            Vector3Int start = Vector3Int.FloorToInt((min - boundsMin) * invCellSize);
            Vector3Int end = Vector3Int.CeilToInt((max - boundsMin) * invCellSize);
            if (start.x == Size.x)
                start.x--;
            if (start.y == Size.y)
                start.y--;
            if (start.z == Size.z)
                start.z--;
            if (end.y == start.y)
                end.y++;
            if (end.x == start.x)
                end.x++;
            if (end.z == start.z)
                end.z++;
            for (int x = start.x; x < end.x; ++x)
            {
                for (int z = start.z; z < end.z; ++z)
                {
                    for (int y = start.y; y < end.y; ++y)
                    {
                        if (Voxel.Get(x, y, z))
                            continue;
                        Vector3 center = new Vector3(x * CellSize, y * CellSize, z * CellSize) + boundsMin + halfSize;
                        if (RecastMath.SqrDistancePointTriangle(a, b, c, center) <= sqrHalf)
                        {
                            Voxel.Set(x, y, z, true);
                        }
                    }
                }
            }
        }

        //范围判断版本检测，Voxel.Get 接口不带检测的，有越界风险
        public bool Check(int x, int y, int z)
        {
            if (x >= 0 && x < Size.x && y >= 0 && y < Size.y && z>=0 &&z<Size.z)
            {
                return Voxel.Get(x, y, z);
            }
            return false;
        }
    }

}
