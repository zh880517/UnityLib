using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    public class FrameLineAssetT<TGroup> : FrameLineAsset where TGroup : FrameClipGroup, new()
    {
        [SerializeField, HideInInspector]
        protected List<TGroup> groups = new List<TGroup>();
        [SerializeField, HideInInspector]
        private int groupIndex;

        public IReadOnlyList<TGroup> Groups => groups;

        public override FrameClipGroup FindGroup(int id)
        {
            return groups.Find(it => it.GroupId == id); ;
        }

        public TGroup CreateGroup()
        {
            TGroup group = new TGroup();
            group.GroupId = groupIndex++;
            groups.Add(group);
            return group;
        }
    }
}
