using UnityEditor;
using UnityEngine;
namespace FrameLine
{
    public static class FrameLineDrawer
    {
        public static void DrawFrameLineBackGround(FrameLineGUI gui, Rect showRect)
        {
            using (new Handles.DrawingScope(new Color(0.5f, 0.5f, 0.5f, 0.5f)))
            {
                int startIndex = Mathf.Clamp(gui.VisableFrameStart, 0, gui.Asset.FrameCount);
                int endIndex = Mathf.Clamp(gui.VisableFrameEnd, 0, gui.Asset.FrameCount);
                for (int i = startIndex; i <= endIndex; ++i)
                {
                    float xPos = i * ViewStyles.FrameWidth;
                    Handles.DrawLine(new Vector2(xPos, showRect.yMin), new Vector2(xPos, showRect.yMax));
                    if (i != endIndex)
                        GUI.Label(new Rect(xPos, 0, ViewStyles.FrameWidth, ViewStyles.FrameBarHeight), i.ToString(), ViewStyles.FrameNumStyle);
                }
            }
            if (gui.CurrentFrame >= gui.VisableFrameStart && gui.CurrentFrame <= gui.VisableFrameEnd)
            {
                Rect rect = new Rect(gui.CurrentFrame * ViewStyles.FrameWidth, 0, ViewStyles.FrameWidth, showRect.height);
                GUIRenderHelper.DrawRect(rect, ViewStyles.SelectFrameBackGroundColor, 5, BorderType.Top);
            }
        }

        public static void DrawTrackHead(FrameLineGUI gui)
        {
            int trackIndex = 0;
            foreach (var track in gui.Asset.Tracks)
            {
                if (track.Count == 0)
                    continue;
                if (trackIndex > gui.VisableTrackEnd)
                    return;
                int trackVisableCount = (track.Foldout ? track.Count : 1);
                do
                {
                    if (trackIndex + trackVisableCount <= gui.VisableTrackStart)
                        break;
                    int startIndex = Mathf.Clamp(trackIndex, gui.VisableFrameStart, gui.VisableTrackEnd) - trackIndex;
                    int endIndex = Mathf.Clamp(trackIndex + trackVisableCount, gui.VisableFrameStart, gui.VisableTrackEnd) - trackIndex;
                    DrawTrackHead(gui, track, trackIndex, startIndex, endIndex);
                } while (false);
                trackIndex += trackVisableCount;
            }
        }

        public static void DrawFrameClips(FrameLineGUI gui, bool mouseInView, Vector2 mousePos)
        {
            int trackIndex = 0;
            foreach (var track in gui.Asset.Tracks)
            {
                if (track.Count == 0)
                    continue;
                if (trackIndex > gui.VisableTrackEnd)
                    return;
                int trackVisableCount = (track.Foldout ? track.Count : 1);
                do
                {
                    if (trackIndex + trackVisableCount <= gui.VisableTrackStart)
                        break;
                    int startIndex = Mathf.Clamp(trackIndex, gui.VisableTrackStart, gui.VisableTrackEnd) - trackIndex;
                    int endIndex = Mathf.Clamp(trackIndex + trackVisableCount - 1, gui.VisableFrameStart, gui.VisableTrackEnd) - trackIndex;
                    FrameLineDrawer.DrawTrack(gui, track, trackIndex, startIndex, endIndex, mouseInView, mousePos);
                } while (false);
                trackIndex += trackVisableCount;
            }
        }

