using System;
using System.Collections.Generic;
using System.Reflection;

public enum SdpLiteStructType
{
    Nonsupport,
    Enum,
    Integer,
    Float,
    String,
    Vector,
    Map,
    BuiltInStruct,
    CustomStruct,
}
public class SdpLiteFieldInfo
{
    public uint Index;
    public FieldInfo Info;
    public SdpLiteStructType FieldType;
    public SdpLiteStructType ExternType1;
    public SdpLiteStructType ExternType2;
}
public class SdpLiteStruct
{
    public Type Type;
    public SdpLiteStruct BaseClass;
    public List<SdpLiteFieldInfo> Fields = new List<SdpLiteFieldInfo>();
    public bool IsBuiltIn;
    public bool GenSerializeFunction;

    public bool IsEmpty()
    {
        if (Fields.Count == 0)
        {
            if (BaseClass != null)
                return BaseClass.IsEmpty();
            return true;
        }
        return false;
    }
}