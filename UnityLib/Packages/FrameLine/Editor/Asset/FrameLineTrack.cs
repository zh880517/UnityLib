using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    [System.Serializable]
    public class FrameLineTrack
    {
        public ulong ID;
        public string Name;
        public string Comment;
        public string TypeGUID;
        public bool Foldout = true;//
        public int SubTrackCount;//显示素有Clip需要的轨道数量
        [SerializeField]
        private List<FrameClipRef> clips = new List<FrameClipRef>();

        public IReadOnlyList<FrameClipRef> Clips => clips;

        public int Count => clips.Count;

        public void Add(FrameClipRef clip)
        {
            if (!clips.Contains(clip))
            {
                clips.Add(clip);
            }
        }

        public void Remove(FrameClipRef clip)
        {
            clips.Remove(clip);
        }

        public void Sort()
        {
            clips.Sort(ClipUtil.SortByStartFrame);
        }


        public void OnAfterDeserialize(FrameLineAsset asset)
        {
            for (int i=0; i<clips.Count; ++i)
            {
                var clipRef = clips[i];
                clipRef.Clip = asset.Find(clipRef.ID);
            }
        }
    }
}