using UnityEngine;

namespace FrameLine
{
    public abstract class DragOperateBase
    {
        protected FrameLineGUI GUI;
        protected int lastFrame;
        protected bool hasModify { get; private set; }
        public bool HasDraged { get; private set; }
        public DragOperateBase(FrameLineGUI gui)
        {
            GUI = gui;
        }
        public void Drag(Vector2 pos)
        {
            HasDraged = true;
            int frame = FrameUtil.PosToFrame(pos.x);
            if (frame != lastFrame)
            {
                if (!hasModify)
                {
                    GUI.RegistUndo("drag clip start");
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
                TrackUtil.UpdateAllTrack(GUI);
            }
        }

        public virtual FrameClipHitPartType GetDragePart(FrameClipRef clipRef)
        {
            return FrameClipHitPartType.None;
        }
    }
}
