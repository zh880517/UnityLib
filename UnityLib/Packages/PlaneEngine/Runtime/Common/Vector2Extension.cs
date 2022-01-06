using PlaneEngine;
using UnityEngine;
public static class Vector2Extension
{
    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector2 Abs(this Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }

    public static Vector2 Rotate(this Vector2 vec, Vector2 normal)
    {
        return new Vector2(vec.x * normal.x - vec.y * normal.y, vec.x * normal.y + vec.y * normal.x);
    }

    public static Vector2 Rotate(this Vector2 vec, float angle)
    {
        float radians = Mathf.Deg2Rad * angle;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(vec.x * cos - vec.y * sin, vec.x * sin + vec.y * cos);
    }

    //反向旋转向量
    public static Vector2 InvertRotate(this Vector2 vec, Vector2 normal)
    {
        return new Vector2(vec.x * normal.x + vec.y * normal.y, -vec.x * normal.y + vec.y * normal.x);
    }
}
