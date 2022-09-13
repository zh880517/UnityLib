using UnityEngine;
namespace FrameLine
{
    public static class ClipOperateHelper
    {
        public static void AddClip(this FrameLineGUI gui, System.Type type)
        {

        }

        public static void RemoveSelectedClip(this FrameLineGUI gui)
        {
            gui.RegistUndo("remove clips");
            foreach (var clipRef in gui.SelectedClips)
            {
                gui.Asset.RemoveClip(clipRef);
            }
            gui.SelectedClips.Clear();
        }

        public static void MoveSelectedClip(this FrameLineGUI gui, int offsetFrame)
        {
            foreach (var clipRef in gui.SelectedClips)
            {
                int startFrame = clipRef.Clip.StartFrame - offsetFrame;
                startFrame = Mathf.Clamp(startFrame, 0, gui.FrameCount - 1);
                clipRef.Clip.StartFrame = startFrame;
            }
        }

        public static void MoveClipStart(this FrameLineGUI gui, FrameClipRef clipRef, int frame)
        {
            if (frame < 0)
                return;
            int endFrame = ClipUtil.GetClipEndFrame(gui.Asset, clipRef);
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

        public static void MoveClipEnd(this FrameLineGUI gui, FrameClipRef clipRef, int frame)
        {
            if (frame >= gui.FrameCount || frame < clipRef.Clip.StartFrame)
                return;
            if (clipRef.Clip.Length <= 0 && frame == (gui.FrameCount - 1))
                return;
            clipRef.Clip.Length = Mathf.Max(frame - clipRef.Clip.StartFrame + 1, 1);
        }

        public static void PasteClips(this FrameLineGUI gui)
        {

        }
    }
}
