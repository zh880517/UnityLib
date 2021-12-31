using System;

public class MultiColumnListAttribute : Attribute
{
    public string NamePrefix;
    public bool SkipEmpty;
    public MultiColumnListAttribute(string prefix, bool skipEmpty)
    {
        NamePrefix = prefix;
        SkipEmpty = skipEmpty;
    }
}
