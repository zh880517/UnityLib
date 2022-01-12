namespace EntityCommand
{
    public class CommandIdentity<T> where T : ICommand
    {
        public static int Id { get; set; } = -1;
        public static string Name => typeof(T).Name;
    }
}