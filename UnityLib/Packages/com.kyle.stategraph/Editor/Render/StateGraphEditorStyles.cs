using UnityEditor;
using UnityEngine;

public static class StateGraphEditorStyles
{
    public static readonly Color DefultOutLineColor = new Color32(43, 43, 43, 200);
    public static readonly Color SelectOutLineColor = new Color32(68, 192, 255, 200);

    public static readonly GUIRenderStyle FileNameStyle = new GUIRenderStyle(15, ()=> { return EditorStyles.label; }, (style)=> 
    {
        style.alignment = TextAnchor.LowerRight;
        style.normal.textColor = new Color32(80, 80, 80, 255);
    });

    public static readonly GUIRenderStyle NodeNameStyle = new GUIRenderStyle(12, () => { return EditorStyles.label; }, (style) =>
    {
        style.alignment = TextAnchor.MiddleLeft;
        style.fontStyle = FontStyle.Bold;
        style.wordWrap = true;
    });

    public static readonly GUIRenderStyle TopTipStyle = new GUIRenderStyle(12, () => { return EditorStyles.label; }, (style) =>
    {
        style.alignment = TextAnchor.UpperCenter;
        style.fontStyle = FontStyle.Normal;
        style.normal.textColor = Color.yellow;
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
