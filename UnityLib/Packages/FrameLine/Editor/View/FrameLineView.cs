using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace FrameLine
{
    public class FrameLineView : ScriptableObject
    {
        public FrameLineAsset Asset;
        public List<FrameClipRef> SelectedClips = new List<FrameClipRef>();
        public int CurrentFrame;
        [SerializeField]
        private Vector2 scrollPos;

        //滚动区域看见信息
        public int VisableFrameStart { get; private set; }
        public int VisableFrameEnd { get; private set; }
        public int VisableTrackStart { get; private set; }
        public int VisableTrackEnd { get; private set; }

        public bool OnDraw(Vector2 size)
        {
            int showTrackCount = TrackUtil.GetVisableTrackCount(Asset);
            float frameHeight = showTrackCount * (ViewStyles.TrackHeight + ViewStyles.TrackInterval) + ViewStyles.TrackInterval;
            float framWidth = Asset.FrameCount * ViewStyles.FrameWidth + 10;

            //滚动位置
            float xOffset = scrollPos.x * framWidth;
            float yOffset = scrollPos.y * frameHeight;
            VisableFrameStart = Mathf.FloorToInt(xOffset / ViewStyles.FrameWidth);
            VisableTrackStart = Mathf.FloorToInt(yOffset / (ViewStyles.FrameWidth + ViewStyles.TrackInterval));

            //轨道头部
            Rect trackHeadRect = new Rect(0, 0, ViewStyles.TrackHeadWidth, size.y - ViewStyles.ScrollBarSize);
            bool rePaint = false;
            using (new GUILayout.AreaScope(trackHeadRect))
            {
                //轨道头部按钮区域
                using (new GUILayout.AreaScope(new Rect(0, 0, trackHeadRect.width, ViewStyles.FrameBarHeight), "", EditorStyles.toolbar))
                {
                    using(new GUILayout.HorizontalScope())
                    {
                        DrawToolBar();
                    }
                }
                Rect rect = new Rect(0, ViewStyles.FrameBarHeight, trackHeadRect.width, trackHeadRect.height - ViewStyles.FrameBarHeight);
                VisableTrackEnd = Mathf.CeilToInt((rect.height + yOffset - VisableTrackStart * (ViewStyles.TrackHeight + ViewStyles.TrackInterval)) / (ViewStyles.TrackHeight + ViewStyles.TrackInterval));
                //轨道头部
                using (new GUI.ClipScope(rect))
                {
                    //滚动位置
                    Rect viewRect = new Rect(0, -yOffset, trackHeadRect.width, frameHeight);
                    using(new GUILayout.AreaScope(viewRect))
                    {
                        var e = Event.current;
                        if (viewRect.Contains(e.mousePosition))
                        {
                            rePaint |= OnTrackHeadEvent(e);
                        }

                        DrawTrackHead();
                    }
                }
            }
            //轨道区域大小
            Vector2 trackAreaSize = new Vector2(framWidth, frameHeight);
            //轨道区域
            Rect frameRect = new Rect(ViewStyles.TrackHeadWidth, 0, size.x - ViewStyles.TrackHeadWidth - ViewStyles.ScrollBarSize, size.y - ViewStyles.ScrollBarSize);
            //轨道在窗口中显示的大小
            Vector2 trackAreaInViewSize = new Vector2(frameRect.width, frameRect.height - ViewStyles.FrameBarHeight);
            using(new GUI.ClipScope(frameRect))
            {
                VisableFrameEnd = Mathf.CeilToInt((frameRect.width + xOffset - VisableFrameStart * ViewStyles.FrameWidth) / ViewStyles.FrameWidth) + VisableFrameStart;
                //画帧标号背景条
                GUI.Box(new Rect(0, 0, frameRect.width, ViewStyles.FrameBarHeight), "");
                //帧长度区域|<-所有帧->|，水平滚动区域
                using (new GUILayout.AreaScope(new Rect(-xOffset, 0, framWidth, frameRect.height)))
                {
                    DrawFrameLineBackGround(new Rect(new Vector2(xOffset, 0), frameRect.size));
                    {
                        var e = Event.current;
                        if (e.mousePosition.y < ViewStyles.FrameBarHeight)
                        {
                            rePaint |= OnFrameBarEvent(e);
                        }
                    }
                    //轨道条区域
                    using (new GUI.ClipScope(new Rect(0, ViewStyles.FrameBarHeight, framWidth, frameHeight)))
                    {
                        Rect trackViewRect = new Rect(xOffset, -yOffset, framWidth, frameHeight);
                        using (new GUILayout.AreaScope(trackViewRect))
                        {
                            var e = Event.current;
                            if (trackViewRect.Contains(e.mousePosition))
                            {
                                rePaint |= OnFrameClipsEvent(e);
                            }
                            DrawFrameClips();
                        }
                    }
                }
            }
            Rect vBarRect = new Rect(size.x - ViewStyles.ScrollBarSize, 0, ViewStyles.ScrollBarSize, size.y - ViewStyles.ScrollBarSize);
            float vSize = Mathf.Clamp01(trackAreaInViewSize.y/trackAreaSize.y);
            scrollPos.y = GUI.VerticalScrollbar(vBarRect, scrollPos.y, vSize, 0, 1);
            Rect hBarRect = new Rect(ViewStyles.TrackHeadWidth, size.y - ViewStyles.ScrollBarSize, size.x - ViewStyles.TrackHeadWidth - ViewStyles.ScrollBarSize, ViewStyles.ScrollBarSize);
            float hSize = Mathf.Clamp01(trackAreaInViewSize.x / trackAreaSize.x);
            scrollPos.x = GUI.HorizontalScrollbar(hBarRect, scrollPos.x, hSize, 0, 1);
            return rePaint;
        }

        private bool OnTrackHeadEvent(Event e)
        {
            return false;
        }

        private bool OnFrameBarEvent(Event e)
        {
            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
            {

                int selectFrame = Mathf.FloorToInt(e.mousePosition.x / ViewStyles.FrameWidth);
                if (selectFrame >= 0 && selectFrame < Asset.FrameCount)
                {
                    CurrentFrame = selectFrame;
                }
                return true;
            }
            if (e.button == 1 && e.type == EventType.MouseUp)
            {
                //创建菜单
                return true;
            }
            return false;
        }

        private bool OnFrameClipsEvent(Event e)
        {
            return false;
        }

        private void DrawTrackHead()
        {
            int trackIndex = 0;
            foreach (var track in Asset.Tracks)
            {
                if (track.Count == 0)
                    continue;
                if (trackIndex > VisableTrackEnd)
                    return;
                int trackVisableCount = (track.Foldout ? track.Count : 1);
                do 
                {
                    if (trackIndex + trackVisableCount <= VisableTrackStart)
                        break;
                    int startIndex = Mathf.Clamp(trackIndex, VisableFrameStart, VisableTrackEnd) - trackIndex;
                    int endIndex = Mathf.Clamp(trackIndex + trackVisableCount, VisableFrameStart, VisableTrackEnd) - trackIndex;
                    FrameLineRender.DrawTrackHead(this, track, trackIndex, startIndex, endIndex);
                } while (false);
                trackIndex += trackVisableCount;
            }
        }

        private void DrawFrameClips()
        {
            int trackIndex = 0;
            foreach (var track in Asset.Tracks)
            {
                if (track.Count == 0)
                    continue;
                if (trackIndex > VisableTrackEnd)
                    return;
                int trackVisableCount = (track.Foldout ? track.Count : 1);
                do
                {
                    if (trackIndex + trackVisableCount <= VisableTrackStart)
                        break;
                    int startIndex = Mathf.Clamp(trackIndex, VisableFrameStart, VisableTrackEnd) - trackIndex;
                    int endIndex = Mathf.Clamp(trackIndex + trackVisableCount, VisableFrameStart, VisableTrackEnd) - trackIndex;
                    FrameLineRender.DrawTrack(this, track, trackIndex, startIndex, endIndex);
                } while (false);
                trackIndex += trackVisableCount;
            }
        }

        private void DrawFrameLineBackGround(Rect showRect)
        {
            using(new Handles.DrawingScope(new Color(0.5f, 0.5f, 0.5f, 0.5f)))
            {
                int startIndex = Mathf.Clamp(VisableFrameStart, 0, Asset.FrameCount);
                int endIndex = Mathf.Clamp(VisableFrameEnd, 0, Asset.FrameCount);
                for (int i = startIndex; i <= endIndex; ++i)
                {
                    float xPos = i * ViewStyles.FrameWidth;
                    Handles.DrawLine(new Vector2(xPos, showRect.yMin), new Vector2(xPos, showRect.yMax));
                    GUI.Label(new Rect(xPos, 0, ViewStyles.FrameWidth, ViewStyles.FrameBarHeight), i.ToString(), ViewStyles.FrameNumStyle);
                }
            }
            if (CurrentFrame >= VisableFrameStart && CurrentFrame <= VisableFrameEnd)
            {
                Rect rect = new Rect(CurrentFrame * ViewStyles.FrameWidth, 0, ViewStyles.FrameWidth, showRect.height);
                GUIRenderHelper.DrawArea(rect, ViewStyles.SelectFrameBackGroundColor, 5, BorderType.Top);
            }
        }

        protected virtual void DrawToolBar()
        {
        }

    }
}