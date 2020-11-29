using System;

public class TypeIdentify : Attribute
{
    public string GUID { get; private set; }
    public TypeIdentify(string guid)
    {
        GUID = guid;
    }
}
