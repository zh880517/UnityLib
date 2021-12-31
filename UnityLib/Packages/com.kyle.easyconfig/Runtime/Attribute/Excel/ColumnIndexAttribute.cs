using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ColumnIndexAttribute : Attribute
{
    public string Name { get; private set; }
    public ColumnIndexAttribute(string name)
    {
        Name = name;
    }
}
