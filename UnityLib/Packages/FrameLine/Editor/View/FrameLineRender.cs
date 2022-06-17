using UnityEditor;
using UnityEngine;
namespace FrameLine
{
    public static class FrameLineRender
    {

        public static void DrawTrackHead(FrameLineView view, FrameLineTrack track, int viewTrackIndex, int startSubIndex, int endSubIndex)
        {
            float viewOffsetY = viewTrackIndex * (ViewStyles.ClipHeight + ViewStyles.ClipVInterval);
            int visableSubTrackCount = track.Foldout ? track.SubTrackCount : 1;
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
            int visableSubTrackCount = track.Foldout ? track.SubTrackCount : 1;
            float trackHeight = ViewStyles.TrackHeight * visableSubTrackCount - ViewStyles.ClipVInterval;
            Rect rect = new Rect(0, viewOffsetY, view.FrameCount * ViewStyles.FrameWidth, trackHeight);
            GUIRenderHelper.DrawRect(rect, ViewStyles.TrackBGColor);
            if (!track.Foldout && track.Count > 1)
            {
                foreach (var clipRef in track.Clips)
                {
                    var clip = clipRef.Clip;
                    if (clip.SubTrackIndex < startSubIndex || clip.SubTrackIndex > endSubIndex)
                        continue;
                    if (clip.StartFrame > view.VisableFrameEnd || (clip.Length > 0 && clip.StartFrame + clip.Length < view.VisableFrameStart))
                        continue;
                    if (clip.SubTrackIndex > 0)
                    {
                        float offsetY = viewOffsetY;
                        float offsetX = clip.StartFrame * ViewStyles.FrameWidth;
                        int frameCount = clip.Length;
                        if (clip.Length <= 0 || frameCount > (view.FrameCount - clip.StartFrame))
                            frameCount = view.FrameCount - clip.StartFrame;
                        Rect clipRect = new Rect(offsetX, offsetY, ViewStyles.FrameWidth* frameCount, ViewStyles.ClipHeight);
                        GUIRenderHelper.DrawRect(clipRect, ViewStyles.InvalidClipColor, 5, BorderType.All);
                    }
                }
            }
            foreach (var clipRef in track.Clips)
            {
                var clip = clipRef.Clip;
                if (clip.SubTrackIndex < startSubIndex || clip.SubTrackIndex > endSubIndex)
                    continue;
                if (clip.StartFrame > view.VisableFrameEnd || (clip.Length > 0 && clip.StartFrame + clip.Length < view.VisableFrameStart))
                    continue;
                bool isValid = clip.SubTrackIndex == 0 || track.Foldout;
                if (isValid)
                {
                    float offsetY = viewOffsetY + clip.SubTrackIndex * ViewStyles.TrackHeight;
                    float offsetX = clip.StartFrame * ViewStyles.FrameWidth; 
                    int frameCount = clip.Length;
                    if (clip.Length <= 0 || frameCount > (view.FrameCount - clip.StartFrame))
                        frameCount = view.FrameCount - clip.StartFrame;
                    //左侧控制区域
                    Rect clipLeftCtrlRect = new Rect(offsetX, offsetY, ViewStyles.ClipCtrlWidth, ViewStyles.ClipHeight);
                    Color color = mouseInView && clipLeftCtrlRect.Contains(mousePos) ? ViewStyles.ClipSelectCtrlColor : ViewStyles.ClipCtrlColor;
                    GUIRenderHelper.DrawRect(clipLeftCtrlRect, color, ViewStyles.ClipCtrlWidth, BorderType.Left);
                    //右侧
                    

                    int clipEndFrame = clip.StartFrame + frameCount - 1;
                    Rect clipRightCtrlRect = new Rect((clipEndFrame + 1)*ViewStyles.FrameWidth - ViewStyles.ClipCtrlWidth, 
                        offsetY, 
                        ViewStyles.ClipCtrlWidth, 
                        ViewStyles.ClipHeight);
                    color = mouseInView && clipRightCtrlRect.Contains(mousePos) ? ViewStyles.ClipSelectCtrlColor : ViewStyles.ClipCtrlColor;
                    GUIRenderHelper.DrawRect(clipRightCtrlRect, color, ViewStyles.ClipCtrlWidth, BorderType.Right);
                    //中间区域
                    Rect clipRect = new Rect(clipLeftCtrlRect.xMax, offsetY, clipRightCtrlRect.xMin - clipLeftCtrlRect.xMax, ViewStyles.ClipHeight);
                    GUIRenderHelper.DrawRect(clipRect, ViewStyles.ClipColor);
                    if (view.IsSlecected(clipRef))
                    {
                        Rect fullRect = new Rect(offsetX, offsetY, ViewStyles.FrameWidth * frameCount, ViewStyles.ClipHeight);
                        GUIRenderHelper.DrawWireRect(fullRect, ViewStyles.SelectClipWireFrameColor, ViewStyles.ClipCtrlWidth, BorderType.All);
                    }
                }
            }
        }
    }
}
