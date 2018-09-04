using System;
using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IEntityStore<T, in TId> where T : Entity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// Loads an entity by id
        /// </summary>
        Task<T> Load(TId entityId);

        /// <summary>
        /// Persists an entity
        /// </summary>
        Task Save(T entity);

        /// <summary>
        /// Check if entity with a given id already exists
        /// </summary>
        Task<bool> Exists(TId entityId);
    }
}