using System;
using System.Threading.Tasks;
using Marketplace.Ads.Messages.Ads;
using Marketplace.Infrastructure.RavenDb;
using Raven.Client.Documents.Session;
using Serilog;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Ads.Messages.UserProfile.Events;

namespace Marketplace.Modules.Projections
{
    public class ClassifiedAdDetailsProjection : RavenDbProjection<ReadModels.ClassifiedAdDetails>
    {
        static readonly ILogger Log =
            Serilog.Log.ForContext<ClassifiedAdDetailsProjection>();

        readonly Func<Guid, Task<string>> _getUserDisplayName;

        public ClassifiedAdDetailsProjection(
            Func<IAsyncDocumentSession> getSession,
            Func<Guid, Task<string>> getUserDisplayName)
            : base(getSession)
            => _getUserDisplayName = getUserDisplayName;

        public override async Task Project(object @event)
        {
            Log.Debug("Projecting {event} to ClassifiedAdDetail", @event);

            using var session = GetSession();
            
            await ProjectToRaven(session, @event);
            await session.SaveChangesAsync();
        }

        Task ProjectToRaven(IAsyncDocumentSession session, object @event)
        {
            return @event switch
                {
                ClassifiedAdCreated e => CreateAd(e),
                ClassifiedAdTitleChanged e => UpdateItem(session, e.Id, ad => ad.Title = e.Title),
                _ => Task.CompletedTask
                };

            async Task CreateAd(ClassifiedAdCreated e)
                => await session.StoreAsync(
                    new ReadModels.ClassifiedAdDetails
                    {
                        Id = e.Id.ToString(),
                        SellerId = e.OwnerId,
                        SellersDisplayName = await _getUserDisplayName(e.OwnerId)
                    }
                );
        }

        public async Task Project1(object @event)
        {
            switch (@event)
            {
                case ClassifiedAdTitleChanged e:

                    await UsingSession(
                        session =>
                            UpdateItem(session, e.Id, ad => ad.Title = e.Title)
                    );
                    break;
                case ClassifiedAdTextUpdated e:

                    await UsingSession(
                        session =>
                            UpdateItem(session, e.Id, ad => ad.Description = e.AdText)
                    );
                    break;
                case ClassifiedAdPriceUpdated e:

                    await UsingSession(
                        session =>
                            UpdateItem(
                                session, e.Id, ad =>
                                {
                                    ad.Price = e.Price;
                                    ad.CurrencyCode = e.CurrencyCode;
                                }
                            )
                    );
                    break;
                case ClassifiedAdDeleted e:

                    await UsingSession(
                        async session =>
                        {
                            var doc = await session.LoadAsync<ReadModels.ClassifiedAdDetails>(
                                e.Id.ToString()
                            );
                            session.Delete(doc);
                        }
                    );
                    break;
                case UserDisplayNameUpdated e:

                    await UsingSession(
                        session =>
                            UpdateMultipleItems(
                                session, x => x.SellerId == e.UserId,
                                x => x.SellersDisplayName = e.DisplayName
                            )
                    );
                    break;
                case ClassifiedAdUpcastedEvents.V1.ClassifiedAdPublished e:

                    await UsingSession(
                        session =>
                            UpdateItem(session, e.Id, ad => ad.SellersPhotoUrl = e.SellersPhotoUrl)
                    );
                    break;
            }
        }
    }
}