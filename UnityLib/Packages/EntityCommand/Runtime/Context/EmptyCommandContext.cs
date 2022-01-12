namespace EntityCommand
{
    public class EmptyCommandContext : ICommandContext
    {
        public long Unique => -1;

        public void AddCommand<T>(long id, T command) where T : ICommand, new()
        {
        }

        public long CreateEntity()
        {
            return -1;
        }

        public void DestroyEntity(long id)
        {
            
        }

        public void Execute(ulong frame)
        {
            
        }

        public bool IsValid(long id)
        {
            return false;
        }

        public T NewCommand<T>() where T : ICommand, new()
        {
            return default;
        }
    }

}
