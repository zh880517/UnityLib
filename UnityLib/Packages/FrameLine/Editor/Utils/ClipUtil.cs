using UnityEngine;

namespace FrameLine
{
    public static class ClipUtil
    {
        public static int SortByStartFrame(FrameClipRef a, FrameClipRef b)
        {
            return a.Clip.StartFrame - b.Clip.StartFrame;

        }
        public static int SortByStartFrame(FrameClip a, FrameClip b)
        {
            return a.StartFrame - b.StartFrame;
        }

        public static int GetClipEndFrame(FrameLineAsset asset, FrameClipRef clipRef)
        {
            if (clipRef.Clip.Length > 0)
            {
                return Mathf.Clamp(clipRef.Clip.Length + clipRef.Clip.StartFrame - 1, clipRef.Clip.StartFrame, asset.FrameCount);
            }
            return asset.FrameCount - 1;
        }

        public static bool IsOverlap(FrameClip a, FrameClip b)
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