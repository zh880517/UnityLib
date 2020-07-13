using System.Collections.Generic;
using UnityEngine;

public abstract class GraphLayout : ScriptableObject
{
    protected const float LineWidth = 3;
    protected NodeGraph Graph;
    [SerializeField]
    protected List<GraphNodeRef> SelectNodes = new List<GraphNodeRef>();
    public static TLayout Create<TLayout>(NodeGraph graph) where TLayout : GraphLayout
    {
        AutoGraphStyles.Init();
        TLayout layout = CreateInstance<TLayout>();
        layout.Graph = graph;
        return layout;
    }

    protected bool IsSelecetd(GraphNode node)
    {
        return SelectNodes.FindIndex(obj => obj.GUID == node.GUID) >= 0;
    }
    public virtual bool EnableMultDrag => true;
    public abstract void RefreshLayout();
    public abstract void Draw(GUICamera camera);
    public abstract void OnMouseDown(Event e, Vector2 mouseWorldPos);
    public abstract void OnMouseUp(Event e, Vector2 mouseWorldPos);
    public abstract void OnStartDrag(Vector2 mouseWorldPos);
    public abstract void OnDraging(Vector2 mouseWorldPos, Vector2 delta);
    public abstract void OnEndDrag(Vector2 mouseWorldPos);
}
