namespace FrameLine
{
    public static class TrackUtil
    {
        public static void UpdateAllTrack(this FrameLineAsset asset)
        {
            foreach (var track in asset.Tracks)
            {
                track.Sort();
            }
        }

        public static int GetVisableTrackCount(this FrameLineAsset asset)
        {
            return asset.Clips.Count;
        }
    }
}
