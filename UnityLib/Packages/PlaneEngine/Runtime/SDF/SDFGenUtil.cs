using System.Collections.Generic;
using UnityEngine;
namespace PlaneEngine
{
    public static class SDFGenUtil
    {
        struct GenBox
        {
            public Vector2 Position;
            public Vector2 Rotation;
            public Vector2 Size;
        }

        struct GenCircle
        {
            public Vector2 Position;
            public float Radius;
        }

        struct GenPoints
        {
            public Vector2[] Points;
        }

        class SharpCollector
        {
            public List<GenBox> Boxs = new List<GenBox>();
            public List<GenCircle> Circles = new List<GenCircle>();
            public List<GenPoints> Polygons = new List<GenPoints>();
            public List<GenPoints> Lines = new List<GenPoints>();

            public void AddSharp(Shape sharp)
            {
                Matrix4x4 matrix = PlaneUtils.ToPlaneMatrix(sharp.transform);
                if (sharp is CircleShape circle)
                {
                    var pos = matrix.MultiplyPoint(circle.Offset);
                    Circles.Add(new GenCircle
                    {
                        Position = PlaneUtils.ToVector2(pos),
                        Radius = circle.Radius,
                    });
                }
                else if (sharp is BoxShape box)
                {
                    var pos = matrix.MultiplyPoint(box.Offset);
                    var rotation = matrix.MultiplyVector(new Vector3(1, 0, 0));
                    Boxs.Add(new GenBox
                    {
                        Position = PlaneUtils.ToVector2(pos),
                        Rotation = PlaneUtils.ToVector2(rotation),
                        Size = PlaneUtils.ToVector2(box.Size),
                    });
                }
                else if (sharp is PolyShape polygon)
                {
                    GenPoints points = new GenPoints { Points = new Vector2[polygon.Points.Count] };
                    for (int i=0; i<polygon.Points.Count; ++i)
                    {
                        points.Points[i] = PlaneUtils.ToVector2(matrix.MultiplyPoint(polygon.Points[i]));
                    }
                    Polygons.Add(points);
                }
                else if (sharp is LineShape line)
                {
                    GenPoints points = new GenPoints { Points = new Vector2[line.Points.Count] };
                    for (int i = 0; i < line.Points.Count; ++i)
                    {
                        points.Points[i] = PlaneUtils.ToVector2(matrix.MultiplyPoint(line.Points[i]));
                    }
                    Lines.Add(points);
                }
            }
        }

        public static SDFRawData GeneratorByRoot(GameObject root)
        {
            SDFRawData rawData = new SDFRawData();
            var sharps = root.GetComponentsInChildren<Shape>();
            RectBounds bounds = RectBounds.Empty();
            SharpCollector walkArea = new SharpCollector();

            foreach (var sharp in sharps)
            {
                if (sharp.Type == PolyType.Area && (sharp is LineShape))
                {

                }
            }
            return rawData;
        }

        static RectBounds ToBounds(this PolyShape sharp)
        {
            RectBounds bounds = RectBounds.Empty();
            Matrix4x4 matrix = PlaneUtils.ToPlaneMatrix(sharp.transform);

            foreach (var pt in sharp.Points)
            {
                var pos = matrix.MultiplyPoint(pt);
                bounds.Encapsulate(PlaneUtils.ToVector2(pos));
            }
            return bounds;
        }

        static RectBounds ToBounds(this CircleShape sharp)
        {
            Matrix4x4 matrix = PlaneUtils.ToPlaneMatrix(sharp.transform);
            Vector3 pos = sharp.transform.position; //matrix.MultiplyPoint(sharp.Offset);
            return new RectBounds()
            {
                xMin = pos.x - sharp.Radius,
                yMin = pos.y - sharp.Radius,
                xMax = pos.x + sharp.Radius,
                yMax = pos.y + sharp.Radius,
            };
        }

        //public static RectBounds 
    }

}
