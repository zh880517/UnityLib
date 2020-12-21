using UnityEngine;

public interface IViewDragMode
{
    void OnDrag(StateGraphView view, Vector2 ptInWorld);
    void OnDragEnd(StateGraphView view, Vector2 ptInWorld);

    void Draw(StateGraphView view);
}
