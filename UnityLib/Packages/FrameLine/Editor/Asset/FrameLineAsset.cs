using System.Collections.Generic;
using UnityEngine;
namespace FrameLine
{
    public class FrameLineAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public string Comment;
        [SerializeField, HideInInspector]
        private ulong keyIndex;
        [SerializeField, HideInInspector]
        protected List<FrameClip> clips = new List<FrameClip>();
        [SerializeField, HideInInspector]
        protected List<FrameLineTrack> tracks = new List<FrameLineTrack>();
        public int FrameCount;

        public IReadOnlyList<FrameClip> Clips => clips;
        public IReadOnlyList<FrameLineTrack> Tracks => tracks;

        public FrameClip AddClip(int frame, IFrameLineClipData data)
        {
            FrameClip clip = new FrameClip
            {
                ID = ++keyIndex,
                StartFrame = frame,
                Name = GetTypeShowName(data.GetType())
            };
            clip.SetData(data);
            clips.Add(clip);
            var track = GetTrack(clip.TypeGUID);
            if (track.Name == null)
            {
                track.Name = GetTypeShowName(data.GetType());
            }
            clip.TrackID = track.ID;
            track.Add(clip);
            return clip;
        }

        public void RemoveClip(FrameClipRef clip)
        {
            var index = clips.FindIndex(it => it.ID == clip.ID);
            if (index >= 0)
            {
                var exist = clips[index];
                var track = tracks.Find(it => it.ID == exist.TrackID);
                if (track != null)
                {
                    track.Remove(clip);
                    if (track.Count == 0)
                    {
                        tracks.Remove(track);
                    }
                }
                clips.RemoveAt(index);
            }
        }

        public void RemoveTrack(ulong trackID)
        {
            var track = tracks.Find(it => it.ID == trackID);
            if (track != null)
            {
                clips.RemoveAll(it => it.TrackID == trackID);
                tracks.Remove(track);
            }
        }

        public FrameLineTrack FindTrack(ulong trackID)
        {
            return tracks.Find(it => it.ID == trackID);
        }

        protected FrameLineTrack GetTrack(string typeGUID)
        {
            var track = tracks.Find(it => it.TypeGUID == typeGUID);
            if (track == null)
            {
                track = new FrameLineTrack()
                {
                    ID = ++keyIndex,
                    TypeGUID = typeGUID,
                };
                tracks.Add(track);
            }
            return track;
        }

        public static string GetTypeShowName(System.Type type)
        {
            return type.Name;
        }

        public FrameClip Find(ulong id)
        {
            return clips.Find(it => it.ID == id);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            foreach (var track in tracks)
            {
                track.OnAfterDeserialize(this);
            }
        }
    }
}