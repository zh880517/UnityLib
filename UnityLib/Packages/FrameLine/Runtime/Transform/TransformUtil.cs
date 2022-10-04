using System.Collections.Generic;
using UnityEngine;

namespace FrameLine
{
    public static class TransformUtil
    {
        //取消重复的帧数据
        public static RootTransform Compress(this RootTransform transform)
        {
            RootTransform result = new RootTransform() { Name = transform.Name };
            if (transform.Positions != null && transform.Positions.Length > 0)
            {
                List<FrameVector3> positions = new List<FrameVector3>(transform.Positions);
                for (int i=0;i<positions.Count-1; ++i)
                {
                    if (Vector3.Distance(positions[i].Value, positions[i+1].Value) < 0.001)
                    {
                        positions.RemoveAt(i);
                        --i;
                        continue;
                    }
                }
                if (positions.Count > 1 || positions[0].Value.magnitude >= 0.001)
                {
                    result.Positions = positions.ToArray();
                }
            }
            if (transform.Rotations != null && transform.Rotations.Length > 0)
            {
                List<FrameRotation> rotations = new List<FrameRotation>(transform.Rotations);
                for (int i = 0; i < rotations.Count - 1; ++i)
                {
                    if (Mathf.Abs(rotations[i].YAngle - rotations[i + 1].YAngle) < 0.001)
                    {
                        rotations.RemoveAt(i);
                        --i;
                        continue;
                    }
                }
                if (rotations.Count > 1 || Mathf.Abs(rotations[0].YAngle) >= 0.001)
                {
                    result.Rotations = rotations.ToArray();
                }
            }
            return result;
        }

        public static NodeTransform Compress(this NodeTransform transform)
        {
            NodeTransform result = new NodeTransform() { Name = transform.Name };
            if (transform.Positions != null && transform.Positions.Length > 0)
            {
                List<FrameVector3> positions = new List<FrameVector3>(transform.Positions);
                for (int i = 0; i < positions.Count - 1; ++i)
                {
                    if (Vector3.Distance(positions[i].Value, positions[i + 1].Value) < 0.001)
                    {
                        positions.RemoveAt(i);
                        --i;
                        continue;
                    }
                }
                if (positions.Count > 1 || positions[0].Value.magnitude >= 0.001)
                {
                    result.Positions = positions.ToArray();
                }
            }
            if (transform.Rotations != null && transform.Rotations.Length > 0)
            {
                List<FrameVector3> rotations = new List<FrameVector3>(transform.Rotations);
                for (int i = 0; i < rotations.Count - 1; ++i)
                {
                    if (Vector3.Distance(rotations[i].Value, rotations[i + 1].Value) < 0.001)
                    {
                        rotations.RemoveAt(i);
                        --i;
                        continue;
                    }
                }
                if (rotations.Count > 1 || rotations[0].Value.magnitude >= 0.001)
                {
                    result.Rotations = rotations.ToArray();
                }
            }
            return result;
        }

        public static RootTransform DeCompress(this RootTransform transform, int frameCount)
        {
            RootTransform result = new RootTransform() { Name = transform.Name };
            result.Positions = new FrameVector3[frameCount];
            result.Rotations = new FrameRotation[frameCount];
            for (int i = 0; i < frameCount; ++i)
            {
                result.Positions[i].FrameIndex = i;
                result.Positions[i].FrameIndex = i;
            }
            if (transform.Positions != null && transform.Positions.Length > 0)
            {
                int frameIndex = 0;
                foreach (var pos in transform.Positions)
                {
                    if (pos.FrameIndex >= frameIndex)
                    {
                        for (int i=frameCount; i<=pos.FrameIndex; ++i)
                        {
                            result.Positions[i].Value = pos.Value;
                            frameIndex = i+1;
                        }
                    }
                }
            }
            if (transform.Rotations != null && transform.Rotations.Length > 0)
            {
                int frameIndex = 0;
                foreach (var rot in transform.Rotations)
                {
                    if (rot.FrameIndex >= frameIndex)
                    {
                        for (int i = frameCount; i <= rot.FrameIndex; ++i)
                        {
                            result.Rotations[i].YAngle = rot.YAngle;
                            frameIndex = i + 1;
                        }
                    }
                }
            }
            return result;
        }
        
