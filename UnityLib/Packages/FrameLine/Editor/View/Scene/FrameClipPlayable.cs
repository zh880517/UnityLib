using UnityEngine;

namespace FrameLine
{
    public abstract class FrameClipPlayable : ScriptableObject
    {
        public FrameLineScene Scene;
        public ulong ClipId;
        public bool Active = true;
        public virtual void OnCreate(IFrameLineClipData clipData) { }
        public virtual void OnActive(IFrameLineClipData clipData) { }
        public virtual void OnDeActive(IFrameLineClipData clipData) { }
        public virtual void OnSimatle(IFrameLineClipData clipData, int clipFrame) { }
        public virtual void OnSceneGUI(IFrameLineClipData clipData, int clipFrame) { }
        protected virtual void OnDestroy() { }
    }

    public abstract class FrameClipPlayableT<T> : FrameClipPlayable where  T : class, IFrameLineClipData
    {
        public override void OnActive(IFrameLineClipData clipData)
        {
            OnClipActive(clipData as T);
        }
        public override void OnDeActive(IFrameLineClipData clipData)
        {
            OnClipDeActive(clipData as T);
        }
        public override void OnSimatle(IFrameLineClipData clipData, int clipFrame)
        {
            OnSimatleUnpte(clipData as T, clipFrame);
        }
        public override void OnSceneGUI(IFrameLineClipData clipData, int clipFrame)
        {
            OnClipSceneGUI(clipData as T, clipFrame);
        }
        protected abstract void OnClipActive(T data);
        protected abstract void OnClipDeActive(T data);
        protected abstract void OnClipSceneGUI(T data, int clipFrame);
        protected abstract void OnSimatleUnpte(T data, int clipFrame);
    }
}
