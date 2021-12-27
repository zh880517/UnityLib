using System;

public class EffectTagAttribute : PropertyCustomDrawerAttribute
{
    public override bool TypeCheck(Type type)
    {
        return type == typeof(int);
    }
}
