using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExcelSheetAttribute : Attribute
{
    public string Name { get; private set; }
    public int DataStartRow { get; private set; }
    public ExcelSheetAttribute(string name, int startRow = 2)
    {
        Name = name;
        DataStartRow = startRow;
    }
}
