using PlaneEngine;
using UnityEditor;
using UnityEngine;

public class SDFEditorTool
{

    public static Texture2D ToGrayTexture(SDFRawData sdf)
    {
        Texture2D texture = new Texture2D(sdf.Width, sdf.Height);
        for (int i = 0; i < sdf.Width; ++i)
        {
            for (int j = 0; j < sdf.Height; ++j)
            {
                float val = ((float)sdf[i, j] - short.MinValue) / 65536;
                Color color = new Color(val, val, val, 1);
                texture.SetPixel(i, j, color);
            }
        }
        return texture;
    }

    [MenuItem("GameObject/生成SDF图", false, 100)]
    private static void SDFtest()
    {
        GameObject go = Selection.activeGameObject;
        if (go)
        {
            var data = SDFGenUtil.GeneratorByRoot(go);
            if (data != null)
            {
                var texture = ToGrayTexture(data);
                var bytes = texture.EncodeToPNG();
                string path = "Assets/sdf.png";
                System.IO.File.WriteAllBytes(path, bytes);
                AssetDatabase.ImportAsset(path);
            }
            else
            {
                Debug.LogError("生成失败");
            }
        }
    }
}
