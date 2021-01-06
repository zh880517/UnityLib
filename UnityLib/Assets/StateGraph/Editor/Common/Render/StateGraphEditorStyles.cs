using UnityEditor;
using UnityEngine;

public static class StateGraphEditorStyles
{
    public static readonly GUIRenderStyle FileNameStyle = new GUIRenderStyle(15, ()=> { return EditorStyles.label; }, (style)=> 
    {
        style.alignment = TextAnchor.LowerRight;
        style.normal.textColor = new Color32(80, 80, 80, 255);
    });

    public static readonly GUIRenderStyle NodeNameStyle = new GUIRenderStyle(12, () => { return EditorStyles.label; }, (style) =>
    {
        style.alignment = TextAnchor.MiddleLeft;
        style.fontStyle = FontStyle.Bold;
    });

    public static readonly GUIRenderStyle TxtButtonStyle = new GUIRenderStyle(15, ()=> { return EditorStyles.label; }, (style)=> 
    {
        style.hover.textColor = Color.yellow;
        style.clipping = TextClipping.Overflow;
        style.alignment = TextAnchor.MiddleCenter;
        style.margin = new RectOffset();
        style.padding = new RectOffset();
    });
}
