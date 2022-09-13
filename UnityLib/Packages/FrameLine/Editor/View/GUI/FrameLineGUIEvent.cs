using UnityEditor;
using UnityEngine;

namespace FrameLine
{
    [System.Serializable]
    public class FrameLineGUIEvent
    {
        private DragOperateBase dragOperate;
        [SerializeField]
        private FrameLineGUI GUI;

        public FrameLineGUIEvent(FrameLineGUI gui)
        {
            GUI = gui;
        }

        public FrameClipHitPartType GetDragePart(FrameClipRef clipRef)
        {
            if (dragOperate != null)
                return dragOperate.GetDragePart(clipRef);
            return FrameClipHitPartType.None;
        }
        public FrameClipHitResult HitTest(Vector2 point)
        {
            int hitFrame = FrameUtil.PosToFrame(point.x);
            FrameClipHitResult result = new FrameClipHitResult() { Frame = hitFrame, ClickPos = point };

            if (hitFrame < GUI.FrameCount && hitFrame >= 0)
            {
                int hitTrackIndex = Mathf.FloorToInt(point.y / ViewStyles.TrackHeight);
                int preTrackCount = 0;
                for (int i = 0; i < GUI.Asset.Tracks.Count; ++i)
                {
                    var track = GUI.Asset.Tracks[i];
                    int hitSubIndex = hitTrackIndex - preTrackCount;
                    if (hitSubIndex < 0)
                        break;
                    int subTrakCount = track.Foldout ? track.Count : 1;
                    preTrackCount += subTrakCount;
                    if (hitSubIndex >= subTrakCount)
                        continue;

                    var clip = track.Clips[hitSubIndex].Clip;
                    if (clip.StartFrame <= hitFrame)
                    {
                        int endFrame = ClipUtil.GetClipEndFrame(GUI.Asset, clip);
                        if (endFrame >= hitFrame)
                        {
                            float frameOffset = point.x % ViewStyles.FrameWidth;
                            if (frameOffset <= ViewStyles.ClipCtrlWidth && clip.StartFrame == hitFrame)
                            {
                                result.HitPart = FrameClipHitPartType.LeftCtrl;
                            }
                            else if (clip.Length > 0 && endFrame == hitFrame && (frameOffset >= (ViewStyles.FrameWidth - ViewStyles.ClipCtrlWidth)))
                            {
                                result.HitPart = FrameClipHitPartType.RightCtrl;
                            }
                            else
                            {
                                result.HitPart = FrameClipHitPartType.Normal;
                            }
                            result.Clip = clip;
                        }
                    }
                    if (result.Clip)
                        break;
                }
            }
            return result;
        }

        public bool OnTrackHeadEvent(Event e)
        {
            return false;
        }

        public bool OnFrameBarEvent(Event e)
        {
            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
            {
                int selectFrame = Mathf.FloorToInt(e.mousePosition.x / ViewStyles.FrameWidth);
                if (selectFrame >= 0 && selectFrame < GUI.Asset.FrameCount)
                {
                    GUI.CurrentFrame = selectFrame;
                }
                e.Use();
                return true;
            }
            if (e.button == 1 && e.type == EventType.MouseUp)
            {
                GenericMenu menu = new GenericMenu();
                GUI.OnFrameBarMenue(menu);
                if (menu.GetItemCount() > 0)
                {
                    menu.ShowAsContext();
                }
                e.Use();
                return true;
            }
            return false;
        }

        public bool OnFrameClipsEvent(Event e)
        {
            if (dragOperate != null && e.button == 0)
            {
                //普通的点击也会触发DragOperate,所以这里不能直接返回
                if (e.type == EventType.MouseDrag)
                {
                    dragOperate.Drag(e.mousePosition);
                    return true;
                }
                else if (e.type == EventType.MouseUp)
                {
                    if (dragOperate.HasDraged)
                    {
                        dragOperate.OnDragEnd();
                    }
                    else
                    {
                        bool isMultSelect = (e.modifiers & (EventModifiers.Control | EventModifiers.Command)) != 0;
                        if (!isMultSelect)
                        {
                            var hitTest = HitTest(e.mousePosition);
                            if (hitTest.HitPart == FrameClipHitPartType.Normal || hitTest.HitPart == FrameClipHitPartType.None)
                            {
                                GUI.SelectedClips.Clear();
                                if (hitTest.Clip)
                                {
                                    GUI.SelectedClips.Add(hitTest.Clip);
                                }
                            }
                        }
                    }
                    dragOperate = null;
                    return true;
                }
            }
            if (e.button == 0)
            {
                bool isMultSelect = (e.modifiers & (EventModifiers.Control | EventModifiers.Command)) != 0;
                if (e.type == EventType.MouseDown)
                {
                    var hitTest = HitTest(e.mousePosition);
                    switch (hitTest.HitPart)
                    {
                        case FrameClipHitPartType.None:
                            if (!isMultSelect)
                                GUI.SelectedClips.Clear();
                            return true;
                        case FrameClipHitPartType.Normal:
                            if (isMultSelect)
                            {
                                int selectedIdx = GUI.SelectedClips.IndexOf(hitTest.Clip);
                                if (selectedIdx >= 0)
                                {
                                    GUI.SelectedClips.RemoveAt(selectedIdx);
                                }
                                else
                                {
                                    GUI.SelectedClips.Add(hitTest.Clip);
                                }
                            }
                            else
                            {
                                if (!GUI.SelectedClips.Contains(hitTest.Clip))
                                {
                                    GUI.SelectedClips.Clear();
                                    GUI.SelectedClips.Add(hitTest.Clip);
                                }
                            }
                            dragOperate = new ClipsDragMoveOperate(GUI, hitTest.Frame);
                            return true;
                        case FrameClipHitPartType.LeftCtrl:
                            dragOperate = new ClipDragStartOperate(GUI, hitTest.Clip);
                            return true;
                        case FrameClipHitPartType.RightCtrl:
                            dragOperate = new ClipDragEndOperate(GUI, hitTest.Clip);
                            return true;
                        default:
                            break;
                    }
                }
            }
            if (e.button == 1)
            {
                if (e.type == EventType.MouseDown)
                {
                }
                else if (e.type == EventType.MouseUp)
                {
                }
            }
            if (e.type == EventType.KeyDown)
            {

            }
            return false;
        }
    }
}
