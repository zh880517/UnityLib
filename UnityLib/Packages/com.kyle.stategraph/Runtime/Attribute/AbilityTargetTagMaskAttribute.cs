using System;

public class AbilityTargetTagMaskAttribute : PropertyCustomDrawerAttribute
{
    public override bool TypeCheck(Type type)
    {
        return type == typeof(int);
    }
}
