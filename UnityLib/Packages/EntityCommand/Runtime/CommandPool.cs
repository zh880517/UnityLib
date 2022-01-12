using System.Collections;
using System.Collections.Generic;
namespace EntityCommand
{
    public class CommandPool<T> where T : ICommand, new()
    {
        private static readonly List<T> pool = new List<T>();

        public static T Get()
        {
            if (pool.Count > 0)
            {
                var t = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
                return t;
            }

            return new T();
        }

        public static IList GetPool()
        {
            return pool;
        }

        public static void Recyle(T command)
        {
            pool.Add(command);
        }
    }

}
