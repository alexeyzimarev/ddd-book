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

        static readonly ILogger Log =
            Serilog.Log.ForContext<ClassifiedAdDetailsProjection>();

        readonly IEventStoreConnection _eventStoreConnection;
        readonly Func<Guid, Task<string>> _getUserPhoto;

        public ClassifiedAdUpcasters(
            IEventStoreConnection eventStoreConnection,
            Func<Guid, Task<string>> getUserPhoto)
        {
            _eventStoreConnection = eventStoreConnection;
            _getUserPhoto = getUserPhoto;
        }

        public async Task Project(object @event)
        {
            Log.Debug($"Upcasting {{event}} to {StreamName}", @event);

            switch (@event)
            {
                case Events.ClassifiedAdPublished e:
                    var photoUrl = await _getUserPhoto(e.OwnerId);

                    var newEvent = new ClassifiedAdUpcastedEvents.V1.ClassifiedAdPublished
                    {
                        Id = e.Id,
                        OwnerId = e.OwnerId,
                        ApprovedBy = e.ApprovedBy,
                        SellersPhotoUrl = photoUrl
                    };

                    await _eventStoreConnection.AppendEvents(
                        StreamName,
                        ExpectedVersion.Any,
                        newEvent
                    );
                    break;
            }
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