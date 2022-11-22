using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    public class FrameLineAssetT<TGroup> : FrameLineAsset where TGroup : FrameActionGroup, new()
    {
        [SerializeField, HideInInspector]
        protected List<TGroup> groups = new List<TGroup>();
        [SerializeField, HideInInspector]
        private int groupIndex;

        public IReadOnlyList<TGroup> Groups => groups;

        public override FrameActionGroup FindGroup(int id)
        {
            return groups.Find(it => it.GroupId == id); ;
        }
        public void SortGroup(System.Comparison<TGroup> comparison)
        {
            groups.Sort(comparison);
        }

        public void RemoveGroup(int groupId)
        {
            for (int i=0; i<groups.Count; ++i)
            {
                if (groups[i].GroupId == groupId)
                {
                    groups.RemoveAt(i);
                    break;
                }
            }
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
