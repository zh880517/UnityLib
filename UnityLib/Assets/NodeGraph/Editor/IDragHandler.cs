using UnityEngine;

public interface IDragHandler
{
    bool OnStartDrag(Vector2 mouseWorldPos);
    void OnDraging(Vector2 mouseWorldPos, Vector2 delta);
    void OnEndDrag(Vector2 mouseWorldPos);
}
