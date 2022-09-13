using UnityEngine;

namespace FrameLine
{
    public class ClipDragEndOperate : DragOperateBase
    {
        private FrameClipRef clipRef;
        public ClipDragEndOperate(FrameLineGUI gui, FrameClipRef clipRef) : base(gui)
        {
            this.clipRef = clipRef;
            lastFrame = ClipUtil.GetClipEndFrame(gui.Asset, clipRef);
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            GUI.MoveClipEnd(clipRef.Clip, frame);
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
