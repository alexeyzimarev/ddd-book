using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using static System.String;

namespace Marketplace.Framework
{
    public class EsAggregateStore : IAggregateStore
    {
        private readonly IEventStoreConnection _connection;

        public EsAggregateStore(IEventStoreConnection connection)
        {
            _connection = connection;
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
                    data: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                    metadata: null))
                .ToArray();

            if (!changes.Any()) return;

            var streamName = GetStreamName<T, TId>(aggregate);

            await _connection.AppendToStreamAsync(
                streamName,
                aggregate.Version,
                changes);
        }

        public async Task<T> Load<T, TId>(string aggregateId)
            where T : Aggregate<TId>, new()
        {
            if (IsNullOrWhiteSpace(aggregateId))
                throw new ArgumentException("Value cannot be null or whitespace.", 
                    nameof(aggregateId));

            var stream = GetStreamName<T>(aggregateId);
            var aggregate = new T();

            var page = await _connection.ReadStreamEventsForwardAsync(
                stream, 0, int.MaxValue, false);

            aggregate.Load(page.Events.Select(resolvedEvent =>
            {
                var dataType = Type.GetType(resolvedEvent.Event.EventType);
                var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
                var data = JsonConvert.DeserializeObject(jsonData, dataType);
                return data;
            }).ToArray());

            return aggregate;
        }

        private static string GetStreamName<T>(string aggregateId)
            => $"{typeof(T).Name}-{aggregateId}";

        private static string GetStreamName<T, TId>(T aggregate) where T : Aggregate<TId>
            => $"{typeof(T).Name}-{aggregate.Id.ToString()}";
    }
}