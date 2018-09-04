using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Framework;

namespace Marketplace
{
    public class InMemoryEntityStore<T, TId> : IEntityStore<T, TId>
        where T : Entity<TId>
        where TId : IEquatable<TId>
    {
        private readonly List<T> _storage;

        public InMemoryEntityStore() => _storage = new List<T>();

        public T Load(TId entityId)
            => _storage.SingleOrDefault(x => x.Id.Equals(entityId));

        public void Save(T entity)
        {
            if (Exists(entity.Id))
                throw new DuplicatedEntityIdException("Entity with the id {entityId} already exists");

            _storage.Add(entity);
        }

        public bool Exists(TId entityId)
            => _storage.Any(x => x.Id.Equals(entityId));
    }
}