using UnityEngine;

namespace FrameLine
{
    public static class FrameUtil
    {
        public static int PosToFrame(float pos)
        {
            return Mathf.Max(0, Mathf.FloorToInt(pos / ViewStyles.FrameWidth));
        }
    }
}
