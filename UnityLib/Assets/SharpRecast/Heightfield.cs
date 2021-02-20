using System;
using UnityEngine;

namespace SharpRecast
{
    public class Heightfield
    {
        public Bounds Bounds { get; private set; }
        public int Width { get; private set; }
        public int Length { get; private set; }
        public int Height { get; private set; }
        public float CellSize { get; private set; }
        public Cell[] Cells { get; private set; }

        private Heightfield()
        {
        }

        public Heightfield(Bounds b, float size)
        {
            CellSize = size;
            Bounds = b;
            Width = (int)Math.Ceiling((b.max.x - b.min.x) / size);
            Height = (int)Math.Ceiling((b.max.y - b.min.y) / size);
            Length = (int)Math.Ceiling((b.max.z - b.min.z) / size);
            Cells = new Cell[Width * Length];
            for (int i = 0; i < Cells.Length; i++)
                Cells[i] = new Cell(Height);
        }
        public void RasterizeTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            Bounds bbox = RecastMath.GetBounds(a, b, c);
            if (!Bounds.Intersects(bbox))
                return;
            float[] distances = new float[12];

            float invCellSize = 1f / CellSize;
            float invCellHeight = 1f / CellSize;
            float boundHeight = Bounds.max.y - Bounds.min.y;

            int z0 = (int)((bbox.min.z - Bounds.min.z) * invCellSize);
            int z1 = (int)((bbox.max.z - Bounds.min.z) * invCellSize);
            z0 = RecastMath.Clamp(z0, 0, Length - 1);
            z1 = RecastMath.Clamp(z1, 0, Length - 1);
            Vector3[] inVerts = new Vector3[7], outVerts = new Vector3[7], inRowVerts = new Vector3[7];
            for (int z = z0; z <= z1; z++)
            {
                //copy the original vertices to the array.
                inVerts[0] = a;
                inVerts[1] = b;
                inVerts[2] = c;

                //clip the triangle to the row
                int nvrow = 3;
                float cz = Bounds.min.z + z * CellSize;
                nvrow = RecastMath.ClipPolygonToPlane(inVerts, outVerts, distances, nvrow, 0, 1, -cz);
                if (nvrow < 3)
                    continue;
                nvrow = RecastMath.ClipPolygonToPlane(outVerts, inRowVerts, distances, nvrow, 0, -1, cz + CellSize);
                if (nvrow < 3)
                    continue;

                float minX = inRowVerts[0].x, maxX = minX;
                for (int i = 1; i < nvrow; i++)
                {
                    float vx = inRowVerts[i].x;
                    if (minX > vx)
                        minX = vx;
                    if (maxX < vx)
                        maxX = vx;
                }

                int x0 = (int)((minX - Bounds.min.x) * invCellSize);
                int x1 = (int)((maxX - Bounds.min.x) * invCellSize);

                x0 = RecastMath.Clamp(x0, 0, Width - 1);
                x1 = RecastMath.Clamp(x1, 0, Width - 1);

                for (int x = x0; x <= x1; x++)
                {
                    //clip the triangle to the column
                    int nv = nvrow;
                    float cx = Bounds.min.x + x * CellSize;
                    nv = RecastMath.ClipPolygonToPlane(inRowVerts, outVerts, distances, nv, 1, 0, -cx);
                    if (nv < 3)
                        continue;
                    nv = RecastMath.ClipPolygonToPlane(outVerts, inVerts, distances, nv, -1, 0, cx + CellSize);
                    if (nv < 3)
                        continue;

                    //calculate the min/max of the polygon
                    float polyMin = inVerts[0].y, polyMax = polyMin;
                    for (int i = 1; i < nv; i++)
                    {
                        float y = inVerts[i].y;
                        polyMin = Math.Min(polyMin, y);
                        polyMax = Math.Max(polyMax, y);
                    }

                    //normalize span bounds to bottom of heightfield
                    float boundMinY = Bounds.min.y;
                    polyMin -= boundMinY;
                    polyMax -= boundMinY;

                    //if the spans are outside the heightfield, skip.
                    if (polyMax < 0f || polyMin > boundHeight)
                        continue;

                    //clamp the span to the heightfield.
                    if (polyMin < 0)
                        polyMin = 0;
                    if (polyMax > boundHeight)
                        polyMax = boundHeight;

                    //snap to grid
                    int spanMin = (int)(polyMin * invCellHeight);
                    int spanMax = (int)Math.Ceiling(polyMax * invCellHeight);

                    //add the span
                    Cells[z * Width + x].AddSpan(new Span { Min = spanMin, Max = spanMax });
                }
            }
        }

