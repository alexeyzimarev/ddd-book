using System;
using System.Threading.Tasks;
using Marketplace.Infrastructure.RavenDb;
using Raven.Client.Documents.Session;
using Serilog;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Ads.Messages.UserProfile.Events;
using static Marketplace.Modules.Projections.ClassifiedAdUpcastedEvents;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.Projections
{
    public class ClassifiedAdDetailsProjection
        : RavenDbProjection<ClassifiedAdDetails>
    {
        public ClassifiedAdDetailsProjection(
            Func<IAsyncDocumentSession> getSession,
            Func<Guid, Task<string>> getUserDisplayName)
            : base(
                getSession,
                (session, @event) => GetHandler(
                    session, @event, getUserDisplayName
                )
            ) { }

        static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event,
            Func<Guid, Task<string>> getUserDisplayName)
        {
            return @event switch
                {
                ClassifiedAdCreated e =>
                () => Create(e.Id, e.OwnerId),
                ClassifiedAdTitleChanged e =>
                () => Update(e.Id, ad => ad.Title = e.Title),
                ClassifiedAdTextUpdated e =>
                () => Update(e.Id, ad => ad.Description = e.AdText),
                ClassifiedAdPriceUpdated e =>
                () => Update(
                    e.Id,
                    ad =>
                    {
                        ad.Price = e.Price;
                        ad.CurrencyCode = e.CurrencyCode;
                    }
                ),
                ClassifiedAdDeleted e =>
                () => Delete(e.Id),
                UserDisplayNameUpdated e =>
                () =>
                    UpdateMultipleItems(
                        session, x => x.SellerId == e.UserId,
                        x => x.SellersDisplayName = e.DisplayName
                    ),
                V1.ClassifiedAdPublished e =>
                () => Update(
                    e.Id, ad => ad.SellersPhotoUrl = e.SellersPhotoUrl
                ),
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
                => UpdateItem(session, GetDbId(id), update);

            async Task Delete(Guid id)
            {
                var ad = await session
                    .LoadAsync<ClassifiedAdDetails>(GetDbId(id));

                if (ad != null) session.Delete(ad);
            }
        }
    }
}