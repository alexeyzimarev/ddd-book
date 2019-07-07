using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.EventSourcing;
using Marketplace.PaidServices.Queries;
using Marketplace.PaidServices.Queries.ClassifiedAds;
using Raven.Client.Documents.Session;
using static Marketplace.PaidServices.Integration.AdsIntegration.Events;
using static Marketplace.PaidServices.Integration.StreamNames;

namespace Marketplace.PaidServices.Integration.ClassifiedAds
{
    public static class AdsReaction
    {
        public static Func<Task> React(
            IEventStore eventStore,
            Func<IAsyncDocumentSession> getSession,
            object @event
        )
        {
            return @event switch
            {
                V1.AdPublished e => () => EmitPublishedAdEvent(e),
                _                => (Func<Task>) null
            };

            async Task EmitPublishedAdEvent(V1.AdPublished e)
            {
                var services = await GetServices(e.Id);

                await eventStore.AppendEvents(
                    IntegrationStream,
                    new Events.V1.EnrichedAdPublished
                    {
                        Id          = e.Id,
                        Price       = e.Price,
                        Title       = e.Title,
                        PhotoUrl    = e.PhotoUrl,
                        SellerId    = e.SellerId,
                        PublishedAt = e.PublishedAt,
                        ActiveServices = services.ActiveServices
                            .Select(x => x.ServiceType)
                            .ToArray()
                    }
                );
            }

            async Task<ReadModels.AdActiveServices> GetServices(Guid id)
            {
                using var session = getSession();

                return await session.Query(
                    new QueryModels.GetAdActiveServices {ClassifiedAdId = id}
                );
            }
        }
    }
}
