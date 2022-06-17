using UnityEngine;

namespace FrameLine
{
    public abstract class DragOperateBase
    {
        protected FrameLineView view;
        public DragOperateBase(FrameLineView view)
        {
            this.view = view;
        }
        public abstract void OnDrag(Vector2 pos);
        public abstract void OnDragEnd();
    }
}
