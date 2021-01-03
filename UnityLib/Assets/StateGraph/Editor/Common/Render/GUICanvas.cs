using UnityEditor;
using UnityEngine;
[System.Serializable]
public class GUICanvas
{
    const float maxAllowedZoom = 6.0f;
    [SerializeField]
    Vector2 position = Vector2.zero;
    public Vector2 Position
    {
        get
        {
            return position;
        }
    }
    [SerializeField]
    float zoomLevel = 1;
    public float ZoomLevel
    {
        get
        {
            return zoomLevel;
        }
    }
    [SerializeField]
    private float scale = 1;
    public float Scale
    {
        get
        {
            return scale;
        }
    }

    private const float BOARD_RADIUS = 8;
    private static readonly Vector4 TopBoardRadius = new Vector4(BOARD_RADIUS, BOARD_RADIUS, 0, 0);
    private static readonly Vector4 BottomBoardRadius = new Vector4(0, 0, BOARD_RADIUS, BOARD_RADIUS);
    public Vector2 MouseInWorld { get; private set; }
    public Vector2 MouseInView { get; private set; }
    public void Pan(Vector2 delta)
    {
        position += delta * zoomLevel;
    }

    public Vector2 WorldToScreen(Vector2 worldCoord)
    {
        return (worldCoord - position) / zoomLevel;
    }

    public Vector2 ScreenToWorld(Vector2 screenCoord)
    {
        return screenCoord * zoomLevel + position;
    }

    public Rect WorldToScreen(Rect worldCoord)
    {
        var screen = worldCoord;
        screen.position = WorldToScreen(worldCoord.position);
        screen.size = worldCoord.size / zoomLevel;
        screen.yMin = Mathf.Floor(screen.yMin);
        screen.yMax = Mathf.Ceil(screen.yMax);
        screen.xMin = Mathf.Floor(screen.xMin);
        screen.xMax = Mathf.Ceil(screen.xMax);

        return screen;
    }

    public Rect ScreenToWorld(Rect screenCoord)
    {
        var world = screenCoord;
        world.position = ScreenToWorld(screenCoord.position);
        world.size = screenCoord.size * ZoomLevel;
        return world;
    }

    public bool HandleInput(Event e)
    {
        if (e.type == EventType.ScrollWheel)
        {
            var originalGraphPosition = ScreenToWorld(e.mousePosition);

            float zoomMultiplier = 0.1f;
            zoomMultiplier *= Mathf.Sign(e.delta.y);
            zoomLevel = Mathf.Clamp(zoomLevel * (1 + zoomMultiplier), 1, maxAllowedZoom);
            scale = 1 / zoomLevel;

            var newGraphPosition = ScreenToWorld(e.mousePosition);
            position += originalGraphPosition - newGraphPosition;
            return true;
        }
        MouseInView = e.mousePosition;
        MouseInWorld = ScreenToWorld(MouseInView);
        return false;
    }
    public Vector2 PointInWorld { get; private set; }
    public Rect ViewInWorld { get; private set; }

    public Event OnGUI(Vector2 size)
    {
        Event e = Event.current;
        HandleInput(e);
        PointInWorld = ScreenToWorld(e.mousePosition);
        ViewInWorld = ScreenToWorld(new Rect(Vector2.zero, size));
        return e;
    }

    public void DrawArea(Rect rect, Color color)
    {
        if (rect.Overlaps(ViewInWorld))
        {
            color.a = 0.4f;
            GUI.DrawTexture(WorldToScreen(rect), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
        }
    }

    public bool DrawText(Rect bounds, string content, string toolTip, GUIRenderFontStyle style)
    {
        if (!bounds.Overlaps(ViewInWorld))
            return false;
        style.Style.fontSize = Mathf.CeilToInt(style.FontSize * Scale);
        GUI.Label(WorldToScreen(bounds), new GUIContent(content, toolTip), style.Style);
        return true;
    }

    private static void GetTangents(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
    {
        points = new Vector3[] { start, end };
        tangents = new Vector3[2];

        // Heuristics to define the length of the tangents and tweak the look of the bezier curves.
        const float minTangent = 30;
        const float weight = 0.5f;
        float cleverness = Mathf.Clamp01(((start - end).magnitude - 10) / 50);
        tangents[0] = start + new Vector2((end.x - start.x) * weight + minTangent, 0) * cleverness;
        tangents[1] = end + new Vector2((end.x - start.x) * -weight - minTangent, 0) * cleverness;
    }


    public bool DrawLinkLines(Vector2 from, Vector2 to, Color color, float width)
    {
        width *= Scale;
        Vector3[] points, tangents;
        from = WorldToScreen(from);
        to = WorldToScreen(to);
        GetTangents(from, to, out points, out tangents);
        Handles.DrawBezier(points[0], points[1], tangents[0], tangents[1], color, null, width);
        return true;
    }

    public bool DrawRect(Rect rect, Color color, bool topCorner, bool bottomCorner, bool outLine = false)
    {
        if (!rect.Overlaps(ViewInWorld))
            return false;
        Vector4 boardRadius = Vector4.zero;
        if (topCorner)
            boardRadius += TopBoardRadius;
        if (bottomCorner)
            boardRadius += BottomBoardRadius;
        color.a = 0.4f;
        Rect realRect = WorldToScreen(rect);
        GUI.DrawTexture(realRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, color, Vector4.zero, boardRadius*Scale);
        if (outLine)
        {
            using (new Handles.DrawingScope(new Color(0, 1, 0.74f, 0.8f)))
            {
                float offset = 2;
                Vector2 pt1 = new Vector2(realRect.xMin - offset, realRect.yMin - offset);
                Vector2 pt2 = new Vector2(realRect.xMax + offset, realRect.yMin - offset);
                Vector2 pt3 = new Vector2(realRect.xMax + offset, realRect.yMax + offset + 1);
                Vector2 pt4 = new Vector2(realRect.xMin - offset, realRect.yMax + offset + 1);
                Handles.DrawAAPolyLine(1, pt1, pt2, pt3, pt4, pt1);
            }
        }
        return true;
    }

    public void DrawCircle(Vector2 pos, Color color, float radius, bool wire)
    {
        using (new Handles.DrawingScope(color))
        {
            radius *= scale;
            if (wire)
            {
                Handles.DrawWireDisc(WorldToScreen(pos), new Vector3(0, 0, 1), radius);
                //Handles.DrawWireDisc(WorldToScreen(pos), new Vector3(0, 0, 1), radius-1);
            }
            else
            {
                Handles.DrawSolidDisc(WorldToScreen(pos), new Vector3(0, 0, 1), radius);
            }

        }
    }
}
