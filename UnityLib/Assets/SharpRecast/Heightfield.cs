using UnityEngine;

namespace SharpRecast
{
    public class Heightfield
    {
        public Vector3 Offset { get; private set; }
        public Vector2Int Size { get; private set; }
        public float CellSize { get; private set; }
        //可为null
        public Cell[] Cells { get; private set; }

        public Heightfield(Vector3 offset, int width, int length, float size)
        {
            Offset = offset;
            Size = new Vector2Int(width, length);
            CellSize = size;
            Cells = new Cell[width * length];
        }

        public Cell Get(int x, int y)
        {
            return Cells[y * Size.x + x];
        }
    }
}