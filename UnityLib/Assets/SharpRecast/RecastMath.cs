using UnityEngine;

namespace SharpRecast
{
    public static class RecastMath
    {

        public static Bounds GetBounds(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 min = a, max = a;

            if (b.x < min.x) min.x = b.x;
            if (b.y < min.y) min.y = b.y;
            if (b.z < min.z) min.z = b.z;
            if (c.x < min.x) min.x = c.x;
            if (c.y < min.y) min.y = c.y;
            if (c.z < min.z) min.z = c.z;

            if (b.x > max.x) max.x = b.x;
            if (b.y > max.y) max.y = b.y;
            if (b.z > max.z) max.z = b.z;
            if (c.x > max.x) max.x = c.x;
            if (c.y > max.y) max.y = c.y;
            if (c.z > max.z) max.z = c.z;
            Vector3 size = max - min;
            return new Bounds(min + size*0.5f, size);
        }

        public static void GetMinMax(Vector3 a, Vector3 b, Vector3 c, out Vector3 min, out Vector3 max)
        {
            min = a;
            max = a;

            if (b.x < min.x) min.x = b.x;
            if (b.y < min.y) min.y = b.y;
            if (b.z < min.z) min.z = b.z;
            if (c.x < min.x) min.x = c.x;
            if (c.y < min.y) min.y = c.y;
            if (c.z < min.z) min.z = c.z;

            if (b.x > max.x) max.x = b.x;
            if (b.y > max.y) max.y = b.y;
            if (b.z > max.z) max.z = b.z;
            if (c.x > max.x) max.x = c.x;
            if (c.y > max.y) max.y = c.y;
            if (c.z > max.z) max.z = c.z;
        }

        //点到面距离的平方
        public static float SqrDistancePointTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 point)
        {
            Vector3 diff = point - a;
            Vector3 edge0 = b - a;
            Vector3 edge1 = c - a;
            float a00 = Vector3.Dot(edge0, edge0);
            float a01 = Vector3.Dot(edge0, edge1);
            float a11 = Vector3.Dot(edge1, edge1);
            float b0 = -Vector3.Dot(diff, edge0);
            float b1 = -Vector3.Dot(diff, edge1);
            float det = a00 * a11 - a01 * a01;
            float t0 = a01 * b1 - a11 * b0;
            float t1 = a01 * b0 - a00 * b1;
            if (t0 + t1 <= det)
            {
                if (t0 < 0)
                {
                    if (t1 < 0)  // region 4
                    {
                        if (b0 < 0)
                        {
                            t1 = 0;
                            if (-b0 >= a00)  // V1
                            {
                                t0 = 1;
                            }
                            else  // E01
                            {
                                t0 = -b0 / a00;
                            }
                        }
                        else
                        {
                            t0 = 0;
                            if (b1 >= 0)  // V0
                            {
                                t1 = 0;
                            }
                            else if (-b1 >= a11)  // V2
                            {
                                t1 = 1;
                            }
                            else  // E20
                            {
                                t1 = -b1 / a11;
                            }
                        }
                    }
                    else  // region 3
                    {
                        t0 = 0;
                        if (b1 >= 0)  // V0
                        {
                            t1 = 0;
                        }
                        else if (-b1 >= a11)  // V2
                        {
                            t1 = 1;
                        }
                        else  // E20
                        {
                            t1 = -b1 / a11;
                        }
                    }
                }
                else if (t1 < 0)  // region 5
                {
                    t1 = 0;
                    if (b0 >= 0)  // V0
                    {
                        t0 = 0;
                    }
                    else if (-b0 >= a00)  // V1
                    {
                        t0 = 1;
                    }
                    else  // E01
                    {
                        t0 = -b0 / a00;
                    }
                }
                else  // region 0, interior
                {
                    float invDet = 1 / det;
                    t0 *= invDet;
                    t1 *= invDet;
                }
            }
            else
            {
                float tmp0, tmp1, numer, denom;

                if (t0 < 0)  // region 2
                {
                    tmp0 = a01 + b0;
                    tmp1 = a11 + b1;
                    if (tmp1 > tmp0)
                    {
                        numer = tmp1 - tmp0;
                        denom = a00 - (float)2 * a01 + a11;
                        if (numer >= denom)  // V1
                        {
                            t0 = 1;
                            t1 = 0;
                        }
                        else  // E12
                        {
                            t0 = numer / denom;
                            t1 = 1 - t0;
                        }
                    }
                    else
                    {
                        t0 = 0;
                        if (tmp1 <= 0)  // V2
                        {
                            t1 = 1;
                        }
                        else if (b1 >= 0)  // V0
                        {
                            t1 = 0;
                        }
                        else  // E20
                        {
                            t1 = -b1 / a11;
                        }
                    }
                }
                else if (t1 < 0)  // region 6
                {
                    tmp0 = a01 + b1;
                    tmp1 = a00 + b0;
                    if (tmp1 > tmp0)
                    {
                        numer = tmp1 - tmp0;
                        denom = a00 - 2 * a01 + a11;
                        if (numer >= denom)  // V2
                        {
                            t1 = 1;
                            t0 = 0;
                        }
                        else  // E12
                        {
                            t1 = numer / denom;
                            t0 = 1 - t1;
                        }
                    }
                    else
                    {
                        t1 = 0;
                        if (tmp1 <= 0)  // V1
                        {
                            t0 = 1;
                        }
                        else if (b0 >= 0)  // V0
                        {
                            t0 = 0;
                        }
                        else  // E01
                        {
                            t0 = -b0 / a00;
                        }
                    }
                }
                else  // region 1
                {
                    numer = a11 + b1 - a01 - b0;
                    if (numer <= 0)  // V2
                    {
                        t0 = 0;
                        t1 = 1;
                    }
                    else
                    {
                        denom = a00 - 2 * a01 + a11;
                        if (numer >= denom)  // V1
                        {
                            t0 = 1;
                            t1 = 0;
                        }
                        else  // 12
                        {
                            t0 = numer / denom;
                            t1 = 1 - t0;
                        }
                    }
                }
            }

            Vector3 closest = a + t0 * edge0 + t1 * edge1;
            diff = point - closest;

            return diff.sqrMagnitude;
        }

    }
}