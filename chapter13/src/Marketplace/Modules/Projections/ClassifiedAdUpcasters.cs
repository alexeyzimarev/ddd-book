using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Ads.Messages.Ads;
using Marketplace.EventSourcing;
using Marketplace.Infrastructure.EventStore;
using ILogger = Serilog.ILogger;

namespace Marketplace.Modules.Projections
{
    public class ClassifiedAdUpcasters : IProjection
    {
        const string StreamName = "UpcastedClassifiedAdEvents";

        readonly IEventStoreConnection _eventStoreConnection;
        readonly Func<Guid, Task<string>> _getUserPhoto;

        public ClassifiedAdUpcasters(
            IEventStoreConnection eventStoreConnection,
            Func<Guid, Task<string>> getUserPhoto)
        {
            _eventStoreConnection = eventStoreConnection;
            _getUserPhoto = getUserPhoto;
        }

        public Task Project(object @event)
        {
            return @event switch
            {
                Events.ClassifiedAdPublished e => UpcastPublished(e),
                _ => (Func<Task>) null
            };

            async Task UpcastPublished(
                Events.ClassifiedAdPublished e)
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