using UnityEngine;

namespace FrameLine
{
    public class ActionDragEndOperate : DragOperateBase
    {
        private FrameActionRef clipRef;
        public ActionDragEndOperate(FrameLineGUI gui, FrameActionRef actionRef) : base(gui)
        {
            this.clipRef = actionRef;
            lastFrame = FrameActionUtil.GetClipEndFrame(gui.Asset, actionRef);
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            GUI.MoveClipEnd(clipRef.Action, frame);
        }

        public override FrameActionHitPartType GetDragePart(FrameActionRef actionRef)
        {
            if (this.clipRef == actionRef)
            {
                return FrameActionHitPartType.RightCtrl;
            }
            return FrameActionHitPartType.None;
        }
    }
}
