using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    public static class AnimationUtil
    {
        public static RootTransform SampleRootTransform(AnimationClip clip, GameObject obj, float frameRate)
        {
            RootTransform result = new RootTransform() { Name = clip.name };
            int frameCount = Mathf.CeilToInt(clip.length * frameRate);
            result.Positions = new FrameVector3[frameCount];
            result.Rotations = new FrameRotation[frameCount];
            for (int i=0; i<frameCount; ++i)
            {
                clip.SampleAnimation(obj, i / frameRate);
                Vector3 pos = obj.transform.position;
                Vector3 eulerAngles = obj.transform.rotation.eulerAngles;
                result.Positions[i] = new FrameVector3 { FrameIndex = i, Value = Quaternion.Euler(0, -eulerAngles.y, 0) * pos };
                result.Rotations[i] = new FrameRotation { FrameIndex = i, YAngle = eulerAngles.y };
            }
            return result;
        }

        public static Transform FindNode(Transform root, string node)
        {
            if (root.name == node)
                return root;
            for (int i=0; i<root.childCount; ++i)
            {
                var child = root.GetChild(i);
                var result = FindNode(child, node);
                if (result)
                    return result;
            }
            return null;
        }

        public static NodeTransform SampleNodeTransform(AnimationClip clip, GameObject obj, float frameRate, string nodeName)
        {
            NodeTransform result = new NodeTransform { Name = nodeName };
            Transform node = FindNode(obj.transform, nodeName);
            if (node)
            {
                int frameCount = Mathf.CeilToInt(clip.length * frameRate);
                result.Positions = new FrameVector3[frameCount];
                result.Rotations = new FrameVector3[frameCount];
                for (int i = 0; i < frameCount; ++i)
                {
                    clip.SampleAnimation(obj, i / frameRate);
                    Vector3 pos = obj.transform.position;
                    Quaternion inverseRot = Quaternion.Inverse(obj.transform.rotation);
                    result.Positions[i] = new FrameVector3 { FrameIndex = i, Value = node.position - pos };
                    result.Rotations[i] = new FrameVector3 { FrameIndex = i, Value = (node.rotation * inverseRot).eulerAngles };
                }
            }
            return result;
        }

        public static List<NodeTransform> SampleNodeTransform(AnimationClip clip, GameObject obj, float frameRate, List<string> nodeNames)
        {
            List<Transform> nodes = new List<Transform>(nodeNames.Count);
            List<NodeTransform> results = new List<NodeTransform>(nodeNames.Count);
            int frameCount = Mathf.CeilToInt(clip.length * frameRate);
            foreach (var name in nodeNames)
            {
                var trans = FindNode(obj.transform, name);
                nodes.Add(trans);
                var result = new NodeTransform { Name = name };
                if (trans != null)
                {
                    result.Positions = new FrameVector3[frameCount];
                    result.Rotations = new FrameVector3[frameCount];
                }
                results.Add(result);
            }
            for (int i = 0; i < frameCount; ++i)
            {
                clip.SampleAnimation(obj, i / frameRate);
                Vector3 pos = obj.transform.position;
                Quaternion inverseRot = Quaternion.Inverse(obj.transform.rotation);
                for (int j=0; j<nodes.Count; ++j)
                {
                    var node = nodes[j];
                    if (!node)
                        continue;
                    var result = results[j];

                    result.Positions[i] = new FrameVector3 { FrameIndex = i, Value = node.position - pos };
                    result.Rotations[i] = new FrameVector3 { FrameIndex = i, Value = (node.rotation * inverseRot).eulerAngles };
                }
            }
            return results;
        }
    }
}
