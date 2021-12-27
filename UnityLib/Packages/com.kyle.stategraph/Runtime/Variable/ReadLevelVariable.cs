public enum ReadLevelOperate
{
    [DisplayName("无")]
    None = 0,
    [DisplayName("乘以Level")]
    Multiply = 1,
}

[System.Serializable]
public struct ReadLevelVariable
{
    public double Value;
    public string Key;
    public bool Share;
    public ReadLevelOperate OpType;
}
