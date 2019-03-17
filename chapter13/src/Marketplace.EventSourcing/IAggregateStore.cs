using System.Threading.Tasks;

namespace Marketplace.EventSourcing
{
    public interface IAggregateStore
    {
        Task<bool> Exists<T>(AggregateId<T> aggregateId) where T : AggregateRoot;

        Task Save<T>(T aggregate) where T : AggregateRoot;

        Task<T> Load<T>(AggregateId<T> aggregateId) where T : AggregateRoot;
    }
}