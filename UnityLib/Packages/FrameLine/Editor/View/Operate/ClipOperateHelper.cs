using UnityEngine;
namespace FrameLine
{
    public static class ClipOperateHelper
    {
        public static void AddClip(this FrameLineView view, System.Type type)
        {

        }

        public static void RemoveSelectedClip(this FrameLineView view)
        {
            view.RegistUndo("remove clips");
            foreach (var clipRef in view.SelectedClips)
            {
                view.Asset.RemoveClip(clipRef);
            }
            view.SelectedClips.Clear();
        }

        public static void MoveSelectedClip(this FrameLineView view, int offsetFrame)
        {
            foreach (var clipRef in view.SelectedClips)
            {
                int startFrame = clipRef.Clip.StartFrame - offsetFrame;
                startFrame = Mathf.Clamp(startFrame, 0, view.FrameCount - 1);
                clipRef.Clip.StartFrame = startFrame;
            }
        }

        public static void MoveClipStart(this FrameLineView view, FrameClipRef clipRef, int frame)
        {
            if (frame < 0)
                return;
            int endFrame = ClipUtil.GetClipEndFrame(view.Asset, clipRef);
            if (frame > endFrame)
                return;
            int lastStart = clipRef.Clip.StartFrame;
            clipRef.Clip.StartFrame = frame;
            if (clipRef.Clip.Length > 0)
            {
                int length = clipRef.Clip.Length - (frame - lastStart);
                clipRef.Clip.Length = Mathf.Max(length, 1);
            }
        }

        public static void MoveClipEnd(this FrameLineView view, FrameClipRef clipRef, int frame)
        {
            if (frame >= view.FrameCount || frame < clipRef.Clip.StartFrame)
                return;
            if (clipRef.Clip.Length <= 0 && frame == (view.FrameCount - 1))
                return;
            clipRef.Clip.Length = Mathf.Max(frame - clipRef.Clip.StartFrame + 1, 1);
        }

        public static void PasteClips(this FrameLineView view)
        {

        }
    }
}
