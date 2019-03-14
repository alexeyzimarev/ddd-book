using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Ads.Messages.UserProfile;
using Marketplace.Infrastructure.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Ads.Messages.UserProfile.Events;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.Projections
{
    public class MyClassifiedAdsProjection : RavenDbProjection<MyClassifiedAds>
    {
        public MyClassifiedAdsProjection(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public override Task Project(object @event)
        {
            switch (@event)
            {
                case UserRegistered e:
                    return UsingSession(session =>
                        session.StoreAsync(
                            new MyClassifiedAds
                            {
                                Id = e.UserId.ToString(),
                                MyAds = new List<MyClassifiedAds.MyAd>()
                            }));
                case ClassifiedAdCreated e:
                    return UsingSession(session =>
                        UpdateItem(session, e.OwnerId,
                            myAds => myAds.MyAds.Add(
                                new MyClassifiedAds.MyAd
                                    {Id = e.Id.ToString()})
                        ));
                case ClassifiedAdTitleChanged e:
                    return UsingSession(session =>
                        UpdateItem(session, e.OwnerId,
                            ad => UpdateOneAd(
                                ad, ad.Id,
                                myAd => myAd.Title = e.Title)));
                case ClassifiedAdTextUpdated e:
                    return UsingSession(session =>
                        UpdateItem(session, e.OwnerId,
                            ad => UpdateOneAd(
                                ad, ad.Id,
                                myAd => myAd.Description = e.AdText)));
                case ClassifiedAdPriceUpdated e:
                    return UsingSession(session =>
                        UpdateItem(session, e.OwnerId,
                            ad => UpdateOneAd(
                                ad, ad.Id,
                                myAd => myAd.Price = e.Price)));
                case ClassifiedAdDeleted e:
                    return UsingSession(session =>
                        UpdateItem(session, e.OwnerId,
                            ad => UpdateOneAd(
                                ad, ad.Id,
                                myAd => ad.MyAds.Remove(myAd))));
                default:
                    return Task.CompletedTask;
            }

            void UpdateOneAd(
                MyClassifiedAds myAds, string adId,
                Action<MyClassifiedAds.MyAd> update)
            {
                var ad = myAds.MyAds
                    .FirstOrDefault(x => x.Id == adId);
                if (ad != null) update(ad);
            }
        }
    }
}