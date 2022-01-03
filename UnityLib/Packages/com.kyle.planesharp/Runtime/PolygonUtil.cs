using System.Collections.Generic;
using UnityEngine;

namespace PlaneSharp
{
    public struct ClosestPointResult
    {
        public Vector3 Point;
        public int Index;//边索引，从0开始
        public float NormalLength;//0-1;
        public float SegmentLength;//靠近的线段长度
    }

    public static class PolygonUtil
    {
        public static ClosestPointResult ClosestPointToPolyLine(Vector3 point, List<Vector3> polygon)
        {
            float num = DistancePointLine(point, polygon[polygon.Count - 1], polygon[0]);
            int index = polygon.Count - 1;
            for (int i = 1; i < polygon.Count; i++)
            {
                float num3 = DistancePointLine(point, polygon[i - 1], polygon[i]);
                if (num3 < num)
                {
                    num = num3;
                    index = i - 1;
                }
            }
            Vector3 vector = polygon[index];
            int nextIndex = index + 1;
            if (nextIndex == polygon.Count)
                nextIndex = 0;
            Vector3 vector2 = polygon[nextIndex];
            Vector3 v = point - vector;
            v.y = 0;
            Vector3 v2 = vector2 - vector;
            v2.y = 0;
            float magnitude = v2.magnitude;
            float num4 = Vector3.Dot(v2, v);
            if (magnitude > 1E-06f)
            {
                num4 /= magnitude * magnitude;
            }

            num4 = Mathf.Clamp01(num4);
            return new ClosestPointResult { Index = index, Point = Vector3.Lerp(vector, vector2, num4), NormalLength = num4, SegmentLength = magnitude };
        }

        public static ClosestPointResult ClosestPointToPolyLine(Vector3 point, Vector3[] polygon)
        {
            float num = DistancePointLine(point, polygon[polygon.Length - 1], polygon[0]);
            int index = polygon.Length - 1;
            for (int i = 1; i < polygon.Length; i++)
            {
                float num3 = DistancePointLine(point, polygon[i - 1], polygon[i]);
                if (num3 < num)
                {
                    num = num3;
                    index = i - 1;
                }
            }
            Vector3 vector = polygon[index];
            int nextIndex = index + 1;
            if (nextIndex == polygon.Length)
                nextIndex = 0;
            Vector3 vector2 = polygon[nextIndex];
            Vector3 v = point - vector;
            v.y = 0;
            Vector3 v2 = vector2 - vector;
            v2.y = 0;
            float magnitude = v2.magnitude;
            float num4 = Vector3.Dot(v2, v);
            if (magnitude > 1E-06f)
            {
                num4 /= magnitude * magnitude;
            }

            num4 = Mathf.Clamp01(num4);
            return new ClosestPointResult { Index = index, Point = Vector3.Lerp(vector, vector2, num4), NormalLength = num4, SegmentLength = magnitude };
        }

        public static ClosestPointResult ClosestPointToLine(Vector3 point, Vector3[] line)
        {
            float num = float.MaxValue;
            int index = 0;
            for (int i = 1; i < line.Length; i++)
            {
                float num3 = DistancePointLine(point, line[i - 1], line[i]);
                if (num3 < num)
                {
                    num = num3;
                    index = i - 1;
                }
            }
            Vector3 vector = line[index];
            Vector3 vector2 = line[index + 1];
            Vector3 v = point - vector;
            v.y = 0;
            Vector3 v2 = vector2 - vector;
            v2.y = 0;
            float magnitude = v2.magnitude;
            float num4 = Vector3.Dot(v2, v);
            if (magnitude > 1E-06f)
            {
                num4 /= magnitude * magnitude;
            }

            num4 = Mathf.Clamp01(num4);
            return new ClosestPointResult { Index = index, Point = Vector3.Lerp(vector, vector2, num4), NormalLength = num4, SegmentLength = magnitude };
        }

        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
        }
        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector = lineEnd - lineStart;
            float magnitude = vector.magnitude;
            Vector3 vector2 = vector;
            if (magnitude > 1E-06f)
            {
                vector2 /= magnitude;
            }

            float value = Vector3.Dot(vector2, rhs);
            value = Mathf.Clamp(value, 0f, magnitude);
            return lineStart + vector2 * value;
        }
    }

}
