using UnityEngine;
namespace PlaneEngine
{
    public static class SDFGenUtil
    {

        public static SDFRawData GeneratorByRoot(GameObject root)
        {
            SDFScene scene = new SDFScene();
            scene.AddRoot(root);
            if (!scene.IsValid())
                return null;
            RectBounds bounds = scene.GetBounds();
            bounds.Expand(1, 1);
            bounds.Floor();
            Vector2 size = bounds.Size;
            if (size.x < 1 || size.y < 1)
                return null;
            Vector2 min = bounds.Min;
            int width = Mathf.FloorToInt(size.x);
            int height = Mathf.FloorToInt(size.y);
            float maxDistance = Mathf.Max(size.x, size.y);
            float scale = maxDistance / short.MaxValue;
            short[] data = new short[width * height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    Vector2 pos = min + new Vector2(i, j);
                    float val = scene.SDF(pos); 
                    val = (val / maxDistance)* short.MaxValue;
                    data[i + width * j] = (short)val;
                }
            }
            SDFRawData rawData = new SDFRawData();

            rawData.Init(width, height, scale, min, data);
            return rawData;
        }

    }

}
