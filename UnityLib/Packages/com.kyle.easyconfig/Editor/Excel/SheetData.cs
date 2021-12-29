using System.Collections.Generic;

[System.Serializable]
public class ColumnTitle
{
    public string Name;
    public int Index;
}

[System.Serializable]
public class SheetData
{
    public List<ColumnTitle> Titiles = new List<ColumnTitle>();
    public List<RowData> Data = new List<RowData>();
}
