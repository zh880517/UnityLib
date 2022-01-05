using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlaneEngine
{
    public static class BoundUtil
    {
        public static readonly Vector2[] BoxPoints = new Vector2[]
        {
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(-0.5f, -0.5f),
            new Vector2(-0.5f, 0.5f),
        };
        public static RectBounds CircleBounds(Vector2 center, float radius)
        {
            return new RectBounds
            {
                xMin = center.x - radius,
                xMax = center.x + radius,
                yMin = center.y - radius,
                yMax = center.y + radius,
            };
        }

        public static RectBounds PointsBounds(Vector2[] points)
        {
            RectBounds bounds = RectBounds.Empty();
            for (int i=0; i<points.Length; ++i)
            {
                bounds.Encapsulate(points[i]);
            }
            return bounds;
        }

        public static RectBounds BoxBounds(Vector2 center, Vector2 rotation, Vector2 size)
        {
            RectBounds bounds = RectBounds.Empty();
            Quaternion quaternion = Quaternion.LookRotation(new Vector3(rotation.x, 0, rotation.y));
            for (int i=0; i<4; ++i)
            {
                Vector2 pt = (BoxPoints[i] + center);
                pt.Scale(size);
                Vector3 point = quaternion*PlaneUtils.FromVector2(pt);
                bounds.Encapsulate(PlaneUtils.ToVector2(point));
            }
            return bounds;
        }
    }
}
