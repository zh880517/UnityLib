using UnityEngine;

public class GUIRenderFontStyle
{
    public int FontSize { get; private set; }
    private readonly Font font;
    private GUIStyle style;
    private Color color;
    private bool richText;
    private TextAnchor alignment;

    public GUIRenderFontStyle(int size, Font font, Color color, bool richText = false, TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        FontSize = size;
        this.font = font;
        this.color = color;
        this.richText = richText;
        this.alignment = alignment;
    }

    public GUIStyle Style
    {
        get
        {
            if ( style == null)
            {
                style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = FontSize,
                    richText = richText,
                    alignment = alignment,
                };
                if (font)
                {
                    style.font = font;
                }
                style.normal.textColor = color;
                style.hover.textColor = color;
                style.focused.textColor = color;
            }
            return style;
        }
    }
}
