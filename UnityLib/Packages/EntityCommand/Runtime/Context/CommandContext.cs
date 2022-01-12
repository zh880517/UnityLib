using System.Collections;
using System.Collections.Generic;

namespace EntityCommand
{
    public abstract class CommandContext : ICommandContext
    {
        protected enum CommandType
        {
            Normal = 0,
            Create = 1,
            Destroy = 2,
        }
        protected struct CommandEntity
        {
            public long Id;
            public CommandType Type;
            public ICommand Command;
            public IList Pool;
        }

        private readonly long _Unique;
        private long idIndex;
        protected HashSet<long> exists = new HashSet<long>();
        protected Queue<CommandEntity> Commands = new Queue<CommandEntity>(); 
        public CommandContext()
        {
            _Unique = CreateEntity();
        }
        public long Unique => _Unique;

        public void AddCommand<T>(long id, T command) where T : ICommand, new()
        {
            Commands.Enqueue(new CommandEntity { Id = id, Type = CommandType.Normal, Command = command, Pool = CommandPool<T>.GetPool() });
        }

        public long CreateEntity()
        {
            long id = idIndex++;
            exists.Add(id);
            Commands.Enqueue(new CommandEntity { Id = id, Type = CommandType.Create, Command = null });
            return id;
        }

        public void DestroyEntity(long id)
        {
            Commands.Enqueue(new CommandEntity { Id = id, Type = CommandType.Destroy, Command = null });
            exists.Remove(id);
        }

        public void Execute(ulong frame)
        {
            while(Commands.Count > 0)
            {
                var ce = Commands.Dequeue();
                switch (ce.Type)
                {
                    case CommandType.Normal:
                        ExecuteCommand(ce.Id, frame,ce.Command);
                        ce.Pool.Add(ce.Command);
                        break;
                    case CommandType.Create:
                        OnCreateEntity(ce.Id, frame);
                	    break;
                    case CommandType.Destroy:
                        OnDestroyEntity(ce.Id, frame);
                        break;
                }
            }
        }

        public bool IsValid(long id)
        {
            return id >= 0 && exists.Contains(id);
        }
        public T NewCommand<T>() where T : ICommand, new()
        {
            return CommandPool<T>.Get();
        }

        protected abstract void ExecuteCommand(long id, ulong frame, ICommand command);
        protected abstract void OnCreateEntity(long id, ulong frame);
        protected abstract void OnDestroyEntity(long id, ulong frame);

    }
}