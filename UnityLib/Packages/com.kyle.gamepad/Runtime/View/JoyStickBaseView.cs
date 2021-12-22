using UnityEngine;
using UnityEngine.EventSystems;
namespace GamePadView
{
    public abstract class JoyStickBaseView : BaseView, IDragHandler
    {
        [Tooltip("摇杆拖动半径")]
        public float Radius;
        [Tooltip("摇杆的底板")]
        public RectTransform Area;//摇杆底板
        [Tooltip("摇杆的按钮")]
        public RectTransform Stick;//摇杆
        protected Vector2 centerPosition;//摇杆的中心点


        public virtual void OnDrag(PointerEventData eventData)
        {
            if (pointId != eventData.pointerId)
                return;

            Camera camera = eventData.enterEventCamera ?? eventData.pressEventCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, camera, out var localPoint);
            Vector2 diff = localPoint - centerPosition;
            float length = diff.magnitude / Radius;
            length = Mathf.Clamp01(length);
            if (length > 0)
            {
                Vector2 val = diff.normalized * length;
                GamePadManager.Instance.ValueChange(Key, uid, val);
            }
        }
    }

}
