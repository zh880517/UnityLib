using UnityEngine;

namespace FrameLine
{
    public class ClipDragEndOperate : DragOperateBase
    {
        private FrameClipRef clipRef;
        public ClipDragEndOperate(FrameLineView view, FrameClipRef clipRef) : base(view)
        {
            this.clipRef = clipRef;
            lastFrame = ClipUtil.GetClipEndFrame(view.Asset, clipRef);
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            view.MoveClipEnd(clipRef.Clip, frame);
        }

        public override FrameClipHitPartType GetDragePart(FrameClipRef clipRef)
        {
            if (this.clipRef == clipRef)
            {
                return FrameClipHitPartType.RightCtrl;
            }
            return FrameClipHitPartType.None;
        }
    }
}
