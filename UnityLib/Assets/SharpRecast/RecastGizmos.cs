using UnityEngine;

namespace SharpRecast
{
    public static class RecastGizmos
    {
        public static void DrawVoxel(Voxelization voxel)
        {
            Color color = Gizmos.color;
            Gizmos.color = Color.blue;
            Vector3 offset = voxel.Bounds.min;
            for (int x=0; x<voxel.Size.x; ++x)
            {
                for (int y = 0; y < voxel.Size.y; ++y)
                {
                    for (int z = 0; z < voxel.Size.z; ++z)
                    {
                        if (voxel.Voxel.Get(x, y, z))
                        {
                            DrawGrid(x, y, z, voxel.CellSize, offset);
                        }
                    }
                }
            }
            Gizmos.color = color;
        }

        public static void DrawHeightfield(Heightfield heightfield)
        {
            //Color color = Gizmos.color;
            //Gizmos.color = Color.green;
            for (int x=0; x<heightfield.Size.x; ++x)
            {
                for (int z=0; z<heightfield.Size.y; ++z)
                {
                    Cell cell = heightfield.Get(x, z);
                    if (cell == null)
                        continue;
                    for (int i=0; i<cell.Spans.Count; ++i)
                    {
                        var span = cell.Spans[i];
                        Vector3 size = new Vector3(heightfield.CellSize, heightfield.CellSize * (span.Max - span.Min), heightfield.CellSize);
                        Vector3 pos = new Vector3(heightfield.CellSize * x, heightfield.CellSize * span.Min, heightfield.CellSize * z) + heightfield.Offset + size * 0.5f;
                        Gizmos.DrawCube(pos, size);
                    }
                }
            }
            //Gizmos.color = color;
        }

        public static void DrawGridData(RecastGridData gridData)
        {
            for (int x = 0; x < gridData.Width; ++x)
            {
                for (int z = 0; z < gridData.Length; ++z)
                {
                    int index = z * gridData.Width + x;
                    if (gridData.Mask.Get(index))
                    {
                        Vector3 size = new Vector3(1, 0.2f, 1);
                        Vector3 pos = new Vector3(x + 0.5f + gridData.OriginPoint.x, 0, z + 0.5f + gridData.OriginPoint.y);
                        Gizmos.DrawCube(pos, size);
                    }
                }
            }
        }

        public static void DrawGrid(int x, int y, int z, float w, Vector3 offset)
        {
            Vector3 size = Vector3.one * w;
            Vector3 pos = new Vector3(x * w, y * w, z * w) + size*0.5f + offset;
            Gizmos.DrawWireCube(pos, size);
        }
    }


}