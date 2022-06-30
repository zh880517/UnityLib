using UnityEngine;

namespace FrameLine
{
    public class ClipsDragMoveOperate : DragOperateBase
    {
        public ClipsDragMoveOperate(FrameLineView view, int frame) : base(view)
        {
            lastFrame = frame;
        }

        protected override void OnDrag(Vector2 pos, int frame)
        {
            foreach (var clipRef in view.SelectedClips)
            {
                int startFrame = clipRef.Clip.StartFrame + (frame - lastFrame);
                startFrame = Mathf.Clamp(startFrame, 0, view.FrameCount - 1);
                clipRef.Clip.StartFrame = startFrame;
            }
        }
    }
}
