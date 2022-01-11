using System.Collections.Generic;
namespace LiteECS
{

    public class ComponentCollector<T> : IComponentCollectorT<T> where T : class, IComponent, new()
    {
        private class ComponentUnit
        {
            public T Component;
            public Entity Owner;
            public int Index;
        }
        private List<ComponentUnit> units = new List<ComponentUnit>();
        private Queue<int> unUsedIdxs = new Queue<int>();
        private Dictionary<int, int> idIdxMap = new Dictionary<int, int>();//EntityId => 数组索引
        private List<IEventGroup> eventGroups = new List<IEventGroup>();
        public int Count { get; private set; }

        public ComponentCollector()
        {
        }

        private ComponentUnit CreateUnit()
        {
            if (unUsedIdxs.Count > 0)
            {
                var index = unUsedIdxs.Dequeue();
                return units[index];
            }
            var unit = new ComponentUnit
            {
                Component = new T(),
                Index = units.Count,
            };
            units.Add(unit);
            return unit;
        }

        public IComponent Add(Entity entity, bool forceModify)
        {
            if (idIdxMap.TryGetValue(entity.Id, out int idx))
            {
                if (forceModify)
                {
                    for (int i = 0; i < eventGroups.Count; ++i)
                    {
                        eventGroups[i].OnModify<T>(entity.Id);
                    }
                }
                return units[idx].Component;
            }
            var unit = CreateUnit();
            idIdxMap.Add(entity.Id, idx);
            unit.Owner = entity;
            ++Count;
            for (int i = 0; i < eventGroups.Count; ++i)
            {
                eventGroups[i].OnAdd<T>(entity.Id);
            }
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

        public IComponent Modify(Entity entity)
        {
            if (idIdxMap.TryGetValue(entity.Id, out int idx))
            {
                for (int i=0; i<eventGroups.Count; ++i)
                {
                    eventGroups[i].OnModify<T>(entity.Id);
                }
                return units[idx].Component;
            }
            return null;
        }

        public void Remove(Entity entity)
        {
            if (idIdxMap.TryGetValue(entity.Id, out int idx))
            {
                var unit = units[idx];
                if (unit.Component is IReset resetComp)
                    resetComp.Reset();
                unit.Owner = null;
                unUsedIdxs.Enqueue(idx);
                idIdxMap.Remove(entity.Id);
                --Count;
                for (int i = 0; i < eventGroups.Count; ++i)
                {
                    eventGroups[i].OnRemove<T>(entity.Id);
                }
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
                        if (unit.Component is IReset resetComp)
                            resetComp.Reset();
                        unUsedIdxs.Enqueue(i);
                        idIdxMap.Remove(unit.Owner.Id);
                        for (int j = 0; j < eventGroups.Count; ++j)
                        {
                            eventGroups[j].OnRemove<T>(unit.Owner.Id);
                        }
                        --Count;
                        unit.Owner = null;
                    }
                }
            }
        }

        public Entity Find(ref int startIndex, System.Func<T, bool> condition)
        {
            for (int i=startIndex; i<units.Count; ++i)
            {
                var unit = units[i];
                if (unit.Owner != null && (condition == null || condition(unit.Component)))
                {
                    startIndex = i + 1;
                    return unit.Owner;
                }
            }
            startIndex = units.Count;
            return null;
        }

        public void RegistEventGroup(IEventGroup eventGroup)
        {
            eventGroups.Add(eventGroup);
        }

        public void RemoveEventGroup(IEventGroup eventGroup)
        {
            eventGroups.Remove(eventGroup);
        }

        public Entity Find(ref int startIndex, out T component, System.Func<T, bool> condition)
        {
            for (int i = startIndex; i < units.Count; ++i)
            {
                var unit = units[i];
                if (unit.Owner != null && condition == null || condition(unit.Component))
                {
                    startIndex = i + 1;
                    component = unit.Component;
                    return unit.Owner;
                }
            }
            startIndex = units.Count;
            component = null;
            return null;
        }

    }

}