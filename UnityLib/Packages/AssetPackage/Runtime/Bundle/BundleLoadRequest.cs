using UnityEngine;

namespace AssetPackage
{
    public abstract class BundleLoadRequest : CustomYieldInstruction
    {
        public abstract float Progress { get; }

        public bool IsDone => !keepWaiting;

        private System.Action<float, bool> onProgress;

        public event System.Action<float, bool> OnProgress
        {
            add
            {
                value(Progress, IsDone);
                onProgress += value;
            }
            remove
            {
                onProgress -= value;
            }
        }

        protected void RefreshProgress()
        {
            onProgress?.Invoke(Progress, IsDone);
        }
    }
}