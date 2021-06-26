[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
public abstract class PropertyCustomDrawerAttribute : System.Attribute
{
    public abstract bool TypeCheck(System.Type type);
}
