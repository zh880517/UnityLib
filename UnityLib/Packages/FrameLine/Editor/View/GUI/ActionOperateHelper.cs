using UnityEngine;
namespace FrameLine
{
    public static class ActionOperateHelper
    {
        public static void AddClip(this FrameLineGUI gui, System.Type type)
        {

        }

        public static void RemoveSelectedClip(this FrameLineGUI gui)
        {
            gui.RegistUndo("remove clips");
            foreach (var clipRef in gui.SelectedActions)
            {
                gui.Asset.RemoveClip(clipRef);
                gui.OnRemoveClip(clipRef);
            }
            gui.SelectedActions.Clear();
        }

        public static void MoveSelectedClip(this FrameLineGUI gui, int offsetFrame)
        {
            foreach (var clipRef in gui.SelectedActions)
            {
                int startFrame = clipRef.Action.StartFrame - offsetFrame;
                startFrame = Mathf.Clamp(startFrame, 0, gui.FrameCount - 1);
                clipRef.Action.StartFrame = startFrame;
            }
        }

        public static void MoveClipStart(this FrameLineGUI gui, FrameActionRef actionRef, int frame)
        {
            if (frame < 0)
                return;
            int endFrame = FrameActionUtil.GetClipEndFrame(gui.Asset, actionRef);
            if (frame > endFrame)
                return;
            int lastStart = actionRef.Action.StartFrame;
            actionRef.Action.StartFrame = frame;
            if (actionRef.Action.Length > 0)
            {
                int length = actionRef.Action.Length - (frame - lastStart);
                actionRef.Action.Length = Mathf.Max(length, 1);
            }
        }

        public static void MoveClipEnd(this FrameLineGUI gui, FrameActionRef actionRef, int frame)
        {
            if (frame >= gui.FrameCount || frame < actionRef.Action.StartFrame)
                return;
            if (actionRef.Action.Length <= 0 && frame == (gui.FrameCount - 1))
                return;
            actionRef.Action.Length = Mathf.Max(frame - actionRef.Action.StartFrame + 1, 1);
        }

        public static void PasteClips(this FrameLineGUI gui)
        {

        }
    }
}
