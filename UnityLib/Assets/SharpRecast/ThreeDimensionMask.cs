using System.Collections;

namespace SharpRecast
{
    public class ThreeDimensionMask
    {
        public int Width { get; private set; }//X
        public int Height { get; private set; }//Y
        public int Length { get; private set; }//Z

        private int yOffset;

        private BitArray data;

        private ThreeDimensionMask() { }

        public ThreeDimensionMask(int w, int h, int l) 
        {
            Width = w;
            Height = h;
            Length = l;
            yOffset = w * l;
            data = new BitArray(Width * Height * Length);
        }

        private int ToIndex(int x, int y, int z)
        {
            return yOffset * y + z * Width + x;
        }

        public bool Get(int x, int y, int z)
        {
            return data.Get(ToIndex(x, y, z));
        }

        public void Set(int x, int y, int z, bool val)
        {
            data.Set(ToIndex(x, y, z), val);
        }

    }

}
