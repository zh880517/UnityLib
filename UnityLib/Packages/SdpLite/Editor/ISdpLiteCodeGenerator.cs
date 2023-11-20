using System.Collections.Generic;

public interface ISdpLiteCodeGenerator
{
    string GenerateCode(IEnumerable<SdpLiteStruct> structs, string nameSpace, string className);
}
