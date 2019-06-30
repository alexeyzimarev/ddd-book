using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.EventSourcing;
using Marketplace.EventStore.Logging;

namespace Marketplace.EventStore
{
    public class EventStoreReactor : ISubscription
    {
        static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public EventStoreReactor(params Reactor[] reactions)
            => _reactions = reactions;

        readonly Reactor[] _reactions;

        public Task Project(object @event)
        {
            var handlers = _reactions.Select(x => x(@event))
                .Where(x => x != null)
                .ToArray();

            if (!handlers.Any()) return Task.CompletedTask;

            Log.Debug("Reacting to event {event}", @event);

            return Task.WhenAll(handlers.Select(x => x()));
        }
    }

    public delegate Func<Task> Reactor(object @event);
}
