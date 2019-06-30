using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;

namespace Marketplace.Ads.Queries.Projections
{
    public static class MyClassifiedAdsProjection 
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            Func<Guid, string> getDbId = ReadModels.MyClassifiedAds.GetDatabaseId;

            return @event switch
            { 
                Events.V1.ClassifiedAdCreated e =>
                    () => CreateOrUpdate(e.OwnerId,
                        myAds => myAds.MyAds.Add(
                            new ReadModels.MyClassifiedAds.MyAd {Id = e.Id}
                        ),
                        () => new ReadModels.MyClassifiedAds
                        {
                            Id = getDbId(e.OwnerId),
                            MyAds = new List<ReadModels.MyClassifiedAds.MyAd>()
                        }),
                Events.V1.ClassifiedAdTitleChanged e =>
                    () => UpdateOneAd(e.OwnerId, e.Id,
                        myAd => myAd.Title = e.Title),
                Events.V1.ClassifiedAdTextUpdated e =>
                    () => UpdateOneAd(e.OwnerId, e.Id,
                        myAd => myAd.Description = e.AdText),
                Events.V1.ClassifiedAdPriceUpdated e =>
                    () => UpdateOneAd(e.OwnerId, e.Id,
                        myAd => myAd.Price = e.Price),
                Events.V1.PictureAddedToAClassifiedAd e =>
                    () => UpdateOneAd(e.OwnerId, e.ClassifiedAdId,
                        myAd => myAd.PhotoUrls.Add(e.Url)),
                Events.V1.ClassifiedAdDeleted e =>
                    () => Update(e.OwnerId,
                        myAd => myAd.MyAds
                            .RemoveAll(x => x.Id == e.Id)),
                _ => (Func<Task>) null
            };

            Task CreateOrUpdate(
                Guid id,
                Action<ReadModels.MyClassifiedAds> update,
                Func<ReadModels.MyClassifiedAds> create
            )
                => session.UpsertItem(getDbId(id), update, create);
            
            Task Update(Guid id,
                Action<ReadModels.MyClassifiedAds> update)
                => session.Update(getDbId(id), update);

            Task UpdateOneAd(Guid id, Guid adId,
                Action<ReadModels.MyClassifiedAds.MyAd> update)
                => Update(id, myAds =>
                    {
                        var ad = myAds.MyAds
                            .FirstOrDefault(x => x.Id == adId);
                        if (ad != null) update(ad);
                    });
        }
    }
}