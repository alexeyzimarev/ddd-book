using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Infrastructure.RavenDb;
using Raven.Client.Documents.Session;
using Serilog;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Ads.Messages.UserProfile.Events;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.Projections
{
    public class MyClassifiedAdsProjection : RavenDbProjection<MyClassifiedAds>
    {
        static readonly ILogger Log =
            Serilog.Log.ForContext<ClassifiedAdDetailsProjection>();

        public MyClassifiedAdsProjection(Func<IAsyncDocumentSession> getSession)
            : base(getSession, GetHandler) { }

        static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            Func<Guid, string> getDbId = MyClassifiedAds.GetDatabaseId;

            switch (@event)
            {
                case UserRegistered e:

                    return () =>
                        session.StoreAsync(
                            new MyClassifiedAds
                            {
                                Id = getDbId(e.UserId),
                                MyAds = new List<MyClassifiedAds.MyAd>()
                            }
                        );
                case ClassifiedAdCreated e:

                    return () =>
                        Update(
                            e.OwnerId,
                            myAds => myAds.MyAds.Add(
                                new MyClassifiedAds.MyAd
                                    {Id = e.Id}
                            )
                        );
                case ClassifiedAdTitleChanged e:

                    return () =>
                        UpdateOneAd(
                            e.OwnerId, e.Id,
                            myAd => myAd.Title = e.Title
                        );
                case ClassifiedAdTextUpdated e:

                    return () =>
                        UpdateOneAd(
                            e.OwnerId, e.Id,
                            myAd => myAd.Description = e.AdText
                        );
                case ClassifiedAdPriceUpdated e:

                    return () =>
                        UpdateOneAd(
                            e.OwnerId, e.Id,
                            myAd => myAd.Price = e.Price
                        );
                case ClassifiedAdDeleted e:

                    return () =>
                        Update(
                            e.OwnerId,
                            myAd => myAd.MyAds
                                .RemoveAll(x => x.Id == e.Id)
                        );
                default:
                    return null;
            }

            Task Update(
                Guid id,
                Action<MyClassifiedAds> update)
                => UpdateItem(
                    session, getDbId(id),
                    update
                );

            Task UpdateOneAd(
                Guid id,
                Guid adId,
                Action<MyClassifiedAds.MyAd> update)
                => Update(
                    id,
                    myAds =>
                    {
                        var ad = myAds.MyAds
                            .FirstOrDefault(x => x.Id == adId);
                        if (ad != null) update(ad);
                    }
                );
        }
    }
}