using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Marketplace.Infrastructure.EventStore
{
    public interface ICheckpointStore
    {
        Task<Position> GetCheckpoint();
        Task StoreCheckpoint(Position checkpoint);
    }
}