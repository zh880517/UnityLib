using System.Collections.Generic;
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
            SDFRawData rawData = new SDFRawData();
            RectBounds bounds = scene.GetBounds();

            return rawData;
        }


        public static Texture2D ToTexture(SDFRawData sdf)
        {
            Texture2D texture = new Texture2D(sdf.Width, sdf.Heigh);
            for (int i = 0; i < sdf.Width; ++i)
            {
                for (int j = 0; j < sdf.Heigh; ++j)
                {
                    sbyte val = sdf[i, j];
                    //val = ;
                    texture.SetPixel(i, j, Color.white * val);
                }
            }
            return texture;
        }
    }

}
