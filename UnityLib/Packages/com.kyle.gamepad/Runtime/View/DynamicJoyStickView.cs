using UnityEngine;
namespace GamePadView
{
    public class DynamicJoyStickView : JoyStickBaseView
    {
        private Vector2 initPosition;
        private void Awake()
        {
            initPosition = Area.anchoredPosition;
            pressPoint = initPosition;
        }

        public override void OnPress()
        {
            base.OnPress();
            Vector2 size = (transform as RectTransform).rect.size * 0.5f;
            size.x = size.x - Radius;
            size.y = size.y - Radius;

            pressPoint.x = Mathf.Clamp(pressPoint.x, -size.x, size.x);
            pressPoint.y = Mathf.Clamp(pressPoint.y, -size.y, size.y);
            Area.anchoredPosition = pressPoint;
            Stick.anchoredPosition = pressPoint;
            centerPosition = pressPoint;
        }

        public override void OnRelease()
        {
            base.OnRelease();
            Area.anchoredPosition = initPosition;
            Stick.anchoredPosition = initPosition;
            pressPoint = initPosition;
        }

        public override void OnValueChange(Vector2 val)
        {
            Vector2 localPoint = pressPoint + val * Radius;
            Stick.anchoredPosition = localPoint;
        }
    }

}
