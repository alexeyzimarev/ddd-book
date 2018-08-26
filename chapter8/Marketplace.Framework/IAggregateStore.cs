using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IAggregateStore
    {
        Task Save<T>(T aggregate);
    }
}