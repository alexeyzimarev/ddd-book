using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Ads.Queries;
using Marketplace.EventSourcing;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Domain.ClassifiedAds.Events;
using static Marketplace.Ads.Integration.StreamName;
using static Marketplace.Ads.Queries.Projections.ReadModels;
using static Marketplace.Ads.Queries.QueryModels;

namespace Marketplace.Ads.Integration.ClassifiedAds
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
                V1.ClassifiedAdPublished e =>
                () => EmitPublishedAdEvent(e.Id, e.PublishedAt),
                _ => (Func<Task>) null
                };

            async Task EmitPublishedAdEvent(Guid id, DateTimeOffset publishedAt)
            {
                var ad = await GetAd(id);

                await eventStore.AppendEvents(
                    IntegrationStream, new Events.V1.AdPublished
                    {
                        Id          = id,
                        Price       = ad.Price,
                        Title       = ad.Title,
                        PhotoUrl    = ad.PhotoUrls.FirstOrDefault(),
                        SellerId    = ad.SellerId,
                        PublishedAt = publishedAt
                    }
                );
            }

            async Task<ClassifiedAdDetails> GetAd(Guid id)
            {
                using var session = getSession();

                return await session.Query(
                    new GetClassifiedAdDetails
                    {
                        ClassifiedAdId = id
                    }
                );
            }
        }
    }
}
