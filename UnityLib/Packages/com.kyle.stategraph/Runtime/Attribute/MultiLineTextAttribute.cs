using System;

public class MultiLineTextAttribute : PropertyCustomDrawerAttribute
{

    public override bool TypeCheck(Type type)
    {
        return type == typeof(string);
    }
}
