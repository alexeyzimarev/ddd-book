using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Marketplace.EventStore;
using static Marketplace.Ads.Messages.Ads.Events;
using ILogger = Serilog.ILogger;

namespace Marketplace.Modules.Projections
{
    public class ClassifiedAdUpcasters : ISubscription
    {
        const string StreamName = "UpcastedClassifiedAdEvents";

        readonly IEventStoreConnection _eventStoreConnection;
        readonly Func<Guid, Task<string>> _getUserPhoto;

        public ClassifiedAdUpcasters(
            IEventStoreConnection eventStoreConnection,
            Func<Guid, Task<string>> getUserPhoto
        )
        {
            _eventStoreConnection = eventStoreConnection;
            _getUserPhoto = getUserPhoto;
        }

        public Task Project(object @event)
            => @event switch
            {
                V1.ClassifiedAdPublished e => UpcastPublished(e),
                _ => Task.CompletedTask
            };

        async Task UpcastPublished(
            V1.ClassifiedAdPublished e
        )
            => await _eventStoreConnection.AppendEvents(
                StreamName,
                ExpectedVersion.Any,
                new ClassifiedAdUpcastedEvents.V1.ClassifiedAdPublished
                {
                    Id = e.Id,
                    OwnerId = e.OwnerId,
                    ApprovedBy = e.ApprovedBy,
                    SellersPhotoUrl = await _getUserPhoto(e.OwnerId)
                }
            );
    }

    public static class ClassifiedAdUpcastedEvents
    {
        public static class V1
        {
            public class ClassifiedAdPublished
            {
                public Guid Id { get; set; }
                public Guid OwnerId { get; set; }
                public string SellersPhotoUrl { get; set; }
                public Guid ApprovedBy { get; set; }
            }
        }
    }
}