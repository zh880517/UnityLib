public class TypeIconAttribute : System.Attribute
{
    public string Name { get; private set; }
    public TypeIconAttribute(string name)
    {
        Name = name;
    }
}
