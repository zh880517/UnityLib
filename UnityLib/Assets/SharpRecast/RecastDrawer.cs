using UnityEngine;

namespace SharpRecast
{
    public static class RecastDrawer
    {
        public static void DrawHeightfield(Heightfield heightfield)
        {
            Vector3 offset = heightfield.Bounds.min;
            for (int i=0; i<heightfield.Width; ++i)
            {
                for (int j=0; j<heightfield.Height; ++j)
                {
                    int idx = i + j * heightfield.Width;
                    Cell cell = heightfield.Cells[idx];
                    if (cell == null || cell.Spans.Count == 0)
                        continue;
                    Vector3 cellMin = new Vector3(i * heightfield.CellSize, 0, j * heightfield.CellSize) + offset;
                    foreach (var span in cell.Spans)
                    {
                        Vector3 min = cellMin + new Vector3(0, span.Min * heightfield.CellSize, 0);
                        Vector3 size = new Vector3(heightfield.CellSize, (span.Max - span.Min) * heightfield.CellSize, heightfield.CellSize);
                        Gizmos.DrawCube(min + size * 0.5f, size);
                    }
                }
            }
        }
    }


}