using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;

namespace Marketplace.EventStore
{
    public class FunctionalStore : IFunctionalAggregateStore
    {
        readonly IEventStore _eventStore;

        public FunctionalStore(IEventStore eventStore)
            => _eventStore = eventStore;

        public Task Save<T>(
            long version,
            AggregateState<T>.Result update
        )
            where T : class, IAggregateState<T>, new()
            => _eventStore.AppendEvents(
                update.State.StreamName, version, update.Events.ToArray()
            );

        public Task<T> Load<T>(Guid id) where T : IAggregateState<T>, new()
            => Load<T>(id, (x, e) => x.When(x, e));

        async Task<T> Load<T>(Guid id, Func<T, object, T> when)
            where T : IAggregateState<T>, new()
        {
            var state = new T();
            var streamName = state.GetStreamName(id);

            var events = await _eventStore.LoadEvents(streamName);

            return events.Aggregate(state, when);
        }
    }
}