using System.Collections.Generic;

namespace FrameLine
{
    [System.Serializable]
    public class FrameLineTrack
    {
        public string Name;
        public string Comment;
        public string TypeGUID;
        public bool Foldout = true;//未被折叠
        public List<FrameActionRef> Actions = new List<FrameActionRef>();

        public int Count => Actions.Count;

        public void Add(FrameActionRef action)
        {
            if (!Actions.Contains(action))
            {
                Actions.Add(action);
            }
        }

        public bool Remove(FrameActionRef action)
        {
            for (int i =0; i< Actions.Count; ++i)
            {
                if (Actions[i] == action)
                {
                    Actions.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }


        public void Sort()
        {
            Actions.Sort(FrameActionUtil.SortByStartFrame);
        }


        public void OnAfterDeserialize(FrameLineAsset asset)
        {
            for (int i=0; i< Actions.Count; ++i)
            {
                var actionRef = Actions[i];
                actionRef.Action = asset.Find(actionRef.ID);
                Actions[i] = actionRef;
            }
        }
    }
}