namespace FrameLine
{
    public static class TrackUtil
    {
        public static void UpdateAllTrack(this FrameLineGUI gui)
        {
            foreach (var track in gui.Tracks)
            {
                track.Sort();
            }
        }

        public static void RebuildTrack(this FrameLineGUI gui)
        {
            foreach (var track in gui.Tracks)
            {
                track.Clips.Clear();
            }
            var group = gui.Asset.FindGroup(gui.GroupId);
            foreach (var clip in group.Clips)
            {
                gui.OnAddClip(clip);
            }
            for (int i=gui.Tracks.Count-1; i>=0; --i)
            {
                var track = gui.Tracks[i];
                if (track.Count == 0)
                {
                    gui.Tracks.RemoveAt(i);
                }
            }
            gui.UpdateAllTrack();
        }
    }
}