        public static Vector3 FramePosition(this RootTransform transform, int frame)
        {
            Vector3 pos = Vector3.zero;
            if (transform.Positions != null && transform.Positions.Length > 0)
            {
                foreach (var val in transform.Positions)
                {
                    pos = val.Value;
                    if (val.FrameIndex >= frame)
                        break;
                }
            }
            return pos;
        }

        public static float FrameRotation(this RootTransform transform, int frame)
        {
            float yAngle = 0;
            if (transform.Rotations != null && transform.Rotations.Length > 0)
            {
                foreach (var val in transform.Rotations)
                {
                    yAngle = val.YAngle;
                    if (val.FrameIndex >= frame)
                        break;
                }
            }
            return yAngle;
        }

        //frameTime = passTime/timeperframe
        public static Vector3 GetPosition(this RootTransform transform, float frameTime)
        {
            if (transform.Positions != null && transform.Positions.Length > 0)
            {
                int frame = (int)frameTime;
                float percent = frameTime - frame;
                if (Mathf.Approximately(0, percent))
                    return FramePosition(transform, frame);
                if (Mathf.Approximately(1, percent))
                    return FramePosition(transform, frame + 1);
                Vector3 pre = FramePosition(transform, frame);
                Vector3 next = FramePosition(transform, frame + 1);
                return Vector3.Slerp(pre, next, percent);
            }
            return Vector3.zero;
        }

        public static float GetRotation(this RootTransform transform, float frameTime)
        {
            if (transform.Rotations != null && transform.Rotations.Length > 0)
            {
                int frame = (int)frameTime;
                float percent = frameTime - frame;
                if (Mathf.Approximately(0, percent))
                    return FrameRotation(transform, frame);
                if (Mathf.Approximately(1, percent))
                    return FrameRotation(transform, frame + 1);
                float pre = FrameRotation(transform, frame);
                float next = FrameRotation(transform, frame + 1);
                return pre + (next - pre)*percent;
            }
            return 0;
        }

        public static Vector3 FramePosition(this NodeTransform transform, int frame)
        {
            Vector3 pos = Vector3.zero;
            if (transform.Positions != null && transform.Positions.Length > 0)
            {
                foreach (var val in transform.Positions)
                {
                    pos = val.Value;
                    if (val.FrameIndex >= frame)
                        break;
                }
            }
            return pos;
        }

        public static Vector3 FrameRotation(this NodeTransform transform, int frame)
        {
            Vector3 angle = Vector3.zero;
            if (transform.Rotations != null && transform.Rotations.Length > 0)
            {
                foreach (var val in transform.Rotations)
                {
                    angle = val.Value;
                    if (val.FrameIndex >= frame)
                        break;
                }
            }
            return angle;
        }

        public static Vector3 GetPosition(this NodeTransform transform, float frameTime)
        {
            if (transform.Positions != null && transform.Positions.Length > 0)
            {
                int frame = (int)frameTime;
                float percent = frameTime - frame;
                if (Mathf.Approximately(0, percent))
                    return FramePosition(transform, frame);
                if (Mathf.Approximately(1, percent))
                    return FramePosition(transform, frame + 1);
                Vector3 pre = FramePosition(transform, frame);
                Vector3 next = FramePosition(transform, frame + 1);
                return Vector3.Slerp(pre, next, percent);
            }
            return Vector3.zero;
        }

        public static Vector3 GetRotation(this NodeTransform transform, float frameTime)
        {
            if (transform.Rotations != null && transform.Rotations.Length > 0)
            {
                int frame = (int)frameTime;
                float percent = frameTime - frame;
                if (Mathf.Approximately(0, percent))
                    return FrameRotation(transform, frame);
                if (Mathf.Approximately(1, percent))
                    return FrameRotation(transform, frame + 1);
                Vector3 pre = FrameRotation(transform, frame);
                Vector3 next = FrameRotation(transform, frame + 1);
                return Vector3.Slerp(pre, next, percent);
            }
            return Vector3.zero;
        }
    }
}
