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
            float grain = 0.5f;//精度
            int width = Mathf.FloorToInt(size.x / grain);
            int height = Mathf.FloorToInt(size.y / grain);
            float maxDistance = Mathf.Max(size.x, size.y);
            float scale = maxDistance / 127;
            sbyte[] data = new sbyte[width * height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    Vector2 pos = min + new Vector2(i*grain, j*grain);
                    float val = scene.SDF(pos);
                    val = (val / maxDistance)*127;
                    data[i + width * j] = (sbyte)val;
                }
            }
            SDFRawData rawData = new SDFRawData();

            rawData.Init(width, height, grain, scale, min, data);
            return rawData;
        }


        public static Texture2D ToTexture(SDFRawData sdf)
        {
            Texture2D texture = new Texture2D(sdf.Width, sdf.Height);
            for (int i = 0; i < sdf.Width; ++i)
            {
                for (int j = 0; j < sdf.Height; ++j)
                {
                    float val = sdf[i, j];
                    if (val <= 0)
                    {
                        texture.SetPixel(i, j, new Color(1, 0, 0, -val / 128));
                    }
                    else
                    {
                        texture.SetPixel(i, j, new Color(0, 1, 0, val/128));
                    }
                }
            }
            return texture;
        }
#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/生成SDF图", false, 100)]
        private static void SDFtest()
        {
            GameObject go = UnityEditor.Selection.activeGameObject;
            if (go)
            {
                var data = GeneratorByRoot(go);
                if (data != null)
                {
                    var texture = ToTexture(data);
                    var bytes = texture.EncodeToPNG();
                    string path = "Assets/sdf.png";
                    System.IO.File.WriteAllBytes(path, bytes);
                    UnityEditor.AssetDatabase.ImportAsset(path);
                }
                else
                {
                    Debug.LogError("生成失败");
                }
            }
        }
#endif
    }

}
