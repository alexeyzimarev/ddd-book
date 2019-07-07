using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Marketplace.EventStore.Logging;

namespace Marketplace.EventStore
{
    public class EsAggregateStore : IAggregateStore
    {
        static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        readonly IEventStore _store;

        public EsAggregateStore(IEventStore store) 
            => _store = store;

        public async Task Save<T>(T aggregate) where T : AggregateRoot
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var streamName = GetStreamName(aggregate);
            var changes = aggregate.GetChanges().ToArray();

            foreach (var change in changes)
                Log.Debug("Persisting event {event}", change.ToString());

            await _store.AppendEvents(streamName, aggregate.Version, changes);

            aggregate.ClearChanges();
        }

        public async Task<T> Load<T>(AggregateId<T> aggregateId)
            where T : AggregateRoot
        {
            if (aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));

            var stream = GetStreamName(aggregateId);
            var aggregate = (T) Activator.CreateInstance(typeof(T), true);

            var events = await _store.LoadEvents(stream);

            Log.Debug("Loading events for the aggregate {aggregate}", aggregate.ToString());

            aggregate.Load(events);

            return aggregate;
        }

        public Task<bool> Exists<T>(AggregateId<T> aggregateId) 
            where T : AggregateRoot
            => _store.StreamExists(GetStreamName(aggregateId));

        static string GetStreamName<T>(AggregateId<T> aggregateId) 
            where T : AggregateRoot 
            => $"{typeof(T).Name}-{aggregateId}";

        static string GetStreamName<T>(T aggregate)
            where T : AggregateRoot
            => StreamName.For<T>(aggregate.Id);
    }
}