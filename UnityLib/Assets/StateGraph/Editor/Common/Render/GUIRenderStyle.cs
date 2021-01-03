using System;
using UnityEngine;

public class GUIRenderStyle
{
    private GUIStyle style;
    private Func<GUIStyle> copyFrom;
    private Action<GUIStyle> onCreate;
    private int fontSize;

    public GUIRenderStyle(int fontSize, Func<GUIStyle> copy, Action<GUIStyle> setFunc)
    {
        copyFrom = copy;
        onCreate = setFunc;
        this.fontSize = fontSize;
    }

    public GUIStyle GetStyle(float scale)
    {
        if (style == null)
        {
            style = new GUIStyle(copyFrom?.Invoke());
            onCreate?.Invoke(style);
        }
        style.fontSize = Mathf.CeilToInt(fontSize * scale);
        return style;
    }
}
