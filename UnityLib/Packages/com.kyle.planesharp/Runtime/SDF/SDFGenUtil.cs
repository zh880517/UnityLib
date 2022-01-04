using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SDFGenUtil
{
    public static Rect EncapsulateRect(Rect rect, Vector2 point)
    {
        if (point.x < rect.xMin)
            rect.xMin = point.x;
        else if (point.x > rect.xMax)
            rect.xMax = point.x;

        if (point.y < rect.yMin)
            rect.yMin = point.y;
        else if (point.y > rect.yMax)
            rect.yMax = point.y;
        return rect;
    }

    public static Rect EncapsulateRect(Rect rect, Rect target)
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
        var sharps = root.GetComponentsInChildren<PlaneSharp.Sharp>();
        Rect bounds = new Rect();
        bool isInit = false;
        foreach (var sharp in sharps)
        {
            if (sharp.Type == PlaneSharp.PolyType.Area && (sharp is PlaneSharp.LineSharp))
            {
                
            }
        }
        return rawData;
    }
}
