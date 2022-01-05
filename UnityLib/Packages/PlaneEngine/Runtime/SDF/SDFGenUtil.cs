using UnityEngine;
namespace PlaneEngine
{
    public static class SDFGenUtil
    {
        public struct RectBounds
        {
            public float xMin;
            public float yMin;
            public float xMax;
            public float yMax;

            public void Encapsulate(Vector2 point)
            {
                if (point.x < xMin)
                    xMin = point.x;
                else if (point.x > xMax)
                    xMax = point.x;

                if (point.y < yMin)
                    yMin = point.y;
                else if (point.y > yMax)
                    yMax = point.y;
            }

            public void Encapsulate(RectBounds target)
            {
                xMin = Mathf.Min(target.xMin, xMin);
                yMin = Mathf.Min(target.yMin, yMin);
                xMax = Mathf.Max(target.xMax, xMax);
                yMax = Mathf.Max(target.yMax, yMax);
            }

            public static RectBounds Empty()
            {
                return new RectBounds
                {
                    xMin = float.MaxValue,
                    yMin = float.MaxValue,
                    xMax = float.MinValue,
                    yMax = float.MinValue,
                };
            }
        }
        public static RectBounds EncapsulateRect(RectBounds rect, RectBounds target)
        {
            rect.xMin = Mathf.Min(target.xMin, rect.xMin);
            rect.yMin = Mathf.Min(target.yMin, rect.yMin);
            rect.xMax = Mathf.Max(target.xMax, rect.xMax);
            rect.yMax = Mathf.Max(target.yMax, rect.yMax);
            return rect;
        }

        public static SDFRawData GeneratorByRoot(GameObject root)
        {
            SDFRawData rawData = new SDFRawData();
            var sharps = root.GetComponentsInChildren<Sharp>();
            RectBounds bounds = RectBounds.Empty();
            foreach (var sharp in sharps)
            {
                if (sharp.Type == PolyType.Area && (sharp is LineSharp))
                {

                }
            }
            return rawData;
        }

        public static RectBounds ToBounds(this PolySharp sharp)
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

        public static RectBounds ToBounds(this CircleSharp sharp)
        {
            Matrix4x4 matrix = PlaneUtils.ToPlaneMatrix(sharp.transform);
            Vector3 pos = matrix.MultiplyPoint(sharp.Offset);
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
