using System;
using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;

namespace Marketplace.Ads.Queries.Projections
{
    public static class ClassifiedAdDetailsProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            return @event switch
            {
                Events.V1.ClassifiedAdCreated e =>
                    () => Create(e.Id, e.OwnerId),
                Events.V1.ClassifiedAdTitleChanged e =>
                    () => Update(e.Id, ad => ad.Title = e.Title),
                Events.V1.ClassifiedAdTextUpdated e =>
                    () => Update(e.Id, ad => ad.Description = e.AdText),
                Events.V1.ClassifiedAdPriceUpdated e =>
                    () => Update( e.Id,
                        ad =>
                        {
                            ad.Price = e.Price;
                            ad.CurrencyCode = e.CurrencyCode;
                        }),
                Events.V1.PictureAddedToAClassifiedAd e =>
                    () => Update(e.ClassifiedAdId, 
                        ad => ad.PhotoUrls.Add(e.Url)),
                Events.V1.ClassifiedAdDeleted e =>
                    () => Delete(e.Id),
                _ => (Func<Task>) null
            };

            string GetDbId(Guid id) 
                => ReadModels.ClassifiedAdDetails.GetDatabaseId(id);

            Task Create(Guid id, Guid ownerId)
                => session.Create<ReadModels.ClassifiedAdDetails>(
                    x =>
                    {
                        x.Id = GetDbId(id);
                        x.SellerId = ownerId;
                    }
                );

            Task Update(Guid id, Action<ReadModels.ClassifiedAdDetails> update)
                => session.Update(GetDbId(id), update);

            Task Delete(Guid id)
            {
                session.Delete(GetDbId(id));
                return Task.CompletedTask;
            }
        }
    }
}