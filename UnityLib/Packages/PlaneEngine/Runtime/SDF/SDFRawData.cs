using System.IO;
using UnityEngine;

namespace PlaneEngine
{
    public class SDFRawData
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Grain { get; private set; }
        public float Scale { get; private set; }
        public Vector2 Origin { get; private set; }
        private sbyte[] data;
        public sbyte this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return sbyte.MinValue;
                return data[x + y * Width];
            }
        }

        public float this[int idx]
        {
            get
            {
                if (idx < 0 || idx >= data.Length)
                    return sbyte.MinValue * Scale;
                return data[idx] * Scale;
            }
        }

        public float Sample(Vector2 pos)
        {
            pos /= Grain;
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int idx = x + y * Width;
            float rx = pos.x - x;
            float ry = pos.y - y;
            //2 3
            //0 1
            float v0 = this[idx];
            float v1 = this[idx + 1];
            float v2 = this[idx + Width];
            float v3 = this[idx + Width + 1];

            return (v0 * (1 - rx) + v1 * rx) * (1 - ry) + (v2 * (1 - rx) + v3 * rx) * ry;
        }

        public float Get(Vector2Int pt)
        {
            return this[pt.x + pt.y * Width];
        }

        public void Init(int width, int heigh, float grain, float scale, Vector2 origin, sbyte[] data)
        {
            Width = width;
            Height = heigh;
            Grain = grain;
            Scale = scale;
            Origin = origin;
            this.data = new sbyte[Width * heigh];
            data.CopyTo(this.data, 0);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Grain);
            writer.Write(Scale);
            writer.Write(Origin.x);
            writer.Write(Origin.y);
            writer.Write(data.Length);
            for (int i = 0; i < data.Length; ++i)
            {
                writer.Write(data[i]);
            }
        }

        public void Read(BinaryReader reader)
        {
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Grain = reader.ReadSingle();
            Scale = reader.ReadSingle();
            Origin = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            int len = reader.ReadInt32();
            data = new sbyte[len];
            for (int i = 0; i < len; ++i)
            {
                data[i] = reader.ReadSByte();
            }
        }
    }
}
