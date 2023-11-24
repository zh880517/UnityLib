using System.Collections.Generic;
using System.Linq;

public struct SdpLitePackGenerator : ISdpLiteCodeGenerator
{
    public static void WritePackField(CSharpCodeWriter writer, SdpLiteFieldInfo field)
    {
        switch (field.FieldType)
        {
            case SdpLiteStructType.Enum:
                writer.WriteLine($"Pack(packer, {field.Index}, false, (int)value.{field.Info.Name});");
                break;
            case SdpLiteStructType.Integer:
            case SdpLiteStructType.Float:
            case SdpLiteStructType.String:
            case SdpLiteStructType.BuiltInStruct:
            case SdpLiteStructType.CustomStruct:
                writer.WriteLine($"Pack(packer, {field.Index}, false, value.{field.Info.Name});");
                break;
            case SdpLiteStructType.Vector:
                writer.WriteLine($"Pack(packer, {field.Index}, false, value.{field.Info.Name}, Pack);");
                break;
            case SdpLiteStructType.Map:
                writer.WriteLine($"Pack(packer, {field.Index}, false, value.{field.Info.Name}, Pack, Pack);");
                break;
        }
    }

    public static void WritePack(CSharpCodeWriter writer, SdpLiteStruct sdpStruct, IEnumerable<SdpLiteStruct> structs)
    {
        string typeName = GeneratorUtils.TypeToName(sdpStruct.Type);
        writer.WriteLine($"public static void Pack(mygame.SdpLite.Packer packer, uint tag, bool require, {typeName} value)");
        using (new CSharpCodeWriter.Scop(writer))
        {
            writer.WriteLine("var positoin0 = packer.Position;");
            writer.WriteLine("packer.PackHeader(tag, mygame.SdpLite.DataType.StructBegin);");
            writer.WriteLine("var prePositoin = packer.Position;");
            if (sdpStruct.BaseClass != null)
            {
                writer.WriteLine($"Pack(packer, 0, false, ({GeneratorUtils.TypeToName(sdpStruct.BaseClass.Type)})value);");
            }
            if (sdpStruct.Type.IsClass)
            {
                using(new CSharpCodeWriter.Scop(writer, "if(value != null)"))
                {
                    foreach (var field in sdpStruct.Fields)
                    {
                        if (field.FieldType == SdpLiteStructType.CustomStruct)
                        {
                            var s = structs.FirstOrDefault(it => it.Type == field.Info.FieldType);
                            if (s == null || s.IsEmpty())
                                continue;
                        }
                        WritePackField(writer, field);
                    }
                }
            }
            else
            {
                foreach (var field in sdpStruct.Fields)
                {
                    if (field.FieldType == SdpLiteStructType.CustomStruct)
                    {
                        var s = structs.FirstOrDefault(it => it.Type == field.Info.FieldType);
                        if (s == null || s.IsEmpty())
                            continue;
                    }
                    WritePackField(writer, field);
                }
            }
            writer.WriteLine("if(packer.Position == prePositoin && !require)");
            writer.WriteLine("    packer.Rewind(positoin0);");
            writer.WriteLine("else");
            writer.WriteLine("    packer.PackHeader(tag, mygame.SdpLite.DataType.StructEnd);");
        }
    }

    public static void WriteSerialize(CSharpCodeWriter writer, SdpLiteStruct sdpStruct)
    {
        string typeName = GeneratorUtils.TypeToName(sdpStruct.Type);
        writer.WriteLine($"public static void Serialize({typeName} data, System.IO.MemoryStream memory)");
        using (new CSharpCodeWriter.Scop(writer))
        {
            writer.WriteLine("mygame.SdpLite.Packer packer = new mygame.SdpLite.Packer(memory);");
            writer.WriteLine("Pack(packer, 0, true, data);");
        }
    }

    public string GenerateCode(IEnumerable<SdpLiteStruct> structs, string nameSpace, string className)
    {
        CSharpCodeWriter writer = new CSharpCodeWriter();
        writer.WriteLine("//工具自动生成，切勿手动修改");
        using (new CSharpCodeWriter.NameSpaceScop(writer, nameSpace))
        {
            writer.WriteLine($"public partial class {className}Pack : mygame.SdpLitePacker");
            using (new CSharpCodeWriter.Scop(writer))
            {
                foreach (var val in structs)
                {
                    WritePack(writer, val, structs);
                }
                foreach (var val in structs)
                {
                    if (val.GenSerializeFunction)
                    {
                        WriteSerialize(writer, val);
                    }
                }
            }
        }
        return writer.ToString();
    }
}
