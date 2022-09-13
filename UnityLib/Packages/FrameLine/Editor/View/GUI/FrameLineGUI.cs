using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace FrameLine
{
    public enum FrameClipHitPartType
    {
        None,
        Normal,
        LeftCtrl,
        RightCtrl,
    }
    public struct FrameClipHitResult
    {
        public Vector2 ClickPos;
        public FrameClipRef Clip;
        public FrameClipHitPartType HitPart;
        public int Frame;
    }
    public class FrameLineGUI : ScriptableObject, ISerializationCallbackReceiver
    {

        public FrameLineAsset Asset;
        public EditorWindow Window;
        public FrameLineGUIEvent Event;
        public List<FrameClipRef> SelectedClips = new List<FrameClipRef>();
        public int CurrentFrame;
        public Vector2 ScrollPos;
        public int FrameCount => Asset.FrameCount;

        //滚动区域看见信息
        public int VisableFrameStart { get; private set; }
        public int VisableFrameEnd { get; private set; }
        public int VisableTrackStart { get; private set; }
        public int VisableTrackEnd { get; private set; }

        private void OnEnable()
        {
            if (Event == null)
                Event = new FrameLineGUIEvent(this);
        }

        public bool IsSlecected(FrameClipRef clipRef)
        {
            return SelectedClips.Contains(clipRef);
        }

        protected virtual void DrawToolBar()
        {
        }

        public virtual void RegistUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(Asset, name);
            Undo.RegisterCompleteObjectUndo(this, name);
            Undo.RegisterCompleteObjectUndo(Window, name);
            EditorUtility.SetDirty(Asset);
        }

        public virtual void OnFrameBarMenue(GenericMenu menu)
        {

        }

        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
            //反序列化处理
            for (int i = 0; i < SelectedClips.Count; ++i)
            {
                var clipRef = SelectedClips[i];
                clipRef.Clip = Asset.Find(clipRef.ID);
            }
        }

        public bool OnDraw(Vector2 size)
        {
            int showTrackCount = TrackUtil.GetVisableTrackCount(Asset);
            float frameHeight = showTrackCount * (ViewStyles.ClipHeight + ViewStyles.ClipVInterval) + ViewStyles.ClipVInterval;
            float framWidth = Asset.FrameCount * ViewStyles.FrameWidth + 10;

            //滚动位置
            float xOffset = ScrollPos.x * framWidth;
            float yOffset = ScrollPos.y * frameHeight;
            VisableFrameStart = Mathf.FloorToInt(xOffset / ViewStyles.FrameWidth);
            VisableTrackStart = Mathf.FloorToInt(yOffset / (ViewStyles.FrameWidth + ViewStyles.ClipVInterval));

            //轨道头部
            Rect trackHeadRect = new Rect(0, 0, ViewStyles.TrackHeadWidth, size.y - ViewStyles.ScrollBarSize);
            bool rePaint = false;
            using (new GUILayout.AreaScope(trackHeadRect))
            {
                //轨道头部按钮区域
                using (new GUILayout.AreaScope(new Rect(0, 0, trackHeadRect.width, ViewStyles.FrameBarHeight), "", EditorStyles.toolbar))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        DrawToolBar();
                    }
                }
                Rect rect = new Rect(0, ViewStyles.FrameBarHeight, trackHeadRect.width, trackHeadRect.height - ViewStyles.FrameBarHeight);
                VisableTrackEnd = Mathf.CeilToInt((rect.height + yOffset - VisableTrackStart * (ViewStyles.ClipHeight + ViewStyles.ClipVInterval)) / (ViewStyles.ClipHeight + ViewStyles.ClipVInterval));
                //轨道头部
                using (new GUI.ClipScope(rect))
                {
                    //滚动位置
                    Rect viewRect = new Rect(0, -yOffset, trackHeadRect.width, frameHeight);
                    using (new GUILayout.AreaScope(viewRect))
                    {
                        var e = UnityEngine.Event.current;
                        if (viewRect.Contains(e.mousePosition))
                        {
                            rePaint |= Event.OnTrackHeadEvent(e);
                        }
                        FrameLineDrawer.DrawTrackHead(this);
                    }
                }
            }
            //轨道区域大小
            Vector2 trackAreaSize = new Vector2(framWidth, frameHeight);
            //轨道区域
            Rect frameRect = new Rect(ViewStyles.TrackHeadWidth, 0, size.x - ViewStyles.TrackHeadWidth - ViewStyles.ScrollBarSize, size.y - ViewStyles.ScrollBarSize);
            //轨道在窗口中显示的大小
            Vector2 trackAreaInViewSize = new Vector2(frameRect.width, frameRect.height - ViewStyles.FrameBarHeight);
            using (new GUI.ClipScope(frameRect))
            {
                VisableFrameEnd = Mathf.CeilToInt((frameRect.width + xOffset - VisableFrameStart * ViewStyles.FrameWidth) / ViewStyles.FrameWidth) + VisableFrameStart;
                //画帧标号背景条
                GUI.Box(new Rect(0, 0, frameRect.width, ViewStyles.FrameBarHeight), "");
                //帧长度区域|<-所有帧->|，水平滚动区域
                using (new GUILayout.AreaScope(new Rect(-xOffset, 0, framWidth, frameRect.height)))
                {
                    FrameLineDrawer.DrawFrameLineBackGround(this, new Rect(new Vector2(xOffset, 0), frameRect.size));
                    {
                        var e = UnityEngine.Event.current;
                        if (e.mousePosition.y < ViewStyles.FrameBarHeight)
                        {
                            rePaint |= Event.OnFrameBarEvent(e);
                        }
                    }
                    //轨道条区域
                    using (new GUI.ClipScope(new Rect(0, ViewStyles.FrameBarHeight, framWidth, frameHeight)))
                    {
                        Rect trackViewRect = new Rect(0, -yOffset, framWidth, frameHeight);
                        using (new GUILayout.AreaScope(trackViewRect))
                        {
                            var e = UnityEngine.Event.current;
                            Vector2 mousePos = e.mousePosition;
                            bool mouseInView = trackViewRect.Contains(mousePos);
                            if (mouseInView)
                            {
                                if (Event.OnFrameClipsEvent(e))
                                {
                                    e.Use();
                                    rePaint = true;
                                }
                            }
                            FrameLineDrawer.DrawFrameClips(this, mouseInView, mousePos);
                        }
                    }
                }
            }
            Rect vBarRect = new Rect(size.x - ViewStyles.ScrollBarSize, 0, ViewStyles.ScrollBarSize, size.y - ViewStyles.ScrollBarSize);
            float vSize = Mathf.Clamp01(trackAreaInViewSize.y / trackAreaSize.y);
            ScrollPos.y = GUI.VerticalScrollbar(vBarRect, ScrollPos.y, vSize, 0, 1);
            Rect hBarRect = new Rect(ViewStyles.TrackHeadWidth, size.y - ViewStyles.ScrollBarSize, size.x - ViewStyles.TrackHeadWidth - ViewStyles.ScrollBarSize, ViewStyles.ScrollBarSize);
            float hSize = Mathf.Clamp01(trackAreaInViewSize.x / trackAreaSize.x);
            ScrollPos.x = GUI.HorizontalScrollbar(hBarRect, ScrollPos.x, hSize, 0, 1);
            return rePaint;
        }
    }
}