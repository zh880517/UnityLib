using UnityEngine;

namespace FrameLine
{
    public class ClipDragStartOperate : DragOperateBase
    {
        private FrameClipRef clipRef;
        public ClipDragStartOperate(FrameLineGUI gui, FrameClipRef clipRef) : base(gui)
        {
            this.clipRef = clipRef;
            lastFrame = clipRef.Clip.StartFrame;
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            GUI.MoveClipStart(clipRef.Clip, frame);
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
