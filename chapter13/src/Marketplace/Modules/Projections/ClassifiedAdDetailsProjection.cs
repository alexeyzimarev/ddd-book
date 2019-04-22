using System;
using System.Threading.Tasks;
using Marketplace.Infrastructure.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.Projections
{
    public static class ClassifiedAdDetailsProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event,
            Func<Guid, Task<string>> getUserDisplayName)
        {
            return @event switch
            {
                V1.ClassifiedAdCreated e =>
                    () => Create(e.Id, e.OwnerId),
                V1.ClassifiedAdTitleChanged e =>
                    () => Update(e.Id, ad => ad.Title = e.Title),
                V1.ClassifiedAdTextUpdated e =>
                    () => Update(e.Id, ad => ad.Description = e.AdText),
                V1.ClassifiedAdPriceUpdated e =>
                    () => Update( e.Id,
                        ad =>
                        {
                            ad.Price = e.Price;
                            ad.CurrencyCode = e.CurrencyCode;
                        }),
                V1.PictureAddedToAClassifiedAd e =>
                    () => Update(e.ClassifiedAdId, 
                        ad => ad.PhotoUrls.Add(e.Url)),
                V1.ClassifiedAdDeleted e =>
                    () => Delete(e.Id),
                Ads.Messages.UserProfile.Events.V1.UserDisplayNameUpdated e =>
                    () =>
                        session.UpdateMultipleItems<ClassifiedAdDetails>(
                            x => x.SellerId == e.UserId,
                            x => x.SellersDisplayName = e.DisplayName
                        ),
                ClassifiedAdUpcastedEvents.V1.ClassifiedAdPublished e =>
                    () => Update(
                        e.Id, ad => ad.SellersPhotoUrl = e.SellersPhotoUrl),
                _ => (Func<Task>) null
            };

            string GetDbId(Guid id) => ClassifiedAdDetails.GetDatabaseId(id);

            async Task Create(Guid id, Guid ownerId)
                => await session.StoreAsync(
                    new ClassifiedAdDetails
                    {
                        Id = GetDbId(id),
                        SellerId = ownerId,
                        SellersDisplayName =
                            await getUserDisplayName(ownerId)
                    }
                );

            Task Update(Guid id, Action<ClassifiedAdDetails> update)
                => session.UpdateItem(GetDbId(id), update);

            async Task Delete(Guid id)
            {
                var ad = await session
                    .LoadAsync<ClassifiedAdDetails>(GetDbId(id));

                if (ad != null) session.Delete(ad);
            }
        }
    }
}