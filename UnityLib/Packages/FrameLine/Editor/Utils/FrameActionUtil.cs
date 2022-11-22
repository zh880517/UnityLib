using UnityEngine;

namespace FrameLine
{
    public static class FrameActionUtil
    {
        public static int SortByStartFrame(FrameActionRef a, FrameActionRef b)
        {
            return a.Action.StartFrame - b.Action.StartFrame;

        }
        public static int SortByStartFrame(FrameAction a, FrameAction b)
        {
            return a.StartFrame - b.StartFrame;
        }

        public static int GetClipEndFrame(FrameLineAsset asset, FrameActionRef clipRef)
        {
            var group = asset.FindGroup(clipRef.Action.GroupId);
            if (clipRef.Action.Length > 0)
            {
                return Mathf.Clamp(clipRef.Action.Length + clipRef.Action.StartFrame - 1, clipRef.Action.StartFrame, group.FrameCount);
            }
            return group.FrameCount - 1;
        }

        public static bool IsOverlap(FrameAction a, FrameAction b)
        {
            if (a.StartFrame <= b.StartFrame)
            {
                if (a.Length <= 0 || a.StartFrame + a.Length > b.StartFrame)
                {
                    return true;
                }
            }
            if (b.StartFrame <= a.StartFrame)
            {
                if (b.Length <= 0 || b.StartFrame + b.Length > a.StartFrame)
                {
                    return true;
                }
            }
            return false;
        }
    }
}