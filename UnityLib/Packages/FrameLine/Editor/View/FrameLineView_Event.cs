using UnityEngine;

namespace FrameLine
{
    public partial class FrameLineView
    {
        protected DragOperateBase dragOperate;
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
            if (dragOperate != null)
            {
            }
            if (e.button == 0)
            {

            }
            return false;
        }

    }
}
