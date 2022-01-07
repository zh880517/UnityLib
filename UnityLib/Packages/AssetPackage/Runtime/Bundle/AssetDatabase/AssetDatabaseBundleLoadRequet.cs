using UnityEngine;

namespace AssetPackage
{
    internal class AssetDatabaseBundleLoadRequet : BundleLoadRequest
    {
        private float endTime;
        private float delayTime;
        public override float Progress => delayTime > 0 ? Mathf.Clamp01((endTime - Time.realtimeSinceStartup)/delayTime) : 1;

        public override bool keepWaiting 
        {
            get
            {
                RefreshProgress();
                return Time.realtimeSinceStartup < endTime;
            }
        }

        public AssetDatabaseBundleLoadRequet(float delay)
        {
            endTime = Time.realtimeSinceStartup + delay;
            delayTime = delay;
        }
    }

}
