using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    [System.Serializable]
    public class FrameClipGroup
    {
        public string Comment;
        public int GroupId;
        public string Name;
        public int FrameCount;
        [HideInInspector]
        public List<FrameClip> Clips = new List<FrameClip>();
    }
}
