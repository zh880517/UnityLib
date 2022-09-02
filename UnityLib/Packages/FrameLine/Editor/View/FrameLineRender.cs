using UnityEditor;
using UnityEngine;
namespace FrameLine
{
    public static class FrameLineRender
    {

        public static void DrawTrackHead(FrameLineView view, FrameLineTrack track, int viewTrackIndex, int startSubIndex, int endSubIndex)
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
        public static void DrawTrack(FrameLineView view, FrameLineTrack track, int viewTrackIndex, int startSubIndex, int endSubIndex, bool mouseInView, Vector2 mousePos)
        {
            float viewOffsetY = viewTrackIndex * (ViewStyles.ClipHeight + ViewStyles.ClipVInterval);
            int visableSubTrackCount = track.Foldout ? track.Count : 1;
            float trackHeight = ViewStyles.TrackHeight * visableSubTrackCount - ViewStyles.ClipVInterval;
            Rect rect = new Rect(0, viewOffsetY, view.FrameCount * ViewStyles.FrameWidth, trackHeight);
            GUIRenderHelper.DrawRect(rect, ViewStyles.TrackBGColor);
            if (!track.Foldout)
            {
                //画被折叠的轨道
                for (int i = 1; i < track.Count; ++i)
                {
                    var clip = track.Clips[i].Clip;
                    if (clip.StartFrame > view.VisableFrameEnd || (clip.Length > 0 && clip.StartFrame + clip.Length < view.VisableFrameStart))
                        continue;
                    float offsetY = viewOffsetY;
                    float offsetX = clip.StartFrame * ViewStyles.FrameWidth;
                    int frameCount = clip.Length;
                    if (clip.Length <= 0 || frameCount > (view.FrameCount - clip.StartFrame))
                        frameCount = view.FrameCount - clip.StartFrame;
                    Rect clipRect = new Rect(offsetX, offsetY, ViewStyles.FrameWidth * frameCount, ViewStyles.ClipHeight);
                    GUIRenderHelper.DrawRect(clipRect, ViewStyles.InvalidClipColor, 5, BorderType.All);
                }
            }
            for (int i = 0; i < track.Count; ++i)
            {
                var clip = track.Clips[i].Clip;
                if (i < startSubIndex || i > endSubIndex)
                    continue;
                if (clip.StartFrame > view.VisableFrameEnd || (clip.Length > 0 && clip.StartFrame + clip.Length < view.VisableFrameStart))
                    continue;
                float offsetY = viewOffsetY + i * ViewStyles.TrackHeight;
                float offsetX = clip.StartFrame * ViewStyles.FrameWidth;
                int frameCount = clip.Length;
                if (clip.Length <= 0 || frameCount > (view.FrameCount - clip.StartFrame))
                    frameCount = view.FrameCount - clip.StartFrame;
                //左侧控制区域
                Rect clipLeftCtrlRect = new Rect(offsetX, offsetY, ViewStyles.ClipCtrlWidth, ViewStyles.ClipHeight);
                FrameClipHitPartType dragPart = FrameClipHitPartType.None;
                if (view.DragOperate != null)
                {
                    dragPart = view.DragOperate.GetDragePart(clip);
                }
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
                if (view.IsSlecected(clip))
                {
                    Rect fullRect = new Rect(offsetX, offsetY, ViewStyles.FrameWidth * frameCount, ViewStyles.ClipHeight);
                    GUIRenderHelper.DrawWireRect(fullRect, ViewStyles.SelectClipWireFrameColor, ViewStyles.ClipCtrlWidth, BorderType.All);
                }
            }
        }
    }
}
