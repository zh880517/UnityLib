using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlaneEngine
{
    interface ISDFShape
    {
        float SDF(Vector2 point);
        RectBounds GetBounds();
    }
    class Box : ISDFShape
    {
        public Vector2 Position;
        public Vector2 Rotation;
        public Vector2 Size;

        public RectBounds GetBounds()
        {
            return BoundUtil.BoxBounds(Position, Rotation, Size);
        }

        public float SDF(Vector2 point)
        {
            return SDFUtil.OrientedBoxSDF(point, Position, Rotation, Size);
        }
    }

    class Circle : ISDFShape
    {
        public Vector2 Position;
        public float Radius;

        public RectBounds GetBounds()
        {
            return BoundUtil.CircleBounds(Position, Radius);
        }

        public float SDF(Vector2 point)
        {
            return SDFUtil.CircleSDF(point, Position, Radius);
        }
    }

    class Polygon : ISDFShape
    {
        public Vector2[] Points;

        public RectBounds GetBounds()
        {
            return BoundUtil.PointsBounds(Points);
        }

        public float SDF(Vector2 point)
        {
            return SDFUtil.PolygonSDF(point, Points);
        }
    }

    class Line : ISDFShape
    {
        public Vector2[] Points;

        public RectBounds GetBounds()
        {
            return BoundUtil.PointsBounds(Points);
        }

        public float SDF(Vector2 point)
        {
            float sdf = SDFUtil.SegmentSDF(point, Points[0], Points[1]);
            for (int i=2; i<Points.Length; ++i)
            {
                float val = SDFUtil.SegmentSDF(point, Points[i], Points[i-1]);
                sdf = Mathf.Min(val);
            }
            return sdf;
        }
    }

    public class SDFScene
    {
        private readonly List<ISDFShape> WalkAble = new List<ISDFShape>();
        private readonly List<ISDFShape> Obstacle = new List<ISDFShape>();

        public void AddRoot(GameObject root)
        {
            var shapes = root.GetComponentsInChildren<Shape>();
            AddSharps(shapes);
        }

        public bool IsValid()
        {
            return WalkAble.Count > 0;
        }

        public RectBounds GetBounds()
        {
            RectBounds bounds = RectBounds.Empty();
            foreach (var shape in WalkAble)
            {
                bounds.Encapsulate(shape.GetBounds());
            }
            return bounds;
        }

        private void AddSharps(Shape[] shapes)
        {
            foreach (var shape in shapes)
            {
                if (shape.IsStatic)
                {
                    ISDFShape sdfShape = ToSDFSharp(shape);
                    if (shape.Type != PolyType.Area)
                    {
                        WalkAble.Add(sdfShape);
                    }
                    else if (!(shape is LineShape))
                    {
                        Obstacle.Add(sdfShape);
                    }
                }
            }
        }

        private float SDF(Vector2 point)
        {
            float sdf = WalkAbleSDF(point);
            if (Obstacle.Count > 0)
            {
                sdf = Mathf.Min(sdf, ObstacleSDF(point));
            }
            return sdf;
        }

        private float WalkAbleSDF(Vector2 point)
        {
            float sdf = WalkAble[0].SDF(point);
            for (int i=1; i<WalkAble.Count; ++i)
            {
                float val = WalkAble[i].SDF(point);
                sdf = Mathf.Min(val, sdf);
            }
            return -sdf;
        }

        private float ObstacleSDF(Vector2 point)
        {
            float sdf = WalkAble[0].SDF(point);
            for (int i = 1; i < WalkAble.Count; ++i)
            {
                float val = WalkAble[i].SDF(point);
                sdf = Mathf.Min(val, sdf);
            }
            return sdf;
        }

        private ISDFShape ToSDFSharp(Shape shape)
        {
            Matrix4x4 matrix = PlaneUtils.ToPlaneMatrix(shape.transform);
            if (shape is CircleShape circle)
            {
                var pos = matrix.MultiplyPoint(circle.Offset);
                return new Circle
                {
                    Position = PlaneUtils.ToVector2(pos),
                    Radius = circle.Radius,
                };
            }
            else if (shape is BoxShape box)
            {
                var pos = matrix.MultiplyPoint(box.Offset);
                var rotation = matrix.MultiplyVector(new Vector3(1, 0, 0));
                return new Box
                {
                    Position = PlaneUtils.ToVector2(pos),
                    Rotation = PlaneUtils.ToVector2(rotation),
                    Size = PlaneUtils.ToVector2(box.Size),
                };
            }
            else if (shape is PolyShape polygon)
            {
                Polygon points = new Polygon { Points = new Vector2[polygon.Points.Count] };
                for (int i = 0; i < polygon.Points.Count; ++i)
                {
                    points.Points[i] = PlaneUtils.ToVector2(matrix.MultiplyPoint(polygon.Points[i]));
                }
                return points;
            }
            else if (shape is LineShape line)
            {
                Line points = new Line { Points = new Vector2[line.Points.Count] };
                for (int i = 0; i < line.Points.Count; ++i)
                {
                    points.Points[i] = PlaneUtils.ToVector2(matrix.MultiplyPoint(line.Points[i]));
                }
                return points;
            }
            return null;
        }
    }
}
