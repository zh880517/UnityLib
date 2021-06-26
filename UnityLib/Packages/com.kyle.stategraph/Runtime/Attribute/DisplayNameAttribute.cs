using System;

public class DisplayNameAttribute : Attribute
{
    public string Name { get; private set; }
    public DisplayNameAttribute(string name)
    {
        Name = name;
    }
}
