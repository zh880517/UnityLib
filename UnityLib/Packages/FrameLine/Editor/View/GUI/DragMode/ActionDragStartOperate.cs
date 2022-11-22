using UnityEngine;

namespace FrameLine
{
    public class ActionDragStartOperate : DragOperateBase
    {
        private FrameActionRef actionRef;
        public ActionDragStartOperate(FrameLineGUI gui, FrameActionRef actionRef) : base(gui)
        {
            this.actionRef = actionRef;
            lastFrame = actionRef.Action.StartFrame;
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            GUI.MoveClipStart(actionRef.Action, frame);
        }
        public override FrameActionHitPartType GetDragePart(FrameActionRef clipRef)
        {
            if (this.actionRef == clipRef)
            {
                return FrameActionHitPartType.LeftCtrl;
            }
            return FrameActionHitPartType.None;
        }
    }
}
