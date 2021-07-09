using System;

public class DisplayNameAttribute : Attribute
{
    public string Name { get; private set; }
    public string ToolTip { get; private set; }
    public DisplayNameAttribute(string name)
    {
        Name = name;
    }

    public DisplayNameAttribute(string name, string tip)
    {
        Name = name;
        ToolTip = tip;
    }
}
