using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace FrameLine
{
    public class FrameLineView : ScriptableObject
    {
        public FrameLineAsset Asset;
        public List<FrameClipRef> SelectedClips = new List<FrameClipRef>();
        public int CurrentFrame;
        [SerializeField]
        private Vector2 scrollPos = new Vector2(0.5f, 0.5f);

        public bool OnDraw(Vector2 size)
        {
            int showTrackCount = TrackUtil.GetVisableTrackCount(Asset);
            float frameHeight = showTrackCount * (ViewStyles.TrackHeight + ViewStyles.TrackInterval) + ViewStyles.TrackInterval;
            float framWidth = Asset.FrameCount * ViewStyles.FrameWidth + 10;
            //轨道头部
            Rect trackHeadRect = new Rect(0, 0, ViewStyles.TrackHeadWidth, size.y - ViewStyles.ScrollBarSize);
            bool rePaint = false;
            using (new GUILayout.AreaScope(trackHeadRect))
            {
                //轨道头部按钮区域
                using (new GUILayout.AreaScope(new Rect(0, 0, trackHeadRect.width, ViewStyles.FrameBarHeight), "", EditorStyles.toolbar))
                {

                }
                Rect rect = new Rect(0, ViewStyles.FrameBarHeight, trackHeadRect.width, trackHeadRect.height - ViewStyles.FrameBarHeight);
                using(new GUI.ClipScope(rect))
                {
                    //轨道头部
                    using (new GUI.ClipScope(rect))
                    {
                        //滚动位置
                        float yOffset = -scrollPos.y * rect.height;
                        Rect viewRect = new Rect(0, yOffset, trackHeadRect.width, frameHeight);
                        using(new GUILayout.AreaScope(viewRect))
                        {
                            var e = Event.current;
                            if (viewRect.Contains(e.mousePosition))
                            {
                                rePaint |= OnTrackHeadEvent(e);
                            }

                            DrawTrackHead(size, new Rect(new Vector2(0, -yOffset), rect.size));
                        }
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
                //画帧标号背景条
                GUI.Box(new Rect(0, 0, frameRect.width, ViewStyles.FrameBarHeight), "");
                //滚动位置
                float xOffset = -scrollPos.x * framWidth;
                float yOffset = -scrollPos.y * frameHeight;
                //帧长度区域|<-所有帧->|，水平滚动区域
                using(new GUILayout.AreaScope(new Rect(xOffset, 0, framWidth, frameRect.height)))
                {
                    DrawFrameLineBackGround(new Rect(new Vector2(-xOffset, 0), frameRect.size));
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
                        Rect trackViewRect = new Rect(xOffset, yOffset, framWidth, frameHeight);
                        using (new GUILayout.AreaScope(trackViewRect))
                        {
                            var e = Event.current;
                            if (trackViewRect.Contains(e.mousePosition))
                            {
                                rePaint |= OnFrameClipsEvent(e);
                            }
                            DrawFrameClips(trackViewRect.size, new Rect(-xOffset, -yOffset, frameRect.width, frameRect.height - ViewStyles.FrameBarHeight));
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
                CurrentFrame = Mathf.FloorToInt(e.mousePosition.x / ViewStyles.FrameWidth);
                return true;
            }
            return false;
        }

        private bool OnFrameClipsEvent(Event e)
        {
            return false;
        }

        private void DrawTrackHead(Vector2 size, Rect showRect)
        {
        }

        private void DrawFrameClips(Vector2 size, Rect showRect)
        {

        }

        private void DrawFrameLineBackGround(Rect showRect)
        {
            int visableFrameStart = Mathf.FloorToInt(showRect.x / ViewStyles.FrameWidth);
            int visableFrameEnd = Mathf.Clamp(Mathf.CeilToInt(showRect.xMax / ViewStyles.FrameWidth), visableFrameStart, Asset.FrameCount);
            using(new Handles.DrawingScope(new Color(0.5f, 0.5f, 0.5f, 0.5f)))
            {
                for (int i = visableFrameStart; i <= visableFrameEnd; ++i)
                {
                    float xPos = i * ViewStyles.FrameWidth;
                    Handles.DrawLine(new Vector2(xPos, showRect.yMin), new Vector2(xPos, showRect.yMax));
                    GUI.Label(new Rect(xPos, 0, ViewStyles.FrameWidth, ViewStyles.FrameBarHeight), i.ToString(), ViewStyles.FrameNumStyle);
                }
            }
            if (CurrentFrame >= visableFrameStart && CurrentFrame <= visableFrameEnd)
            {
                Rect rect = new Rect(CurrentFrame * ViewStyles.FrameWidth, 0, ViewStyles.FrameWidth, showRect.height);
                GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, ViewStyles.SelectFrameBackGroundColor, Vector4.zero, new Vector4(5, 5, 0, 0));
            }
        }
    }
}