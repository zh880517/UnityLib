namespace EntityCommand
{
    public interface ICommandContext
    {
        long Unique { get; }
        long CreateEntity();
        T NewCommand<T>() where T : ICommand, new();
        void AddCommand<T>(long id, T command) where T : ICommand, new();
        void Execute(ulong frame);
        void DestroyEntity(long id);
        bool IsValid(long id);
    }
}
