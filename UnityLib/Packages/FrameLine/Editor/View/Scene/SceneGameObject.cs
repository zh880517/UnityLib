using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrameLine
{
    [System.Serializable]
    public class SceneGameObject
    {
        public string Path;
        public int ParentIndex = -1;
        public string ParentNodeName;
        public GameObject Original;
        public GameObject Instance;
        public bool Active = true;
        public Component[] Playables;

        public void SetActive(bool active)
        {
            if (Active != active)
            {
                Active = active;
                if (Instance)
                    Instance.SetActive(active);
            }
        }
    }
}
