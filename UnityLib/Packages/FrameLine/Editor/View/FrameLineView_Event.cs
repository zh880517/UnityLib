using UnityEditor;
using UnityEngine;

namespace FrameLine
{
    public partial class FrameLineView
    {
        public DragOperateBase DragOperate;
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
                e.Use();
                return true;
            }
            if (e.button == 1 && e.type == EventType.MouseUp)
            {
                GenericMenu menu = new GenericMenu();
                OnFrameBarMenue(menu);
                if (menu.GetItemCount() > 0)
                {
                    menu.ShowAsContext();
                }
                e.Use();
                return true;
            }
            return false;
        }

        private bool OnFrameClipsEvent(Event e)
        {
            if (DragOperate != null && e.button == 0)
            {
                //普通的点击也会触发DragOperate,所以这里不能直接返回
                if (e.type == EventType.MouseDrag)
                {
                    DragOperate.Drag(e.mousePosition);
                    return true;
                }
                else if (e.type == EventType.MouseUp)
                {
                    if (DragOperate.HasDraged)
                    {
                        DragOperate.OnDragEnd();
                    }
                    else
                    {
                        bool isMultSelect = (e.modifiers & (EventModifiers.Control | EventModifiers.Command)) != 0;
                        if (!isMultSelect)
                        {
                            var hitTest = HitTest(e.mousePosition);
                            if (hitTest.HitPart == FrameClipHitPartType.Normal || hitTest.HitPart == FrameClipHitPartType.None)
                            {
                                SelectedClips.Clear();
                                if (hitTest.Clip)
                                {
                                    SelectedClips.Add(hitTest.Clip);
                                }
                            }
                        }
                    }
                    DragOperate = null;
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
                                SelectedClips.Clear();
                            return true;
                        case FrameClipHitPartType.Normal:
                            if (isMultSelect)
                            {
                                int selectedIdx = SelectedClips.IndexOf(hitTest.Clip);
                                if (selectedIdx >= 0)
                                {
                                    SelectedClips.RemoveAt(selectedIdx);
                                }
                                else
                                {
                                    SelectedClips.Add(hitTest.Clip);
                                }
                            }
                            else
                            {
                                if (!SelectedClips.Contains(hitTest.Clip))
                                {
                                    SelectedClips.Clear();
                                    SelectedClips.Add(hitTest.Clip);
                                }
                            }
                            DragOperate = new ClipsDragMoveOperate(this, hitTest.Frame);
                            return true;
                        case FrameClipHitPartType.LeftCtrl:
                            DragOperate = new ClipDragStartOperate(this, hitTest.Clip);
                            return true;
                        case FrameClipHitPartType.RightCtrl:
                            DragOperate = new ClipDragEndOperate(this, hitTest.Clip);
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
