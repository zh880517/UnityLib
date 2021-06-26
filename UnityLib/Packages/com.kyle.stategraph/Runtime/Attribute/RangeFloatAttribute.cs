using System;

public class RangeFloatAttribute : PropertyCustomDrawerAttribute
{
    public float Min { get; set; }
    public float Max { get; set; }
    public RangeFloatAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public override bool TypeCheck(Type type)
    {
        return type == typeof(double) || type == typeof(float);
    }
}
