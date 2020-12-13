using System;

public class TypeIdentifyAttribute : Attribute
{
    public string GUID { get; private set; }
    public TypeIdentifyAttribute(string guid)
    {
        GUID = guid;
    }
}
