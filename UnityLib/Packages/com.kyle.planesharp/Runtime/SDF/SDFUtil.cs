using UnityEngine;

public static class SDFUtil
{
    public static float CircleSDF(Vector2 point, Vector2 center, float radius)
    {
        return Vector2.Distance(point, center) - radius;
    }

    public static float SegmentSDF(Vector2 point, Vector2 from, Vector2 to)
    {
        Vector2 ap = point - from;
        Vector2 ab = to - from;
        float h = Mathf.Clamp01(Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab));
        return (ap - h * ab).magnitude;
    }

    public static float BoxSDF(Vector2 point, Vector2 center, Vector2 size)
    {
        Vector2 p = point - center;
        p.x = Mathf.Abs(p.x);
        p.y = Mathf.Abs(p.y);
        Vector2 d = p - size;
        return Vector2.Max(d, Vector2.zero).sqrMagnitude + Mathf.Min(Mathf.Max(d.x, d.y), 0);
    }

    public static float OrientedBoxSDF(Vector2 point, Vector2 center, Vector2 rotation, Vector2 size)
    {
        Vector2 v = point - center;
        float px = Mathf.Abs(Vector2.Dot(v, rotation));//在box的x轴的投影长度
        float py = Mathf.Abs(Vector2.Dot(v, new Vector2(-rotation.y, rotation.x)));//在box的y轴的投影长度
        Vector2 p = new Vector2(px, py);
        Vector2 d = p - size;
        return Vector2.Max(d, Vector2.zero).sqrMagnitude + Mathf.Min(Mathf.Max(d.x, d.y), 0);
    }

    public static float TriangleSDF(Vector2 point, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        Vector2 e0 = p1 - p0, e1 = p2 - p1, e2 = p0 - p2;
        Vector2 v0 = point - p0, v1 = point - p1, v2 = point - p2;
        Vector2 pq0 = v0 - e0 * Mathf.Clamp01(Vector2.Dot(v0, e0) / Vector2.Dot(e0, e0));
        Vector2 pq1 = v1 - e1 * Mathf.Clamp01(Vector2.Dot(v1, e1) / Vector2.Dot(e1, e1));
        Vector2 pq2 = v2 - e2 * Mathf.Clamp01(Vector2.Dot(v2, e2) / Vector2.Dot(e2, e2));
        float s = Sign(e0.x * e2.y - e0.y * e2.x);

        Vector2 d = Vector2.Min(Vector2.Min(new Vector2(Vector2.Dot(pq0, pq0), s * (v0.x * e0.y - v0.y * e0.x)),
                     new Vector2(Vector2.Dot(pq1, pq1), s * (v1.x * e1.y - v1.y * e1.x))),
                     new Vector2(Vector2.Dot(pq2, pq2), s * (v2.x * e2.y - v2.y * e2.x)));
        return -Mathf.Sqrt(d.x) * Sign(d.y);
    }

    public static float PolygonSDF(Vector2 p, Vector2[] v)
    {
        float d = Vector2.Dot(p - v[0], p - v[0]);
        float s = 1;
        for (int i = 0, j = v.Length - 1; i < v.Length; j = i, i++)
        {
            Vector2 e = v[j] - v[i];
            Vector2 w = p - v[i];
            Vector2 b = w - e * Mathf.Clamp01(Vector2.Dot(w, e) / Vector2.Dot(e, e));
            d = Mathf.Min(d, Vector2.Dot(b, b));
            bool bA = p.y >= v[i].y, bB = p.y < v[j].y, bC = e.x * w.y > e.y * w.x;
            if ((bA && bB && bC) || (!bA && !bB && !bC))
                s *= -1.0f;
        }
        return s * Mathf.Sqrt(d);
    }

    public static float Sign(float val)
    {
        if (val == 0)
            return 0;
        if (val > 0)
            return 1;
        return -1;
    }
}
