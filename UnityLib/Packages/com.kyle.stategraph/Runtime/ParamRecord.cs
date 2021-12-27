using System;
[Serializable]
public struct ParamRecord
{
    [DisplayName("参数名字")]
    public string Key;
    [DisplayName("值")]
    public ReadVariable Value;
}
