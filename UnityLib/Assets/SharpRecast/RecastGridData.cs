using UnityEngine;
[System.Serializable]
public class RecastGridData
{
    public Vector2 OriginPoint;
    public int Width;//x
    public int Length;//z
    public SerializBitArray Mask;
}
