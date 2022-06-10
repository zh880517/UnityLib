using UnityEditor;
using UnityEngine;
namespace FrameLine
{
    public static class ViewStyles
    {
        public const float ScrollBarSize = 15;
        public const float FrameWidth = 40;//帧宽度
        public const float FrameBarHeight = 21;//顶部时间条高度
        public const float TrackHeight = 50;//轨道条高度
        public const float TrackInterval = 5;//轨道条之间的间隔
        public const float ToolBarHeight = 20;

        public const float TrackHeadWidth = 200;
        public const float TrackFoldSize = 10;

        public static readonly Color SelectFrameBackGroundColor = new Color32(67, 205, 128, 128);
        public static readonly Color TrackBGColor = new Color32(67, 205, 128, 128);
        public static readonly Color SelectTackBGColor = new Color32(67, 205, 128, 128);
        public static readonly GUIStyle FrameNumStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
    }

}
