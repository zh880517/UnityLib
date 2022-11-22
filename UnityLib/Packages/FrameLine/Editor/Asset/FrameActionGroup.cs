using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    [System.Serializable]
    public class FrameActionGroup
    {
        public string Comment;
        public int GroupId;
        public string Name;
        public int FrameCount;
        [HideInInspector]
        public List<FrameAction> Actions = new List<FrameAction>();
    }
}
