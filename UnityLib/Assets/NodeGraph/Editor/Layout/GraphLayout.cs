using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class GraphLayout : ScriptableObject, IMouseEventHandler
{
    protected const float LineWidth = 3;
    [SerializeField]
    protected NodeGraph Graph;
    [SerializeField]
    protected List<GraphNodeRef> selectNodes = new List<GraphNodeRef>();

    public ReadOnlyCollection<GraphNodeRef> SelectNodes { get { return selectNodes.AsReadOnly(); } }
    public static TLayout Create<TLayout>(NodeGraph graph) where TLayout : GraphLayout
    {
        AutoGraphStyles.Init();
        TLayout layout = CreateInstance<TLayout>();
        layout.Graph = graph;
        return layout;
    }

    protected bool IsSelecetd(GraphNode node)
    {
        return selectNodes.FindIndex(obj => obj.GUID == node.GUID) >= 0;
    }
    public abstract void RefreshLayout();
    public abstract void Draw(GUICamera camera);
    public abstract void OnMouseDown(Event e, Vector2 mouseWorldPos);
    public abstract bool OnStartDrag(Vector2 mouseWorldPos);
    public abstract void OnDraging(Vector2 mouseWorldPos, Vector2 delta);
    public abstract void OnEndDrag(Vector2 mouseWorldPos);
}
