using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlaneEngine
{
    interface ISDFSharp
    {
        float SDF(Vector2 point);
    }
    class Box : ISDFSharp
    {
        public Vector2 Position;
        public Vector2 Rotation;
        public Vector2 Size;

        public float SDF(Vector2 point)
        {
            return SDFUtil.OrientedBoxSDF(point, Position, Rotation, Size);
        }
    }

    class Circle : ISDFSharp
    {
        public Vector2 Position;
        public float Radius;

        public float SDF(Vector2 point)
        {
            return SDFUtil.CircleSDF(point, Position, Radius);
        }
    }

    class Polygon : ISDFSharp
    {
        public Vector2[] Points;

        public float SDF(Vector2 point)
        {
            return SDFUtil.PolygonSDF(point, Points);
        }
    }

    class Line : ISDFSharp
    {
        public Vector2[] Points;

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
        private List<ISDFSharp> WalkAble = new List<ISDFSharp>();
        private List<ISDFSharp> Obstacle = new List<ISDFSharp>();


        private void AddSharps(Sharp[] sharps)
        {
            foreach (var sharp in sharps)
            {
                ISDFSharp sdfSharp = ToSDFSharp(sharp);
                if (sharp.Type != PolyType.Area)
                {
                    WalkAble.Add(sdfSharp);
                }
                else if (!(sharp is LineSharp))
                {
                    Obstacle.Add(sdfSharp);
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

        private ISDFSharp ToSDFSharp(Sharp sharp)
        {
            Matrix4x4 matrix = PlaneUtils.ToPlaneMatrix(sharp.transform);
            if (sharp is CircleSharp circle)
            {
                var pos = matrix.MultiplyPoint(circle.Offset);
                return new Circle
                {
                    Position = PlaneUtils.ToVector2(pos),
                    Radius = circle.Radius,
                };
            }
            else if (sharp is BoxSharp box)
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
            else if (sharp is PolySharp polygon)
            {
                Polygon points = new Polygon { Points = new Vector2[polygon.Points.Count] };
                for (int i = 0; i < polygon.Points.Count; ++i)
                {
                    points.Points[i] = PlaneUtils.ToVector2(matrix.MultiplyPoint(polygon.Points[i]));
                }
                return points;
            }
            else if (sharp is LineSharp line)
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
