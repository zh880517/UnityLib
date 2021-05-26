[System.AttributeUsage(System.AttributeTargets.Field)]
public class ValidFuncAttribute : System.Attribute
{
    public string Name;
    public ValidFuncAttribute(string name)
    {
        Name = name;
    }

}
