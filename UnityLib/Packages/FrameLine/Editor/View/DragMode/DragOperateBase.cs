using UnityEngine;

namespace FrameLine
{
    public abstract class DragOperateBase
    {
        protected FrameLineView view;
        protected int lastFrame;
        protected bool hasModify { get; private set; }
        public bool HasDraged { get; private set; }
        public DragOperateBase(FrameLineView view)
        {
            this.view = view;
        }
        public void Drag(Vector2 pos)
        {
            HasDraged = true;
            int frame = FrameUtil.PosToFrame(pos.x);
            if (frame != lastFrame)
            {
                if (!hasModify)
                {
                    view.RegistUndo("drag clip start");
                    hasModify = true;
                }
                OnDrag(pos, frame);
                lastFrame = frame;
            }
        }
        protected abstract void OnDrag(Vector2 pos, int frame);
        public virtual void OnDragEnd() 
        {
            if (hasModify)
            {
                TrackUtil.UpdateAllTrack(view.Asset);
            }
        }
    }
}
