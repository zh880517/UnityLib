public class RangeFloatAttribute : PropertyCustomDrawerAttribute
{
    public float Min { get; set; }
    public float Max { get; set; }
    public RangeFloatAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }
}
