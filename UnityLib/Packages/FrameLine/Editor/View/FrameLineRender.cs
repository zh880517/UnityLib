using UnityEngine;
namespace FrameLine
{
    public static class FrameLineRender
    {
        public static void DrawTrackHead(FrameLineView view, FrameLineTrack tack, int viewTrackIndex, int startSubIndex, int endSubIndex)
        {

        }
        public static void DrawTrack(FrameLineView view, FrameLineTrack tack, int viewTrackIndex, int startSubIndex, int endSubIndex)
        {
            //float viewOffsetX = view.VisableFrameStart * ViewStyles.FrameWidth;
            //float viewOffsetY = viewTrackIndex * (ViewStyles.TrackHeight + ViewStyles.TrackInterval);
            //float viewWidth = (view.VisableFrameEnd - view.VisableFrameStart + 1) * ViewStyles.FrameWidth;
            //Rect showRect = new Rect(viewOffsetX, viewOffsetY, viewWidth);
            //GUIRenderHelper.DrawArea()
        }
    }
}
