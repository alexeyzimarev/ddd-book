using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Domain.ClassifiedAds.Events;
using static Marketplace.Modules.PublishedAds.ReadModels;

namespace Marketplace.Modules.PublishedAds
{
    public static class PublishedAdsProjection 
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            Func<Guid, string> getDbId = PublishedAd.GetDatabaseId;

            return @event switch
            { 
                V1.ClassifiedAdPublished e =>
                    () => Create(e),
                V1.ClassifiedAdTitleChanged e =>
                    () => Update(e.Id, ad => ad.Title = e.Title),
                V1.ClassifiedAdPriceUpdated e =>
                    () => Update(e.Id, ad => ad.Price = e.Price),
                V1.ClassifiedAdDeleted e =>
                    () => Delete(e.Id),
                _ => (Func<Task>) null
            };

            string GetDbId(Guid id) 
                => PublishedAd.GetDatabaseId(id);
            
            // TODO: Complete the projection
            Task Create(V1.ClassifiedAdPublished e)
            {
                return session.Create<PublishedAd>(
                    x =>
                    {
                        x.Id = GetDbId(e.Id);
                        x.SellerId = e.OwnerId.ToString();
                    }
                );
            }

            Task Update(Guid id, Action<PublishedAd> update)
                => session.Update(getDbId(id), update);
            
            Task Delete(Guid id)
            {
                session.Delete(GetDbId(id));
                return Task.CompletedTask;
            }
        }
    }
}