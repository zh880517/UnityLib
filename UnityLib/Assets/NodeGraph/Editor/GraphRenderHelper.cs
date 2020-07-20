using UnityEditor;
using UnityEngine;

public static class GraphRenderHelper
{
    public static void DrawRect(Rect rect, Color color)
    {
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
    }

    public static void DrawLine(Color color, float lineThickness, params Vector3[] points)
    {
        Color oldColor = Handles.color;
        Handles.color = color;
        Handles.DrawAAPolyLine(lineThickness, points);
        Handles.color = oldColor;
    }

    public static void DrawArrow(Color color, float scale, Vector2 normal, Vector2 endPos)
    {
        Color oldColor = Handles.color;
        Handles.color = color;

        var rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), normal.normalized);
        float arrowSize = 10.0f *scale;
        float arrowWidth = 0.5f *scale;
        var arrowTails = new Vector2[] {
                rotation * new Vector3(1, arrowWidth) * arrowSize,
                rotation * new Vector3(1, -arrowWidth) * arrowSize,
            };

        var p0 = endPos;
        var p1 = endPos + arrowTails[0];
        var p2 = endPos + arrowTails[1];
        Handles.DrawAAConvexPolygon(p0, p1, p2, p0);

        Handles.color = oldColor;
    }
}
