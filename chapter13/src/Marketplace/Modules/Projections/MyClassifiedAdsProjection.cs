using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Ads.Messages.UserProfile;
using Marketplace.Infrastructure.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.Projections
{
    public static class MyClassifiedAdsProjection 
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            Func<Guid, string> getDbId = MyClassifiedAds.GetDatabaseId;

            return @event switch
            { 
                Events.V1.UserRegistered e =>
                    () => session.StoreAsync(
                        new MyClassifiedAds
                        {
                            Id = getDbId(e.UserId),
                            MyAds = new List<MyClassifiedAds.MyAd>()
                        }),
                V1.ClassifiedAdCreated e =>
                    () => Update(e.OwnerId,
                        myAds => myAds.MyAds.Add(
                            new MyClassifiedAds.MyAd {Id = e.Id}
                        )),
                V1.ClassifiedAdTitleChanged e =>
                    () => UpdateOneAd(e.OwnerId, e.Id,
                        myAd => myAd.Title = e.Title),
                V1.ClassifiedAdTextUpdated e =>
                    () => UpdateOneAd(e.OwnerId, e.Id,
                        myAd => myAd.Description = e.AdText),
                V1.ClassifiedAdPriceUpdated e =>
                    () => UpdateOneAd(e.OwnerId, e.Id,
                        myAd => myAd.Price = e.Price),
                V1.PictureAddedToAClassifiedAd e =>
                    () => UpdateOneAd(e.OwnerId, e.ClassifiedAdId,
                        myAd => myAd.PhotoUrls.Add(e.Url)),
                V1.ClassifiedAdDeleted e =>
                    () => Update(e.OwnerId,
                        myAd => myAd.MyAds
                            .RemoveAll(x => x.Id == e.Id)),
                _ => (Func<Task>) null
            };

            Task Update(Guid id,
                Action<MyClassifiedAds> update)
                => session.UpdateItem(getDbId(id), update);

            Task UpdateOneAd(Guid id, Guid adId,
                Action<MyClassifiedAds.MyAd> update)
                => Update(id, myAds =>
                    {
                        var ad = myAds.MyAds
                            .FirstOrDefault(x => x.Id == adId);
                        if (ad != null) update(ad);
                    });
        }
    }
}