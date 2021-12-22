using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePadView
{
    public class StaticJoyStickView : JoyStickBaseView
    {
        public bool ControlShowState;
        private bool asButton;

        private void Awake()
        {
            centerPosition = Area.anchoredPosition;
            if (ControlShowState)
            {
                Area.gameObject.SetActive(false);
                Stick.gameObject.SetActive(false);
            }
        }

        public override void SetState(bool isButton)
        {
            asButton = isButton;
        }

        public override void OnPress()
        {
            base.OnPress();
            if (ControlShowState)
            {
                Area.gameObject.SetActive(true);
                Stick.gameObject.SetActive(true);
            }
        }

        public override void OnRelease()
        {
            base.OnRelease();
            Stick.anchoredPosition = Area.anchoredPosition;
            if (ControlShowState)
            {
                Area.gameObject.SetActive(false);
                Stick.gameObject.SetActive(false);
            }
        }

        public override void OnValueChange(Vector2 val)
        {
            Vector2 localPoint = Area.anchoredPosition + val * Radius;
            Stick.anchoredPosition = localPoint;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!asButton)
                base.OnDrag(eventData);
        }
    }

}
