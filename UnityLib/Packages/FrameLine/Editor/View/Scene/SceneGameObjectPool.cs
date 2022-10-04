using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameLine
{
    [System.Serializable]
    public class SceneGameObjectPool
    {
        [SerializeField]
        protected Transform root;
        protected virtual string RootNodeName => "@FrameLineSceneRoot";
        public Transform Root
        {
            get
            {
                if (root == null)
                {
                    root = new GameObject(RootNodeName).transform;
                }
                return root;
            }
        }
        public List<SceneGameObject> Objects = new List<SceneGameObject>();

        public int Load(string path, bool active)
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj == null)
                return -1;

            SceneGameObject sceneObject = new SceneGameObject { Path = path, Original = obj, Active = active };
            for (int i = 0; i < Objects.Count; ++i)
            {
                if (Objects[i] == null)
                {
                    Objects[i] = sceneObject;
                    return i;
                }
            }
            Objects.Add(sceneObject);
            return Objects.Count - 1;
        }
        public void SetAcive(int index, bool active)
        {
            var item = Get(index);
            if (item != null)
            {
                item.SetActive(active);
            }
        }
        public void SetParent(int index, int parentIndex, string node)
        {
            var item = Get(index);
            if (item != null && (parentIndex != item.ParentIndex || item.ParentNodeName != node))
            {
                item.ParentIndex = parentIndex;
                item.ParentNodeName = node;
                LoadCheck(item);
                AttachToParent(item);
            }
        }
        public GameObject GetGameObject(int index)
        {
            var item = Get(index);
            if (item != null)
            {
                LoadCheck(item);
                return item.Instance;
            }
            return null;
        }
        public void Remove(int index)
        {
            if (index >= 0 && index < Objects.Count)
            {
                var item = Objects[index];
                if (item != null)
                {
                    Objects[index] = null;
                    OnRemove(index);
                    if (item != null && item.Instance)
                    {
                        Object.DestroyImmediate(item.Instance);
                    }
                }
            }
        }
        public void Simulate(int index, float passTime)
        {
            var item = Get(index);
            if (item != null && item.Active)
            {
                LoadCheck(item);
                if (item.Playables != null)
                {
                    foreach (var component in item.Playables)
                    {
                        SimulatePlayable(component, passTime);
                    }
                }
            }
        }
        public virtual void Destroy()
        {
            if (root)
            {
                Object.DestroyImmediate(root);
            }
        }
        protected SceneGameObject Get(int index)
        {
            if (index >= 0 && index < Objects.Count)
            {
                return Objects[index];
            }
            return null;
        }
        protected virtual void OnBuildPlayable(GameObject go, List<Component> components)
        {
        }
        protected virtual bool SimulatePlayable(Component component, float passTime)
        {
            if (component is ParticleSystem particle)
            {
                SimulateUtil.Simulate(particle, passTime);
            }
            else if (component is Animator animator)
            {
                SimulateUtil.Simulate(animator, passTime);
            }
            else if (component is Animation animation)
            {
                SimulateUtil.Simulate(animation, passTime);
            }
            else
            {
                //需要子类处理
                return false;
            }
            return true;
        }
        private void OnRemove(int index)
        {
            for (int i = 0; i < Objects.Count; ++i)
            {
                if (Objects[i] != null && Objects[i].ParentIndex == index)
                {
                    Remove(i);
                }
            }
        }
        protected void LoadCheck(SceneGameObject item)
        {
            if (item.Instance == null)
            {
                item.Instance = Object.Instantiate(item.Original, Root);
                BuildPlayable(item);
                AttachToParent(item);
                if (item.Instance)
                    item.Instance.SetActive(item.Active);
            }
        }
        protected void AttachToParent(SceneGameObject item)
        {
            if (item.ParentIndex < 0 || item.ParentIndex >= Objects.Count || !item.Instance)
                return;
            var parentItem = Objects[item.ParentIndex];
            if (parentItem == null)
            {
                LoadCheck(parentItem);
                if (parentItem.Instance)
                {
                    var parent = parentItem.Instance.transform;
                    if (!string.IsNullOrEmpty(item.ParentNodeName))
                    {
                        parent = AnimationUtil.FindNode(parent, item.ParentNodeName);
                        if (parent == null)
                            parent = parentItem.Instance.transform;
                    }
                    item.Instance.transform.SetParent(parent);
                    return;
                }
            }
            item.Instance.transform.SetParent(Root);
        }

        protected void BuildPlayable(SceneGameObject item)
        {
            if (item.Instance)
            {
                List<Component> playables = new List<Component>();
                playables.AddRange(item.Instance.GetComponentsInChildren<ParticleSystem>());
                playables.AddRange(item.Instance.GetComponentsInChildren<Animation>());
                playables.AddRange(item.Instance.GetComponentsInChildren<Animator>());
                OnBuildPlayable(item.Instance, playables);
                item.Playables = playables.ToArray();
            }
        }

    }
}
