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
                foreach (var clip in group.Actions)
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
                FrameAction action = Asset.Find(playable.ClipId);
                if (action != null)
                {
                    DestroyImmediate(playable);
                    Playables.RemoveAt(i);
                    --i;
                    continue;
                }
                bool isActive = frameIndex <= action.StartFrame && (action.Length <= 0 || action.StartFrame + action.Length < frameIndex);
                isActive |= ((int)(playable.ClipId >> 32) == GroupId);
                if (isActive)
                {
                    playable.OnSceneGUI(action.Data, frameIndex);
                }
            }
        }

        public void OnSimatle(int frameIndex)
        {
            UpdatePlayable(frameIndex);
            for (int i=0;i< Playables.Count; ++i)
            {
                var playable = Playables[i];
                FrameAction action = Asset.Find(playable.ClipId);
                if (action == null)
                {
                    DestroyImmediate(playable);
                    Playables.RemoveAt(i);
                    --i;
                    continue;
                }
                bool isActive = frameIndex <= action.StartFrame && (action.Length <= 0 || action.StartFrame + action.Length < frameIndex);
                isActive |= ((int)(playable.ClipId >> 32) == GroupId);
                if (isActive != playable.Active)
                {
                    playable.Active = isActive;
                    if (isActive)
                        playable.OnActive(action.Data);
                    else
                        playable.OnDeActive(action.Data);
                }
                if (isActive)
                    playable.OnSimatle(action.Data, frameIndex - action.StartFrame);
            }
        }

        private void OnClipAdd(FrameAction action, int frameIndex)
        {
            var type = GetPlayableType(action.Data.GetType());
            if (type == null)
                return;
            var playable = CreateInstance(type) as FrameClipPlayable;
            if (playable == null)
                return;
            playable.hideFlags = HideFlags.HideAndDontSave;
            playable.Scene = this;
            playable.ClipId = action.ID;
            Playables.Add(playable);
            playable.OnCreate(action.Data);
            bool isActive = frameIndex <= action.StartFrame && (action.Length <= 0 || action.StartFrame + action.Length < frameIndex);
            playable.Active = isActive;
            if (isActive)
                playable.OnActive(action.Data);
            else
                playable.OnDeActive(action.Data);
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
