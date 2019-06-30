using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Newtonsoft.Json;

namespace Marketplace.EventStore
{
    public class EventStore : IEventStore
    {
        readonly IEventStoreConnection _connection;

        public EventStore(IEventStoreConnection connection)
            => _connection = connection;

        public Task AppendEvents(
            string streamName,
            long version,
            params object[] events
        )
        {
            if (events == null || !events.Any()) return Task.CompletedTask;

            var preparedEvents = events
                .Select(
                    @event =>
                        new EventData(
                            Guid.NewGuid(),
                            TypeMapper.GetTypeName(@event.GetType()),
                            true,
                            Serialize(@event),
                            Serialize(
                                new EventMetadata
                                {
                                    ClrType = @event.GetType().FullName
                                }
                            )
                        )
                )
                .ToArray();

            return _connection.AppendToStreamAsync(
                streamName,
                version,
                preparedEvents
            );

            static byte[] Serialize(object data)
                => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }

        public Task AppendEvents(
            string streamName,
            params object[] events
        )
            => AppendEvents(streamName, ExpectedVersion.Any, events);

        public async Task<IEnumerable<object>> LoadEvents(string stream)
        {
            const int pageSize = 4096;

            var start  = 0;
            var events = new List<object>();

            do
            {
                var page = await _connection.ReadStreamEventsForwardAsync(
                    stream, start, pageSize, true
                );

                events.AddRange(
                    page.Events.Select(
                        resolvedEvent => resolvedEvent.Deserialze()
                    )
                );
                if (page.IsEndOfStream) break;

                start += pageSize;
            } while (true);

            return events;
        }

        public async Task<bool> StreamExists(string stream)
        {
            var result = await _connection.ReadEventAsync(stream, 1, false);
            return result.Status != EventReadStatus.NoStream;
        }
    }
}