        public void WalkableHeightFilter(float walkableHeight)
        {
            int height = (int)Math.Ceiling(walkableHeight / CellSize);
            foreach (var cell in Cells)
            {
                cell.Combin(height);
            }
        }

        public void UnderGroundFilter(float groundHeight)
        {
            int height = (int)Math.Floor((groundHeight - Bounds.min.y) / CellSize);
            foreach (var cell in Cells)
            {
                for (int i = 0; i < cell.Spans.Count; ++i)
                {
                    if (cell.Spans[i].Max < height)
                    {
                        cell.Spans.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public Heightfield FilterEmptyEdge()
        {
            int startX = 0;
            int startY = 0;
            int endX = Width - 1;
            int endY = Length - 1;
            for (int i = startX; i <= endX; ++i)
            {
                bool isAllEnpty = true;
                for (int j = startY; j <= endY; ++j)
                {
                    var cell = Get(i, j);
                    if (cell != null && cell.Spans.Count > 0)
                    {
                        isAllEnpty = false;
                        break;
                    }
                }
                startX = i;
                if (!isAllEnpty)
                    break;
            }
            for (int i = endX; i > startX; --i)
            {
                bool isAllEnpty = true;
                for (int j = startY; j <= endY; ++j)
                {
                    var cell = Get(i, j);
                    if (cell != null && cell.Spans.Count > 0)
                    {
                        isAllEnpty = false;
                        break;
                    }
                }
                endX = i;
                if (!isAllEnpty)
                    break;
            }
            for (int i = startY; i <= endY; ++i)
            {
                bool isAllEnpty = true;
                for (int j = startX; j <= endX; ++j)
                {
                    var cell = Get(j, i);
                    if (cell != null && cell.Spans.Count > 0)
                    {
                        isAllEnpty = false;
                        break;
                    }
                }
                startY = i;
                if (!isAllEnpty)
                    break;
            }
            for (int i = endY; i > startY; --i)
            {
                bool isAllEnpty = true;
                for (int j = startX; j <= endX; ++j)
                {
                    var cell = Get(j, i);
                    if (cell != null && cell.Spans.Count > 0)
                    {
                        isAllEnpty = false;
                        break;
                    }
                }
                endY = i;
                if (!isAllEnpty)
                    break;
            }
            int newWidth = endX - startX + 1;
            int newLength = endY - startY + 1;
            var newCells = new Cell[newWidth * newLength];
            for (int i = 0; i < newWidth; ++i)
            {
                for (int j = 0; j < newLength; ++j)
                {
                    newCells[i * newWidth + j] = Cells[(i + startX) * Width + (j + startY)];
                }
            }

            Vector3 min = Bounds.min + new Vector3(startX * CellSize, 0, startY * CellSize);
            Vector3 max = Bounds.max - new Vector3((Width - endX + 1) * CellSize, 0, (Length - endY + 1) * CellSize);
            Heightfield heightfield = new Heightfield
            {
                Cells = newCells,
                Width = newWidth,
                Length = newLength,
                CellSize = CellSize,
                Height = Height,
                Bounds = new Bounds(min, max)
            };
            return heightfield;
        }

        private Cell Get(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Length)
                return null;
            return Cells[x + y*Width];
        }

        public float[] ToValidHeight()
        {
            float[] data = new float[Cells.Length];
            float min = Bounds.min.y;
            for (int i = 0; i < Cells.Length; ++i)
            {
                var cell = Cells[i];
                if (cell == null || cell.Spans.Count == 0)
                {
                    data[i] = min;
                }
                else
                {
                    data[i] = cell.Spans[0].Max * CellSize + min;
                }
            }
            return data;
        }
    }
}