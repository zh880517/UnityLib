using UnityEngine;

namespace FrameLine
{
    public class ActionsDragMoveOperate : DragOperateBase
    {
        public ActionsDragMoveOperate(FrameLineGUI gui, int frame) : base(gui)
        {
            lastFrame = frame;
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            foreach (var clipRef in GUI.SelectedActions)
            {
                int startFrame = clipRef.Action.StartFrame + (frame - lastFrame);
                startFrame = Mathf.Clamp(startFrame, 0, GUI.FrameCount - 1);
                clipRef.Action.StartFrame = startFrame;
            }
        }
    }
}
