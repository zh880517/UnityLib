using System;

namespace LiteECS
{
    public struct ECPair<TEntity, TComponent>
    {
        public TEntity Owner;
        public TComponent Value;
    }

    public class ContextT<TEntity> : Context where TEntity : Entity
    {
        protected Func<Context, int, TEntity> creatFacory;

        public TEntity Unique { get; private set; }
        public ContextT(int componentTypeCount, Func<Context, int, TEntity> creatFunc) : base(componentTypeCount)
        {
            creatFacory = creatFunc;
            Unique = CreatEntity();
        }

        public TEntity CreatEntity()
        {
            int newId = GenId();
            var entity = creatFacory(this, newId);
            entitis.Add(newId, entity);
            return entity;
        }

        public TEntity FindEntity(int id)
        {
            return Find(id) as TEntity;
        }

        public ECPair<TEntity, T> GetUniquePair<T>() where T : class, IComponent, IUnique, new()
        {
            if (!ComponentIdentity<T>.Unique)
            {
                throw new Exception($"{ComponentIdentity<T>.Name} not a UniqueComponent");
            }
            int id = ComponentIdentity<T>.Id;
            var collector = collectors[id] as UniqueComponentCollector<T>;
            return collector.GetPair<TEntity>();
        }

        public TComponent GetUnique<TComponent>() where TComponent : class, IComponent, IUnique, new()
        {
            if (!ComponentIdentity<TComponent>.Unique)
            {
                throw new Exception($"{ComponentIdentity<TComponent>.Name} not a UniqueComponent");
            }
            int id = ComponentIdentity<TComponent>.Id;
            var collector = collectors[id] as UniqueComponentCollector<TComponent>;
            return collector.Get();
        }

        public int RegisterReactiveGroup<T>() where T : class, IComponent, new()
        {
            groups.Add(0);
            return groups.Count - 1;
        }

        public ReactiveGroup<TEntity, T> GetReactiveGroup<T>(int index) where T : class, IComponent, new()
        {
            uint version = groups[index];
            versionModify = true;
            return new ReactiveGroup<TEntity, T>(index, version, this);
        }

        public Group<TEntity, TComponent> CreatGroup<TComponent>(Func<TComponent, bool> condition = null) where TComponent : class, IComponent, new()
        {
            return new Group<TEntity, TComponent>(this, condition);
        }

        public EntityFindResult<T> Find<T>(int startIndex, uint version, Func<T, bool> condition = null, int groupIndex = -1) where T : class, IComponent, new()
        {
            int id = ComponentIdentity<T>.Id;
            var collector = collectors[id] as IComponentCollectorT<T>;
            var result = collector.Find(startIndex, version, condition);
            if (groupIndex >= 0)
            {
                if (groups[groupIndex] < result.Version)
                    groups[groupIndex] = result.Version;
            }
            return result;
        }
    }

}
