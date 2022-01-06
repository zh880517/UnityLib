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

            for (int i=0; i<4; ++i)
            {
                Vector2 point = Vector2.Scale(BoxPoints[i], size) + center;
                point = point.Rotate(rotation);
                //point = matrix.Multiply(point);
                bounds.Encapsulate(point);
            }
            return bounds;
        }
    }
}
