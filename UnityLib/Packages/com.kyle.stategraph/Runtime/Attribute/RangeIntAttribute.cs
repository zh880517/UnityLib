using System.Collections.Generic;
using System;

public class RangeIntAttribute : PropertyCustomDrawerAttribute
{
    private static readonly HashSet<Type> IntTypes = new HashSet<Type>
    {
        typeof(int),
        typeof(uint),
        typeof(short),
        typeof(ushort),
    };
    public int Min { get; set; }
    public int Max { get; set; }
    public RangeIntAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public override bool TypeCheck(Type type)
    {
        return IntTypes.Contains(type);
    }
}
