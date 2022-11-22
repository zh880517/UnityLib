using UnityEngine;

namespace FrameLine
{
    public abstract class FrameClipPlayable : ScriptableObject
    {
        public FrameLineScene Scene;
        public ulong ClipId;
        public bool Active = true;
        public virtual void OnCreate(IFrameActionData clipData) { }
        public virtual void OnActive(IFrameActionData clipData) { }
        public virtual void OnDeActive(IFrameActionData clipData) { }
        public virtual void OnSimatle(IFrameActionData clipData, int clipFrame) { }
        public virtual void OnSceneGUI(IFrameActionData clipData, int clipFrame) { }
        protected virtual void OnDestroy() { }
    }

    public abstract class FrameClipPlayableT<T> : FrameClipPlayable where  T : class, IFrameActionData
    {
        public override void OnActive(IFrameActionData clipData)
        {
            OnClipActive(clipData as T);
        }
        public override void OnDeActive(IFrameActionData clipData)
        {
            OnClipDeActive(clipData as T);
        }
        public override void OnSimatle(IFrameActionData clipData, int clipFrame)
        {
            OnSimatleUnpte(clipData as T, clipFrame);
        }
        public override void OnSceneGUI(IFrameActionData clipData, int clipFrame)
        {
            OnClipSceneGUI(clipData as T, clipFrame);
        }
        protected abstract void OnClipActive(T data);
        protected abstract void OnClipDeActive(T data);
        protected abstract void OnClipSceneGUI(T data, int clipFrame);
        protected abstract void OnSimatleUnpte(T data, int clipFrame);
    }
}
