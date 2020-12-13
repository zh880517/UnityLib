using System;

public class CatalogAttribute : Attribute
{
    public string Name { get; private set; }
    public CatalogAttribute(string name)
    {
        Name = name;
    }
}
