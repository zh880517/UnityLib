using System.IO;
using UnityEditor;
using UnityEngine;

namespace MeshPainter
{
    public static class MeshPaintUtil
    {
        public static void DrawPoint(Vector3 point, Vector3 normal, float size)
        {
            Color oldColor = Handles.color;
            Handles.color = new Color(1f, 1f, 0f, 1f);
            Handles.DrawWireDisc(point, normal, size);
            Handles.color = new Color(1f, 0f, 0f, 1f);
            Handles.DrawLine(point, point + normal);
            Handles.color = oldColor;
        }

        public static Vector2 FormateUV(Vector3 uv)
        {
            float changX;
            float changY;
            if (Mathf.Abs(uv.x) > 1 || Mathf.Abs(uv.y) > 1)
            {
                if (uv.x > 1)
                    changX = uv.x % 1;
                else if (uv.x < -1)
                    changX = 1 - Mathf.Abs(uv.x % (-1));
                else
                    changX = uv.x;
                if (uv.y > 1)
                    changY = uv.y % 1;
                else if (uv.y < -1)
                    changY = 1 - Mathf.Abs(uv.y % (-1));
                else
                    changY = uv.y;
                uv = new Vector2(changX, changY);
            }
            if ((uv.y < 0 && uv.y >= -1) || (uv.x < 0 && uv.x >= -1))
            {
                if (uv.x < 0 && uv.x >= -1)
                    changX = 1 - Mathf.Abs(uv.x);
                else
                    changX = uv.x;
                if (uv.y < 0 && uv.y >= -1)
                    changY = 1 - Mathf.Abs(uv.y);
                else 
                    changY = uv.y;
                uv = new Vector2(changX, changY);
            }
            return uv;
        }

        public static void PaintMaskTexture(Texture2D targetTexture, Vector2 pixelUV, int channel, Texture2D brush, float brushSize, float brushOrthValue, float brushStronger)
        {
            //设置控制通道
            Color targetColor = new Color(1f, 0f, 0f, 0f);
            switch (channel)
            {
                case 0:
                    targetColor = new Color(1f, 0f, 0f, 0f);
                    break;
                case 1:
                    targetColor = new Color(0f, 1f, 0f, 0f);
                    break;
                case 2:
                    targetColor = new Color(0f, 0f, 1f, 0f);
                    break;
                case 3:
                    targetColor = new Color(0f, 0f, 0f, 1f);
                    break;
                default:
                    Debug.LogErrorFormat("控制通道选择错误，只能选择0-3，当前选择 {0}", channel);
                    return;
            }

            pixelUV = FormateUV(pixelUV);
            int brushSizeInPourcent = (int)Mathf.Round((brushSize * targetTexture.width) / (brushOrthValue * 0.5f * 100));

            //Calculate the area covered by the brush
            int puX = Mathf.FloorToInt(pixelUV.x * targetTexture.width);
            int puY = Mathf.FloorToInt(pixelUV.y * targetTexture.height);
            int x = Mathf.Clamp(puX - brushSizeInPourcent / 2, 0, targetTexture.width - 1);
            int y = Mathf.Clamp(puY - brushSizeInPourcent / 2, 0, targetTexture.height - 1);
            int width = Mathf.Clamp((puX + brushSizeInPourcent / 2), 0, targetTexture.width) - x;
            int height = Mathf.Clamp((puY + brushSizeInPourcent / 2), 0, targetTexture.height) - y;
            Color[] terrainBay = targetTexture.GetPixels(x, y, width, height, 0);
            float[] brushAlpha = new float[brushSizeInPourcent * brushSizeInPourcent];
            for (int i = 0; i < brushSizeInPourcent; i++)
            {
                for (int j = 0; j < brushSizeInPourcent; j++)
                {
                    brushAlpha[j * brushSizeInPourcent + i] = brush.GetPixelBilinear(((float)i) / brushSizeInPourcent, ((float)j) / brushSizeInPourcent).a;
                }
            }
            //Calculate the color after drawing
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int index = (i * width) + j;
                    float Stronger = brushAlpha[Mathf.Clamp((y + i) - (puY - brushSizeInPourcent / 2), 0, brushSizeInPourcent - 1) * brushSizeInPourcent + Mathf.Clamp((x + j) - (puX - brushSizeInPourcent / 2), 0, brushSizeInPourcent - 1)] * brushStronger;

                    terrainBay[index] = Color.Lerp(terrainBay[index], targetColor, Stronger);
                }
            }

            Undo.RegisterCompleteObjectUndo(targetTexture, "meshPaint");
            //刷新控制贴图
            targetTexture.SetPixels(x, y, width, height, terrainBay, 0);
            targetTexture.Apply();
        }

        public static void SaveTexture(Texture2D targetTexture)
        {
            var path = AssetDatabase.GetAssetPath(targetTexture);
            byte[] bytes = null;
            string externName = path.Substring(path.LastIndexOf('.') + 1).ToLower();
            if (externName == "png")
            {
                bytes = targetTexture.EncodeToPNG();
            }
            else if (externName == "tga")
            {
                bytes = targetTexture.EncodeToTGA();
            }
            else if (externName == "jpg")
            {
                bytes = targetTexture.EncodeToJPG();
            }
            if (bytes != null)
            {
                File.WriteAllBytes(path, bytes);
            }
            else
            {
                Debug.LogErrorFormat("贴图格式不支持， 保存失败：{0}", path);
            }
        }
    }

}