using System.Collections.Generic;
namespace LiteECS
{

    public class ComponentCollector<T> : IComponentCollectorT<T> where T : class, IComponent, new()
    {
        private List<ComponentEntity<T>> units = new List<ComponentEntity<T>>();
        private Queue<int> unUsedIdxs = new Queue<int>();
        private Dictionary<int, int> idIdxMap = new Dictionary<int, int>();//EntityId => 数组索引
        private uint Version;
        public int Count { get; private set; }


        private ComponentEntity<T> Create()
        {
            if (unUsedIdxs.Count > 0)
            {
                var index = unUsedIdxs.Dequeue();
                return units[index];
            }
            var unit = new ComponentEntity<T>();
            units.Add(unit);
            return unit;
        }

        public IComponent Add(Entity entity, uint version, bool forceModify)
        {
            if (idIdxMap.TryGetValue(entity.Id, out int idx))
            {
                return units[idx].Component;
            }
            var unit = Create();
            idIdxMap.Add(entity.Id, idx);
            unit.Owner = entity;
            unit.Version = version;
            Version = version;
            ++Count;
            return unit.Component;
        }

        public IComponent Get(Entity entity)
        {
            if (idIdxMap.TryGetValue(entity.Id, out int idx))
            {
                return units[idx].Component;
            }
            return null;
        }

        public IComponent Modify(Entity entity, uint version)
        {
            if (idIdxMap.TryGetValue(entity.Id, out int idx))
            {
                var unit = units[idx];
                unit.Version = version;
                Version = version;
                return unit.Component;
            }
            return null;
        }

        public void Remove(Entity entity)
        {
            if (idIdxMap.TryGetValue(entity.Id, out int idx))
            {
                var unit = units[idx];
                unit.Reset();
                unUsedIdxs.Enqueue(idx);
                idIdxMap.Remove(entity.Id);
                --Count;
            }
        }

        public void RemoveAll()
        {
            if (Count > 0)
            {
                for (int i = 0; i < units.Count; ++i)
                {
                    var unit = units[i];
                    if (unit.Owner != null)
                    {
                        unUsedIdxs.Enqueue(i);
                        idIdxMap.Remove(unit.Owner.Id);
                        unit.Reset();
                        --Count;
                    }
                }
            }
        }

        public EntityFindResult<T> Find(int startIndex, uint version, System.Func<T, bool> condition = null)
        {
            if (Version > version)
            {
                for (int i = startIndex; i < units.Count; ++i)
                {
                    var unit = units[i];
                    if (unit.Owner != null && unit.Version > version && (condition == null || condition(unit.Component)))
                    {
                        return new EntityFindResult<T>()
                        {
                            Id = unit.Owner.Id,
                            Index = i + 1,
                            Version = unit.Version,
                            Component = unit.Component
                        };
                    }
                }
            }
            return new EntityFindResult<T>();
        }
    }

}