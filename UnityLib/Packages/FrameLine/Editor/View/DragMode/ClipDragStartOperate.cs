using UnityEngine;

namespace FrameLine
{
    public class ClipDragStartOperate : DragOperateBase
    {
        private FrameClipRef clipRef;
        public ClipDragStartOperate(FrameLineView view, FrameClipRef clipRef) : base(view)
        {
            this.clipRef = clipRef;
            lastFrame = clipRef.Clip.StartFrame;
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            view.MoveClipStart(clipRef.Clip, frame);
        }
        public override FrameClipHitPartType GetDragePart(FrameClipRef clipRef)
        {
            if (this.clipRef == clipRef)
            {
                return FrameClipHitPartType.LeftCtrl;
            }
            return FrameClipHitPartType.None;
        }
    }
}
