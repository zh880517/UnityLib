using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    public class FrameLineScene : ScriptableObject
    {
        public FrameLineAsset Asset;
        public ulong LastKeyIndex;
        public int GroupId;
        [SerializeField]
        private List<FrameClipPlayable> Playables = new List<FrameClipPlayable>();
        [SerializeField]
        protected SceneGameObjectPool objectPool;
        public SceneGameObjectPool ObjectPool => objectPool;

        protected virtual void Awake()
        {
            objectPool = new SceneGameObjectPool();
        }

        private void UpdatePlayable(int frameIndex)
        {
            if (LastKeyIndex != Asset.KeyIndex)
            {
                LastKeyIndex = Asset.KeyIndex;
                var group = Asset.FindGroup(GroupId);
                foreach (var clip in group.Clips)
                {
                    if (Playables.Exists(it => it.ClipId == clip.ID))
                        continue;
                    OnClipAdd(clip, frameIndex);
                }
            }
        }

        public virtual void OnSceneGUI(int frameIndex)
        {
            UpdatePlayable(frameIndex);
            for (int i = 0; i < Playables.Count; ++i)
            {
                var playable = Playables[i];
                FrameClip clip = Asset.Find(playable.ClipId);
                if (clip != null)
                {
                    DestroyImmediate(playable);
                    Playables.RemoveAt(i);
                    --i;
                    continue;
                }
                bool isActive = frameIndex <= clip.StartFrame && (clip.Length <= 0 || clip.StartFrame + clip.Length < frameIndex);
                isActive |= ((int)(playable.ClipId >> 32) == GroupId);
                if (isActive)
                {
                    playable.OnSceneGUI(clip.Data, frameIndex);
                }
            }
        }

        public void OnSimatle(int frameIndex)
        {
            UpdatePlayable(frameIndex);
            for (int i=0;i< Playables.Count; ++i)
            {
                var playable = Playables[i];
                FrameClip clip = Asset.Find(playable.ClipId);
                if (clip == null)
                {
                    DestroyImmediate(playable);
                    Playables.RemoveAt(i);
                    --i;
                    continue;
                }
                bool isActive = frameIndex <= clip.StartFrame && (clip.Length <= 0 || clip.StartFrame + clip.Length < frameIndex);
                isActive |= ((int)(playable.ClipId >> 32) == GroupId);
                if (isActive != playable.Active)
                {
                    playable.Active = isActive;
                    if (isActive)
                        playable.OnActive(clip.Data);
                    else
                        playable.OnDeActive(clip.Data);
                }
                if (isActive)
                    playable.OnSimatle(clip.Data, frameIndex - clip.StartFrame);
            }
        }

        private void OnClipAdd(FrameClip clip, int frameIndex)
        {
            var type = GetPlayableType(clip.Data.GetType());
            if (type == null)
                return;
            var playable = CreateInstance(type) as FrameClipPlayable;
            if (playable == null)
                return;
            playable.hideFlags = HideFlags.HideAndDontSave;
            playable.Scene = this;
            playable.ClipId = clip.ID;
            Playables.Add(playable);
            playable.OnCreate(clip.Data);
            bool isActive = frameIndex <= clip.StartFrame && (clip.Length <= 0 || clip.StartFrame + clip.Length < frameIndex);
            playable.Active = isActive;
            if (isActive)
                playable.OnActive(clip.Data);
            else
                playable.OnDeActive(clip.Data);
        }

        protected virtual System.Type GetPlayableType(System.Type clipType)
        {
            return null;
        }

        protected virtual void OnDestroy()
        {
            foreach (var playble in Playables)
            {
                DestroyImmediate(playble);
            }
            Playables.Clear();
            objectPool.Destroy();
        }
    }
}
