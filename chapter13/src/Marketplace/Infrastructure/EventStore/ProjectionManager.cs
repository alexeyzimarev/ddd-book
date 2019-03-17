using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace Marketplace.Infrastructure.EventStore
{
    public class ProjectionManager
    {
        static readonly ILogger Log =
            Serilog.Log.ForContext<ProjectionManager>();

        readonly ICheckpointStore _checkpointStore;
        readonly IEventStoreConnection _connection;
        readonly IProjection[] _projections;
        EventStoreAllCatchUpSubscription _subscription;

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
            var settings = new CatchUpSubscriptionSettings(
                2000, 500,
                Log.IsEnabled(LogEventLevel.Verbose),
                false, "try-out-subscription"
            );

            Log.Debug("Starting the projection manager...");

            var position = await _checkpointStore.GetCheckpoint();
            Log.Debug("Retrieved the checkpoint: {checkpoint}", position);

            _subscription = _connection.SubscribeToAllFrom(
                position,
                settings, EventAppeared
            );
            Log.Debug("Subscribed to $all stream");
        }

        public void Stop() => _subscription.Stop();

        async Task EventAppeared(EventStoreCatchUpSubscription _, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;

            var @event = resolvedEvent.Deserialze();

            Log.Debug("Projecting event {event}", @event.ToString());
            await Task.WhenAll(_projections.Select(x => x.Project(@event)));

            await _checkpointStore.StoreCheckpoint(resolvedEvent.OriginalPosition.Value);
        }
    }
}