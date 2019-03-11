using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace Marketplace.Infrastructure.EventStore
{
    public class ProjectionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly ICheckpointStore _checkpointStore;
        private readonly IProjection[] _projections;
        private EventStoreAllCatchUpSubscription _subscription;
        private static readonly ILogger Log = 
            Serilog.Log.ForContext<ProjectionManager>();

        public ProjectionManager(
            IEventStoreConnection connection, 
            ICheckpointStore checkpointStore, 
            params IProjection[] projections)
        {
            _connection = connection;
            _checkpointStore = checkpointStore;
            _projections = projections;
        }

        public async Task Start()
        {
            var settings = new CatchUpSubscriptionSettings(2000, 500,
                Log.IsEnabled(LogEventLevel.Verbose),
                false, "try-out-subscription");

            Log.Debug("Starting the projection manager...");
            
            var position = await _checkpointStore.GetCheckpoint();
            Log.Debug("Retrieved the checkpoint: {checkpoint}", position);
            
            _subscription = _connection.SubscribeToAllFrom(position,
                settings, EventAppeared);
            Log.Debug("Subscribed to $all stream");
        }

        public void Stop() => _subscription.Stop();

        private async Task EventAppeared(EventStoreCatchUpSubscription _, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;
            
            var @event = resolvedEvent.Deserialzie();
            
            Log.Debug("Projecting event {event}", @event.ToString());
            await Task.WhenAll(_projections.Select(x => x.Project(@event)));

            await _checkpointStore.StoreCheckpoint(resolvedEvent.OriginalPosition.Value);
        }
    }
}