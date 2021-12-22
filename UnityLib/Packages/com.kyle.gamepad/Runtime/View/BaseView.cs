using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace GamePadView
{
    public abstract class BaseView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        protected int uid { get; private set; }
        protected int pointId = -1;
        protected Vector2 pressPoint;
        public int Key;
        [Tooltip("自动注册到管理中")]
        public bool AutoRegister;
        public UnityEvent OnPressDown;
        public UnityEvent OnPressUp;

        protected virtual void Start()
        {
            if (AutoRegister)
                Register();
        }

        public void Register()
        {
            UnRegister();
            uid = GamePadManager.Instance.RegisterView(Key, this);
        }

        public void UnRegister()
        {
            if (uid != 0)
            {
                GamePadManager.Instance.UnRegisterView(this);
                uid = 0;
            }
        }

        private void OnEnable()
        {
            if (uid != 0)
            {
                //GamePadManager.Instance.SetActive(Key, true);
            }
        }

        private void OnDisable()
        {
            pointId = -1;
            if (uid != 0)
            {
                GamePadManager.Instance.Release(Key, uid);
                //GamePadManager.Instance.SetActive(Key, false);
            }
        }

        protected virtual void OnDestroy()
        {
            UnRegister();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (uid == 0 || pointId != -1 || eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!GamePadManager.Instance.CheckStartInput(Key, uid))
                return;
            pointId = eventData.pointerId;
            Camera camera = eventData.enterEventCamera ?? eventData.pressEventCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, camera, out pressPoint);
            GamePadManager.Instance.Press(Key, uid);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pointId == eventData.pointerId)
            {
                pointId = -1;
                GamePadManager.Instance.Release(Key, uid);
            }
        }

        public virtual void OnPress()
        {
            OnPressDown.Invoke();
        }
        public virtual void OnRelease()
        {
            pointId = -1;
            OnPressUp.Invoke();
        }
        public abstract void OnValueChange(Vector2 val);

        public virtual void SetState(bool isButton)
        {
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus && pointId != -1)
            {
                GamePadManager.Instance.Release(Key, uid);
            }
        }
    }

}
