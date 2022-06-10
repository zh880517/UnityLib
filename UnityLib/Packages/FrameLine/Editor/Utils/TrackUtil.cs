namespace FrameLine
{
    public static class TrackUtil
    {
        public static void UpdateClipTrackIndex(FrameLineTrack track)
        {
            if (track.Count == 0)
                return;
            track.Sort();
            track.Clips[0].Clip.SubTrackIndex = 0;
            int newIndex = 1;
            for (int i=1; i<track.Count; ++i)
            {
                var clip = track.Clips[i].Clip;
                clip.SubTrackIndex = newIndex;
                for (int j=0; j<i; ++j)
                {
                    var preClip = track.Clips[j].Clip;
                    if (preClip.Length > 0 && preClip.StartFrame + preClip.Length <= clip.StartFrame)
                    {
                        clip.SubTrackIndex = preClip.SubTrackIndex;
                        break;
                    }
                }
                if (clip.SubTrackIndex == newIndex)
                {
                    ++newIndex;
                }
            }
            track.SubTrackCount = newIndex;
        }

        public static void UpdateAllTrack(this FrameLineAsset asset)
        {
            foreach (var track in asset.Tracks)
            {
                UpdateClipTrackIndex(track);
            }
        }

        public static int GetVisableTrackCount(this FrameLineAsset asset)
        {

            int count = 0;
            foreach (var track in asset.Tracks)
            {
                count += track.SubTrackCount;
            }
            return count;
        }
    }
}
