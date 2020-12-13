using System.Collections.Generic;
using UnityEngine;

public class ViewAreaSelectMode : IViewDragMode
{
    private Vector2 Start;
    private Rect Area;
    private static readonly Color RectColor = new Color(0.8f, 0.6f, 0.11f, 0.8f);

    public ViewAreaSelectMode(StateGraphView view, Vector2 ptInWorld)
    {
        view.Selecteds.Clear();
        Area.size = Vector2.zero;
        Start = ptInWorld;
    }

    public void Draw(StateGraphView view)
    {
        Vector2 size = Area.size;
        if (size.x > 2 || size.y > 2)
        {
            view.Canvas.DrawArea(Area, RectColor);
        }
    }

    public void OnDrag(StateGraphView view, Vector2 ptInWorld)
    {
        view.Selecteds.Clear();
        Vector2 size = ptInWorld - Start;
        Area.center = size * 0.5f;
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        Area.size = size;
        view.Selecteds.Clear();
        foreach (var node in view.Graph.Nodes)
        {
            if (node.Bounds.Overlaps(Area))
            {
                view.Selecteds.Add(node);
            }
        }
    }

    public void OnDragEnd(StateGraphView view, Vector2 ptInWorld)
    {
        HashSet<ulong> handleNodes = new HashSet<ulong>();
        foreach (var node in view.Selecteds)
        {
            var parent = node.Node.Parent;
            if (!parent)
            {
                handleNodes.Add(node.Id);
                node.Node.SortIndex = ++view.SelectIndex;
            }
            else if (!handleNodes.Contains(parent.Id))
            {
                handleNodes.Add(parent.Id);
                parent.Node.SortIndex = ++view.SelectIndex;
                foreach (var link in view.Graph.Links)
                {
                    if (link.IsChild && link.From == parent)
                    {
                        link.To.Node.SortIndex = ++view.SelectIndex;
                    }
                }
            }
        }
        if (view.Selecteds.Count > 0)
        {
            view.SortNodes();
        }
    }

}
