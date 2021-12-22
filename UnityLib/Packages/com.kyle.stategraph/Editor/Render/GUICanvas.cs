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
    private const float LINK_LINE_OFFSET = 30;
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
            e.Use();
            return true;
        }
        MouseInView = e.mousePosition;
        MouseInWorld = ScreenToWorld(MouseInView);
        return false;
    }
    public Vector2 PointInWorld { get; private set; }
    public Rect ViewInWorld { get; private set; }

    public void OnGUI(Vector2 size, Event e)
    {
        Rect rect = new Rect(Vector2.zero, size);
        if (!rect.Contains(e.mousePosition))
            return;
        HandleInput(e);
        ViewInWorld = ScreenToWorld(rect);
        PointInWorld = ScreenToWorld(e.mousePosition);
    }

    public void DrawArea(Rect rect, Color color)
    {
        if (rect.Overlaps(ViewInWorld))
        {
            color.a = 0.4f;
            GUI.DrawTexture(WorldToScreen(rect), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
        }
    }

    public void DrawIcon(Rect bounds, Texture icon)
    {
        if (bounds.Overlaps(ViewInWorld))
        {
            GUI.DrawTexture(WorldToScreen(bounds), icon, ScaleMode.ScaleToFit);
        }
    }

    public bool DrawText(Rect bounds, string content, Texture img, string toolTip, GUIRenderStyle style)
    {
        if (!bounds.Overlaps(ViewInWorld))
            return false;
        GUI.Label(WorldToScreen(bounds), new GUIContent(content, img, toolTip), style.GetStyle(scale));
        return true;
    }

    public bool DrawButton(Rect bounds, string content, GUIRenderStyle style)
    {
        if (!bounds.Overlaps(ViewInWorld))
            return false;
        return GUI.Button(WorldToScreen(bounds), content, style.GetStyle(scale));
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


    public bool DrawLinkLines(Vector2 from, Vector2 to, Color color, float width, bool reversalFrom = false, bool reversalTo = false)
    {
        width *= Scale;
        if (!ViewInWorld.Contains(from) && !ViewInWorld.Contains(to))
            return false;
        float offet = LINK_LINE_OFFSET * Scale;
        using (new Handles.DrawingScope(color))
        {
            from = WorldToScreen(from);
            to = WorldToScreen(to);
            if (Mathf.Abs(from.y - to.y) < 10)
            {
                Handles.DrawAAPolyLine(width, from, to);
            }
            else
            {
                
                var p1 = from + new Vector2(offet*(reversalFrom ? -1 : 1), 0);
                
                var p3 = to - new Vector2(offet * (reversalTo ? -1 : 1), 0);
                var diff = p3 - p1;
                var dir = diff.normalized;
                var distance = diff.magnitude*0.5f;
                if (offet > distance)
                {
                    offet = distance;
                }
                Vector2 p4 = p1 + dir * offet;
                Vector2 p5 = p3 - dir * offet;
                //出口
                Handles.DrawBezier(from, p4, p1, p1, color, null, width);
                //入口
                Handles.DrawBezier(to, p5, p3, p3, color, null, width);

                //单条线颜色太浅，和贝塞尔曲线的颜色不匹配
                Handles.DrawAAPolyLine(width, p5, p4);
                Handles.DrawAAPolyLine(width, p5, p4);
            }
        }
        return true;
    }

    public void DrawStraightLine(Vector2 from, Vector2 to, Color color, float width)
    {
        if (!ViewInWorld.Contains(from) && !ViewInWorld.Contains(to))
            return ;
        using (new Handles.DrawingScope(color))
        {
            Handles.DrawAAPolyLine(width, WorldToScreen(from), WorldToScreen(to));
        }
    }

    public bool DrawRect(Rect rect, Color color, bool topCorner, bool bottomCorner, bool outLine = false, bool select = false)
    {
        if (!rect.Overlaps(ViewInWorld))
            return false;
        Vector4 boardRadius = Vector4.zero;
        if (topCorner)
            boardRadius += TopBoardRadius;
        if (bottomCorner)
            boardRadius += BottomBoardRadius;
        Rect realRect = WorldToScreen(rect);
        GUI.DrawTexture(realRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, color, Vector4.zero, boardRadius*Scale);
        if (outLine || select)
        {
            Color lineColor = select ? StateGraphEditorStyles.SelectOutLineColor : StateGraphEditorStyles.DefultOutLineColor;

            realRect.position -= new Vector2(2, 2);
            realRect.size += new Vector2(4, 4);
            float width = Mathf.Floor(2 * Scale);
            GUI.DrawTexture(realRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, lineColor, Vector4.one * width, boardRadius * Scale);
        }

        return true;
    }

    public void DrawCircle(Vector2 pos, Color color, float radius, bool wire)
    {
        if (!ViewInWorld.Contains(pos))
            return;

        Vector2 halfSize = new Vector2(radius, radius);

        Rect rect = new Rect(pos - halfSize, halfSize * 2);
        GUI.DrawTexture(WorldToScreen(rect), Texture2D.whiteTexture, ScaleMode.ScaleToFit, true, 0, color, wire ? 1 : 0, radius * scale);
    }
}
