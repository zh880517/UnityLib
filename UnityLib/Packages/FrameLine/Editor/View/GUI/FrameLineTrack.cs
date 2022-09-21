using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    [System.Serializable]
    public class FrameLineTrack
    {
        public string Name;
        public string Comment;
        public string TypeGUID;
        public bool Foldout = true;//未被折叠
        public List<FrameClipRef> Clips = new List<FrameClipRef>();

        public int Count => Clips.Count;

        public void Add(FrameClipRef clip)
        {
            if (!Clips.Contains(clip))
            {
                Clips.Add(clip);
            }
        }

        public bool Remove(FrameClipRef clip)
        {
            for (int i =0; i< Clips.Count; ++i)
            {
                if (Clips[i] == clip)
                {
                    Clips.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }


        public void Sort()
        {
            Clips.Sort(ClipUtil.SortByStartFrame);
        }


        public void OnAfterDeserialize(FrameLineAsset asset)
        {
            for (int i=0; i< Clips.Count; ++i)
            {
                var clipRef = Clips[i];
                clipRef.Clip = asset.Find(clipRef.ID);
                Clips[i] = clipRef;
            }
        }
    }
}