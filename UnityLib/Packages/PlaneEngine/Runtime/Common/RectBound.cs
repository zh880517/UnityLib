using UnityEngine;
namespace PlaneEngine
{
    public struct RectBounds
    {
        public float xMin;
        public float yMin;
        public float xMax;
        public float yMax;

        public Vector2 Min => new Vector2(Mathf.Min(xMin, xMax), Mathf.Min(yMin, yMax));
        public Vector2 Max => new Vector2(Mathf.Max(xMin, xMax), Mathf.Max(yMin, yMax));
        public Vector2 Size => new Vector2(Mathf.Abs(xMax - xMin), Mathf.Abs(yMax - yMin));

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

}
