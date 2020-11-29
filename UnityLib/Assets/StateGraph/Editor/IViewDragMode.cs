using UnityEngine;

public interface IViewDragMode
{
    void OnStartDrag(StateGraphView view, Vector2 ptInWorld);
    void OnDrag(StateGraphView view, Vector2 ptInWorld);
    void OnDragEnd(StateGraphView view, Vector2 ptInWorld);
}
