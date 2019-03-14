using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;

namespace Marketplace.Infrastructure.EventStore
{
    public class EventStoreService : IHostedService
    {
        private readonly IEventStoreConnection _esConnection;
        private readonly ProjectionManager[] _projectionManager;

        public EventStoreService(
            IEventStoreConnection esConnection, 
            params ProjectionManager[] projectionManagers)
        {
            _esConnection = esConnection;
            _projectionManager = projectionManagers;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _esConnection.ConnectAsync();
            await Task.WhenAll(
                _projectionManager
                    .Select(projection => projection.Start()));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _esConnection.Close();
            return Task.CompletedTask;
        }
    }
}