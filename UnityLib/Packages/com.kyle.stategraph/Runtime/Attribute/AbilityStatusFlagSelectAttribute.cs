using System;

public class AbilityStatusFlagSelectAttribute : PropertyCustomDrawerAttribute
{
    public override bool TypeCheck(Type type)
    {
        return type == typeof(int);
    }
}
