using UnityEngine;

namespace AssetPackage
{
    internal class AssetDatabaseLoadAssetRequest<T> : LoadAssetRequest<T> where T : Object
    {
        public override float Progeres => 1;

        public override bool keepWaiting => false;

        public AssetDatabaseLoadAssetRequest(T asset)
        {
            Asset = asset;
            DoLoadCallBack();
        }
    }

}

