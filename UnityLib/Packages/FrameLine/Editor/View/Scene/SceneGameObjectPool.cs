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

        public int Load(string path, int key = 0)
        {
            if (key > 0 && key < Objects.Count)
            {
                var sceneObject = Objects[key-1];
                if (sceneObject.Path != path)
                {
                    if (sceneObject.Instance)
                        Object.DestroyImmediate(sceneObject.Instance);

                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (obj == null)
                    {
                        Objects[key - 1] = null;
                        return 0;
                    }
                    sceneObject.Path = path;
                    sceneObject.Original = obj;
                    sceneObject.Instance = null;
                }
                return key;
            }
            else
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj == null)
                    return 0;

                SceneGameObject sceneObject = new SceneGameObject { Path = path, Original = obj };
                for (int i = 0; i < Objects.Count; ++i)
                {
                    if (Objects[i] == null)
                    {
                        Objects[i] = sceneObject;
                        return i - 1;
                    }
                }
                Objects.Add(sceneObject);
                return Objects.Count;
            }
        }
        public void SetAcive(int key, bool active)
        {
            var item = Get(key);
            if (item != null)
            {
                item.SetActive(active);
            }
        }
        public void SetParent(int key, int parentKey, string node)
        {
            var item = Get(key);
            if (item != null && (parentKey != item.ParentKey || item.ParentNodeName != node))
            {
                item.ParentKey = parentKey;
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
        public void Remove(int key)
        {
            if (key > 0 && key <= Objects.Count)
            {
                var item = Objects[key - 1];
                if (item != null)
                {
                    Objects[key - 1] = null;
                    OnRemove(key);
                    if (item != null && item.Instance)
                    {
                        Object.DestroyImmediate(item.Instance);
                    }
                }
            }
        }
        public void Simulate(int key, float passTime)
        {
            var item = Get(key);
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
        protected SceneGameObject Get(int key)
        {
            if (key > 0 && key <= Objects.Count)
            {
                return Objects[key-1];
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
        private void OnRemove(int key)
        {
            for (int i = 0; i < Objects.Count; ++i)
            {
                if (Objects[i] != null && Objects[i].ParentKey == (key - 1))
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
            if (item.ParentKey <= 0 || item.ParentKey > Objects.Count || !item.Instance)
                return;
            var parentItem = Objects[item.ParentKey - 1];
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
