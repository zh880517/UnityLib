using System;

public class DisaplayNameAttribute : Attribute
{
    public string Name { get; private set; }
    public DisaplayNameAttribute(string name)
    {
        Name = name;
    }
}