        public static void DrawTrackHead(FrameLineGUI gui, FrameLineTrack track, int viewTrackIndex, int startSubIndex, int endSubIndex)
        {
            float viewOffsetY = viewTrackIndex * (ViewStyles.ClipHeight + ViewStyles.ClipVInterval);
            int visableSubTrackCount = track.Foldout ? track.Count : 1;
            float trackHeight = ViewStyles.TrackHeight * visableSubTrackCount - ViewStyles.ClipVInterval;
            Rect rect = new Rect(ViewStyles.TrackFoldSize, viewOffsetY, ViewStyles.TrackHeadWidth - ViewStyles.TrackFoldSize, trackHeight);
            GUIRenderHelper.DrawRect(rect, ViewStyles.TrackBGColor, 5, BorderType.Left);
            Rect titleRect = rect;
            titleRect.height = ViewStyles.ClipHeight;
            GUI.Label(titleRect, track.Name);
            if (track.Count > 1)
            {
                Rect foldRect = new Rect(0, viewOffsetY, ViewStyles.TrackFoldSize, ViewStyles.ClipHeight);
                track.Foldout = EditorGUI.Foldout(foldRect, track.Foldout, "");
            }
        }
        public static void DrawTrack(FrameLineGUI gui, FrameLineTrack track, int viewTrackIndex, int startSubIndex, int endSubIndex, bool mouseInView, Vector2 mousePos)
        {
            float viewOffsetY = viewTrackIndex * (ViewStyles.ClipHeight + ViewStyles.ClipVInterval);
            int visableSubTrackCount = track.Foldout ? track.Count : 1;
            float trackHeight = ViewStyles.TrackHeight * visableSubTrackCount - ViewStyles.ClipVInterval;
            Rect rect = new Rect(0, viewOffsetY, gui.FrameCount * ViewStyles.FrameWidth, trackHeight);
            GUIRenderHelper.DrawRect(rect, ViewStyles.TrackBGColor);
            if (!track.Foldout)
            {
                //画被折叠的轨道
                for (int i = 1; i < track.Count; ++i)
                {
                    var clip = track.Clips[i].Clip;
                    if (clip.StartFrame > gui.VisableFrameEnd || (clip.Length > 0 && clip.StartFrame + clip.Length < gui.VisableFrameStart))
                        continue;
                    float offsetY = viewOffsetY;
                    float offsetX = clip.StartFrame * ViewStyles.FrameWidth;
                    int frameCount = clip.Length;
                    if (clip.Length <= 0 || frameCount > (gui.FrameCount - clip.StartFrame))
                        frameCount = gui.FrameCount - clip.StartFrame;
                    Rect clipRect = new Rect(offsetX, offsetY, ViewStyles.FrameWidth * frameCount, ViewStyles.ClipHeight);
                    GUIRenderHelper.DrawRect(clipRect, ViewStyles.InvalidClipColor, 5, BorderType.All);
                }
            }
            for (int i = 0; i < track.Count; ++i)
            {
                var clip = track.Clips[i].Clip;
                if (i < startSubIndex || i > endSubIndex)
                    continue;
                if (clip.StartFrame > gui.VisableFrameEnd || (clip.Length > 0 && clip.StartFrame + clip.Length < gui.VisableFrameStart))
                    continue;
                float offsetY = viewOffsetY + i * ViewStyles.TrackHeight;
                float offsetX = clip.StartFrame * ViewStyles.FrameWidth;
                int frameCount = clip.Length;
                if (clip.Length <= 0 || frameCount > (gui.FrameCount - clip.StartFrame))
                    frameCount = gui.FrameCount - clip.StartFrame;
                //左侧控制区域
                Rect clipLeftCtrlRect = new Rect(offsetX, offsetY, ViewStyles.ClipCtrlWidth, ViewStyles.ClipHeight);
                FrameClipHitPartType dragPart = gui.Event.GetDragePart(clip);
                Color color = dragPart == FrameClipHitPartType.LeftCtrl ? ViewStyles.ClipSelectCtrlColor : ViewStyles.ClipCtrlColor;
                GUIRenderHelper.DrawRect(clipLeftCtrlRect, color, ViewStyles.ClipCtrlWidth, BorderType.Left);
                //右侧
                int clipEndFrame = clip.StartFrame + frameCount - 1;
                Rect clipRightCtrlRect = new Rect((clipEndFrame + 1) * ViewStyles.FrameWidth - ViewStyles.ClipCtrlWidth,
                    offsetY,
                    ViewStyles.ClipCtrlWidth,
                    ViewStyles.ClipHeight);
                if (clip.Length > 0)
                {
                    color = dragPart == FrameClipHitPartType.RightCtrl ? ViewStyles.ClipSelectCtrlColor : ViewStyles.ClipCtrlColor;
                    GUIRenderHelper.DrawRect(clipRightCtrlRect, color, ViewStyles.ClipCtrlWidth, BorderType.Right);
                }
                else
                {
                    GUIRenderHelper.DrawRect(clipRightCtrlRect, ViewStyles.ClipColor);
                }
                //中间区域
                Rect clipRect = new Rect(clipLeftCtrlRect.xMax, offsetY, clipRightCtrlRect.xMin - clipLeftCtrlRect.xMax, ViewStyles.ClipHeight);
                GUIRenderHelper.DrawRect(clipRect, ViewStyles.ClipColor);
                if (gui.IsSlecected(clip))
                {
                    Rect fullRect = new Rect(offsetX, offsetY, ViewStyles.FrameWidth * frameCount, ViewStyles.ClipHeight);
                    GUIRenderHelper.DrawWireRect(fullRect, ViewStyles.SelectClipWireFrameColor, ViewStyles.ClipCtrlWidth, BorderType.All);
                }
            }
        }
    }
}
