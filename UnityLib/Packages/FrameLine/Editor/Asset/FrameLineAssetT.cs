using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    public class FrameLineAssetT<TGroup> : FrameLineAsset where TGroup : FrameClipGroup
    {
        [SerializeField, HideInInspector]
        protected List<TGroup> Groups = new List<TGroup>();

        public override FrameClipGroup FindGroup(int id)
        {
            return Groups.Find(it => it.GroupId == id); ;
        }
    }
}
