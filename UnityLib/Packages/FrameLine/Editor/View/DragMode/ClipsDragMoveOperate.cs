using UnityEngine;

namespace FrameLine
{
    public class ClipsDragMoveOperate : DragOperateBase
    {
        public ClipsDragMoveOperate(FrameLineGUI gui, int frame) : base(gui)
        {
            lastFrame = frame;
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            foreach (var clipRef in GUI.SelectedClips)
            {
                int startFrame = clipRef.Clip.StartFrame + (frame - lastFrame);
                startFrame = Mathf.Clamp(startFrame, 0, GUI.FrameCount - 1);
                clipRef.Clip.StartFrame = startFrame;
            }
        }
    }
}
