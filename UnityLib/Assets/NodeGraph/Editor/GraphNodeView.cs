using UnityEngine;

public class GraphNodeView : ScriptableObject
{
    public GUICamera Camera = new GUICamera();
    public GraphLayout Layout;


    public virtual void OnRender(Vector2 size)
    {
        Camera.UpdateViewSize(size);
        Camera.DrawGrid();
        Layout.Draw(Camera);
        Camera.DrawZoomLevel();

    }

    public virtual bool OnEvent()
    {
        Event e = Event.current;
        if (Camera.HandleInput(e))
            return true;

        return e.type == EventType.Used;
    }
}
 