using UnityEngine;

namespace FrameLine
{
    public class ClipCtrlDragOperate : DragOperateBase
    {
        protected FrameClipHitResult hitResult;
        protected bool hasModify;
        protected int lastFrame;
        public ClipCtrlDragOperate(FrameLineView view, FrameClipHitResult hitResult) : base(view)
        {
            this.hitResult = hitResult;
            lastFrame = this.hitResult.Frame;
        }

        public override void OnDrag(Vector2 pos)
        {
            int frame = FrameUtil.PosToFrame(pos.x);
            if (!hasModify)
            {
                if (frame != hitResult.Frame)
                {
                    hasModify = true;
                    view.RegistUndo("move clip");
                }
            }
            if (hasModify)
            {
                if (hitResult.HitPart == FrameClipHitPartType.LeftCtrl)
                {
                    foreach (var clipRef in view.SelectedClips)
                    {
                        int startFrame = clipRef.Clip.StartFrame - (frame - lastFrame);
                        startFrame = Mathf.Clamp(startFrame, 0, view.FrameCount - 1);
                        clipRef.Clip.StartFrame = startFrame;
                    }
                }
                else if (hitResult.HitPart == FrameClipHitPartType.RightCtrl)
                {
                }
                lastFrame = frame;
                TrackUtil.UpdateAllTrack(view.Asset);
            }
        }

        public override void OnDragEnd()
        {
            
        }
    }
}
