using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Ads.Domain.Test;
using Marketplace.EventSourcing;
using Serilog;

namespace Marketplace.Infrastructure.EventStore
{
    public class TestStore
    {
        readonly IEventStoreConnection _connection;

        public TestStore(IEventStoreConnection connection)
            => _connection = connection;

        public Task Save<T>(
            long version,
            (T state, IEnumerable<object> events) update)
            where T : IAggregateState<T>
            => _connection.AppendEvents(
                update.state.StreamName, version, update.events.ToArray()
            );

        public async Task<T> Load<T>(Guid id, Func<T, object, T> when)
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

        public async Task<bool> Exists(string streamName)
        {
            var result = await _connection.ReadEventAsync(streamName, 1, false);
            return result.Status != EventReadStatus.NoStream;
        }
    }
}