using System;

public class EffectTagMaskAttribute : PropertyCustomDrawerAttribute
{
    public override bool TypeCheck(Type type)
    {
        return type == typeof(int);
    }
}
