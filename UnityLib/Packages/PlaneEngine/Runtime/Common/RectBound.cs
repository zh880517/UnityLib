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

        public void Floor()
        {
            xMin = Mathf.Floor(xMin);
            yMin = Mathf.Floor(yMin);
            xMax = Mathf.Floor(xMax);
            yMax = Mathf.Floor(yMax);
        }

        public void Expand(float xAmount, float yAmount)
        {
            xMin -= xAmount * 0.5f;
            xMax += xAmount * 0.5f;
            yMin -= yAmount * 0.5f;
            yMax += yAmount * 0.5f;
        }

        public void Encapsulate(Vector2 point)
        {
            xMin = Mathf.Min(point.x, xMin);
            yMin = Mathf.Min(point.y, yMin);
            xMax = Mathf.Max(point.x, xMax);
            yMax = Mathf.Max(point.y, yMax);
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
