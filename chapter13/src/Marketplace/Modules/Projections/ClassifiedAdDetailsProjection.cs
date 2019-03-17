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
            switch (@event)
            {
                case ClassifiedAdCreated e:

                    return
                        async () => await session.StoreAsync(
                            new ClassifiedAdDetails
                            {
                                Id = GetDbId(e.Id),
                                SellerId = e.OwnerId,
                                SellersDisplayName =
                                    await getUserDisplayName(e.OwnerId)
                            }
                        );
                case ClassifiedAdTitleChanged e:

                    return () =>
                        Update(
                            e.Id, ad => ad.Title = e.Title
                        );
                case ClassifiedAdTextUpdated e:

                    return () =>
                        Update(
                            e.Id,
                            ad => ad.Description = e.AdText
                        );
                case ClassifiedAdPriceUpdated e:

                    return () =>
                        Update(
                            e.Id, ad =>
                            {
                                ad.Price = e.Price;
                                ad.CurrencyCode = e.CurrencyCode;
                            }
                        );

                case ClassifiedAdDeleted e:

                    return async () =>
                    {
                        var ad = await session
                            .LoadAsync<ClassifiedAdDetails>(
                                GetDbId(e.Id)
                            );

                        if (ad != null)
                            session.Delete(ad);
                    };
                case UserDisplayNameUpdated e:

                    return () =>
                        UpdateMultipleItems(
                            session, x => x.SellerId == e.UserId,
                            x => x.SellersDisplayName = e.DisplayName
                        );
                case V1.ClassifiedAdPublished e:

                    return () =>
                        Update(
                            e.Id,
                            ad => ad.SellersPhotoUrl = e.SellersPhotoUrl
                        );
                default:
                    return null;
            }

            string GetDbId(Guid id) => ClassifiedAdDetails.GetDatabaseId(id);

            Task Update(Guid id, Action<ClassifiedAdDetails> update)
                => UpdateItem(session, GetDbId(id), update);
        }
    }
}