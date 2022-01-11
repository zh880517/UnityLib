namespace LiteECS
{

    public class ComponentIdentity<T> where T : IComponent
    {
        const string SUFFIX = "Component";
        static ComponentIdentity()
        {
            var name = typeof(T).Name;
            if (name.EndsWith(SUFFIX))
            {
                Name = name.Substring(0, name.Length - SUFFIX.Length);
            }
            else
            {
                Name = name;
            }
        }
        public static int Id { get; set; } = -1;
        public static bool Unique { get; set; } = false;
        public static string Name { get; private set; }
    }

}
