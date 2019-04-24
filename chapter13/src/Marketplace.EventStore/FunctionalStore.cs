using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;

namespace Marketplace.EventStore
{
    public class FunctionalStore : IFunctionalAggregateStore
    {
        readonly IEventStoreConnection _connection;

        public FunctionalStore(IEventStoreConnection connection)
            => _connection = connection;

        public Task Save<T>(
            long version,
            AggregateState<T>.Result update)
            where T : class, IAggregateState<T>, new()
            => _connection.AppendEvents(
                update.State.StreamName, version, update.Events.ToArray()
            );

        async Task<T> Load<T>(Guid id, Func<T, object, T> when)
            where T : IAggregateState<T>, new()
        {
            var state = new T();
            var streamName = state.GetStreamName(id);

            var page = await _connection.ReadStreamEventsForwardAsync(
                streamName, 0, 1024, false
            );

            return page.Events.Select(x => x.Deserialze())
                .Aggregate(state, when);
        }

        public Task<T> Load<T>(Guid id) where T : IAggregateState<T>, new()
            => Load<T>(id, (x, e) => x.When(x, e));
    }
}