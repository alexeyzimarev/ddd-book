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
        public MyClassifiedAdsProjection(Func<IAsyncDocumentSession> getSession)
            : base(getSession, GetHandler) { }

        static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            Func<Guid, string> getDbId = MyClassifiedAds.GetDatabaseId;

            return @event switch
            {
                UserRegistered e =>
                () =>
                    session.StoreAsync(
                        new MyClassifiedAds
                        {
                            Id = getDbId(e.UserId),
                            MyAds = new List<MyClassifiedAds.MyAd>()
                        }
                    ),
                ClassifiedAdCreated e =>
                () =>
                    Update(
                        e.OwnerId,
                        myAds => myAds.MyAds.Add(
                            new MyClassifiedAds.MyAd
                                {Id = e.Id}
                        )
                    ),
                ClassifiedAdTitleChanged e =>
                () =>
                    UpdateOneAd(
                        e.OwnerId, e.Id,
                        myAd => myAd.Title = e.Title
                    ),
                ClassifiedAdTextUpdated e =>
                () =>
                    UpdateOneAd(
                        e.OwnerId, e.Id,
                        myAd => myAd.Description = e.AdText
                    ),
                ClassifiedAdPriceUpdated e =>
                () =>
                    UpdateOneAd(
                        e.OwnerId, e.Id,
                        myAd => myAd.Price = e.Price
                    ),
                ClassifiedAdDeleted e =>
                () =>
                    Update(
                        e.OwnerId,
                        myAd => myAd.MyAds
                            .RemoveAll(x => x.Id == e.Id)
                    ),
                _ => (Func<Task>) null
            };

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