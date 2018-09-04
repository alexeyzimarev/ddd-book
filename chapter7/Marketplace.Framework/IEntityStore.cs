using System;

namespace Marketplace.Framework
{
    public interface IEntityStore<T, TId> where T : Entity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Loads an entity by id
        /// </summary>
        T Load(TId entityId);

        /// <summary>
        /// Persists an entity
        /// </summary>
        void Save(T entity);

        /// <summary>
        /// Check if entity with a given id already exists
        /// </summary>
        bool Exists(TId entityId);
    }
}