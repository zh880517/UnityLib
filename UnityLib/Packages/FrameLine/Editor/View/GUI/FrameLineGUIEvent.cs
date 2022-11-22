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

        public FrameActionHitPartType GetDragePart(FrameActionRef actionRef)
        {
            if (dragOperate != null)
                return dragOperate.GetDragePart(actionRef);
            return FrameActionHitPartType.None;
        }
        public FrameActionHitResult HitTest(Vector2 point)
        {
            int hitFrame = FrameLineUtil.PosToFrame(point.x);
            FrameActionHitResult result = new FrameActionHitResult() { Frame = hitFrame, ClickPos = point };

            if (hitFrame < GUI.FrameCount && hitFrame >= 0)
            {
                int hitTrackIndex = Mathf.FloorToInt(point.y / ViewStyles.TrackHeight);
                int preTrackCount = 0;
                for (int i = 0; i < GUI.Tracks.Count; ++i)
                {
                    var track = GUI.Tracks[i];
                    int hitSubIndex = hitTrackIndex - preTrackCount;
                    if (hitSubIndex < 0)
                        break;
                    int subTrakCount = track.Foldout ? track.Count : 1;
                    preTrackCount += subTrakCount;
                    if (hitSubIndex >= subTrakCount)
                        continue;

                    var clip = track.Actions[hitSubIndex].Action;
                    if (clip.StartFrame <= hitFrame)
                    {
                        int endFrame = FrameActionUtil.GetClipEndFrame(GUI.Asset, clip);
                        if (endFrame >= hitFrame)
                        {
                            float frameOffset = point.x % ViewStyles.FrameWidth;
                            if (frameOffset <= ViewStyles.ClipCtrlWidth && clip.StartFrame == hitFrame)
                            {
                                result.HitPart = FrameActionHitPartType.LeftCtrl;
                            }
                            else if (clip.Length > 0 && endFrame == hitFrame && (frameOffset >= (ViewStyles.FrameWidth - ViewStyles.ClipCtrlWidth)))
                            {
                                result.HitPart = FrameActionHitPartType.RightCtrl;
                            }
                            else
                            {
                                result.HitPart = FrameActionHitPartType.Normal;
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
                if (selectFrame >= 0 && selectFrame < GUI.Group.FrameCount)
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
                            if (hitTest.HitPart == FrameActionHitPartType.Normal || hitTest.HitPart == FrameActionHitPartType.None)
                            {
                                GUI.SelectedActions.Clear();
                                if (hitTest.Clip)
                                {
                                    GUI.SelectedActions.Add(hitTest.Clip);
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
                        case FrameActionHitPartType.None:
                            if (!isMultSelect)
                                GUI.SelectedActions.Clear();
                            return true;
                        case FrameActionHitPartType.Normal:
                            if (isMultSelect)
                            {
                                int selectedIdx = GUI.SelectedActions.IndexOf(hitTest.Clip);
                                if (selectedIdx >= 0)
                                {
                                    GUI.SelectedActions.RemoveAt(selectedIdx);
                                }
                                else
                                {
                                    GUI.SelectedActions.Add(hitTest.Clip);
                                }
                            }
                            else
                            {
                                if (!GUI.SelectedActions.Contains(hitTest.Clip))
                                {
                                    GUI.SelectedActions.Clear();
                                    GUI.SelectedActions.Add(hitTest.Clip);
                                }
                            }
                            dragOperate = new ActionsDragMoveOperate(GUI, hitTest.Frame);
                            return true;
                        case FrameActionHitPartType.LeftCtrl:
                            dragOperate = new ActionDragStartOperate(GUI, hitTest.Clip);
                            return true;
                        case FrameActionHitPartType.RightCtrl:
                            dragOperate = new ActionDragEndOperate(GUI, hitTest.Clip);
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
