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
    }
}