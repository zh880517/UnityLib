public class RangeIntAttribute : PropertyCustomDrawerAttribute
{
    public int Min { get; set; }
    public int Max { get; set; }
    public RangeIntAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
