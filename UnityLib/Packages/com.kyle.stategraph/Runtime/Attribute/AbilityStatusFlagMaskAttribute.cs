using System;

public class AbilityStatusFlagMaskAttribute : PropertyCustomDrawerAttribute
{
    public override bool TypeCheck(Type type)
    {
        return type == typeof(int);
    }
}
