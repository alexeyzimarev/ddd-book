using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IAggregateStore
    {
        Task Save<T, TId>(T aggregate) where T : Aggregate<TId>;
    }
}