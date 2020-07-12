using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GUICamera
{
    public GUICamera()
    {
    }

    public GUICamera(bool enablemouseright)
    {
        enableMouseRightbtn = enablemouseright;
    }

    bool enableMouseRightbtn = true;
    float maxAllowedZoom = 6.0f;
    public float MaxAllowedZoom
    {
        get { return maxAllowedZoom; }
        set { maxAllowedZoom = value; }
    }

    [SerializeField]
    Vector2 position = Vector2.zero;
    public Vector2 Position
    {
        get
        {
            return position;
        }
    }

    public Vector2 ScreenOffset = Vector2.zero;

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

    public Vector2 MouseInWorld { get; private set; }

    public Rect ViewWorldBounds { get; private set; }
    
    public Vector2 MouseInView { get; private set; }
    public Rect ViewBounds { get; private set; }

    public void Pan(int x, int y)
    {
        Pan(new Vector2(x, y));
    }
    
    public void Pan(Vector2 delta)
    {
        position += delta * zoomLevel;
    }
    
    public bool HandleInput(Event e)
    {
        MouseInView = e.mousePosition;
        // Handle zooming
        if (e.type == EventType.ScrollWheel)
        {
            // Grab the original position under the mouse so we can restore it after the zoom
            var originalGraphPosition = ScreenToWorld(e.mousePosition);

            float zoomMultiplier = 0.1f;
            zoomMultiplier *= Mathf.Sign(e.delta.y);
            zoomLevel = Mathf.Clamp(zoomLevel * (1 + zoomMultiplier), 1, maxAllowedZoom);
            scale = 1 / zoomLevel;

            var newGraphPosition = ScreenToWorld(e.mousePosition);
            position += originalGraphPosition - newGraphPosition;
            return true;
        }

        // Handle pan
        int dragButton1 = 1;
        int dragButton2 = 2;
        if (e.type == EventType.MouseDrag && ((enableMouseRightbtn && e.button == dragButton1) || e.button == dragButton2))
        {
            if (e.delta.magnitude < 150)
            {
                Pan(-e.delta);
                e.Use();
            }
        }
        MouseInWorld = ScreenToWorld(e.mousePosition);
        return false;
    }

    public void UpdateViewSize(Vector2 viewSize)
    {
        ViewBounds = new Rect(Vector2.zero, viewSize);
        ViewWorldBounds = ScreenToWorld( new Rect(Vector2.zero, viewSize));
    }

    public Vector2 WorldToScreen(Vector2 worldCoord)
    {
        return (worldCoord - position) / zoomLevel + ScreenOffset;
    }
    
    public Vector2 ScreenToWorld(Vector2 screenCoord)
    {
        screenCoord -= ScreenOffset;

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

    private GUIStyle style;
    private static readonly Color TextColor = new Color(1, 1, 1, 0.2f);
    private static readonly Color LineColor = new Color(1, 1, 1, 0.1f);
    private static readonly Color LineColorThin = new Color(1, 1, 1, 0.05f);
    //显示缩放系数
    public void DrawZoomLevel()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20
            };
            style.normal.textColor = TextColor;
        }
        var x = 20 + ViewBounds.position.x;
        var y = ViewBounds.height - 100;
        var textBounds = new Rect(x, y, ViewBounds.width - 20, 70);
        style.alignment = TextAnchor.LowerLeft;
        float zoomLevel = ZoomLevel;
        if (ZoomLevel > 1)
            zoomLevel = (float)System.Math.Round(ZoomLevel, 1);

        GUI.Label(textBounds, string.Format("Zoom Level: {0:0.0} Pos : {1}", zoomLevel, position), style);
    }

    public void DrawGrid()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f); ;
        GUI.Box(ViewBounds, "");
        GUI.backgroundColor = oldColor;
        float cellSizeWorld = 20 * Scale;

        Vector2 worldStart, worldEnd;

        worldStart = Vector2.zero;
        worldEnd = ViewBounds.size;

        int sx = Mathf.FloorToInt(worldStart.x / cellSizeWorld);
        int sy = Mathf.FloorToInt(worldStart.y / cellSizeWorld);

        int ex = Mathf.CeilToInt(worldEnd.x / cellSizeWorld);
        int ey = Mathf.CeilToInt(worldEnd.y / cellSizeWorld);

        for (int x = sx; x <= ex; x++)
        {
            var startScreen = new Vector2(x, sy) * cellSizeWorld;
            var endScreen = new Vector2(x, ey) * cellSizeWorld;

            Handles.color = (x % 2 == 0) ? LineColor : LineColorThin;
            Handles.DrawLine(startScreen, endScreen);
        }

        for (int y = sy; y <= ey; y++)
        {
            var startScreen = new Vector2(sx, y) * cellSizeWorld;
            var endScreen = new Vector2(ex, y) * cellSizeWorld;

            Handles.color = (y % 2 == 0) ? LineColor : LineColorThin;
            Handles.DrawLine(startScreen, endScreen);
        }
        Handles.color = Color.white;
    }
}
