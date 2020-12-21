using UnityEngine;

public class GUIRenderImage
{
    public Texture2D Image { get; private set; }
    public Vector4 Border { get; private set; }
    public float BorderRadius { get; private set; }

    public GUIRenderImage(Texture2D img, Vector4 border, float borderRadius)
    {
        Image = img;
        Border = border;
        BorderRadius = borderRadius;
    }

    public GUIRenderImage(Texture2D img, float borderWidth, float borderRadius)
    {
        Image = img;
        Border = new Vector4(borderWidth, borderWidth, borderWidth, borderWidth);
        BorderRadius = borderRadius;
    }

    public void Draw(Rect pos, Color color)
    {
        GUI.DrawTexture(pos, Image, ScaleMode.StretchToFill, true, 0, color, Border, BorderRadius);
    }
}
