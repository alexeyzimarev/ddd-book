using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace Marketplace.Framework
{
    public class EsAggregateStore : IAggregateStore
    {
        private readonly IEventStoreConnection _connection;
        private readonly UserCredentials _userCredentials;

        public EsAggregateStore(IEventStoreConnection connection,
            UserCredentials userCredentials = null)
        {
            _connection = connection;
            _userCredentials = userCredentials;
        }

        public async Task Save<T, TId>(T aggregate) where T : Aggregate<TId>
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var changes = aggregate.GetChanges()
                .Select(@event => new EventData(
                    eventId: Guid.NewGuid(),
                    type: @event.GetType().Name,
                    isJson: true,
                    data: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)),
                    encoding: null))
                .ToArray();

            if (!changes.Any()) return default;

            var streamName = GetStreamName<T, TId>(aggregate);

            await _connection.AppendToStreamAsync(
                streamName,
                aggregate.Version,
                changes,
                _userCredentials);
        }

        string GetStreamName<T, TId>(T aggregate) where T : Aggregate<TId>
            => $"{typeof(T).Name}-{aggregate.Id.ToString()}";
    }
}