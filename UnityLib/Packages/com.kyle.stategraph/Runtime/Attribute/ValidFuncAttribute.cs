[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Class, Inherited = false)]
public class ValidFuncAttribute : System.Attribute
{
    public string Name;
    public ValidFuncAttribute(string name)
    {
        Name = name;
    }

}
