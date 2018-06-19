using System;
using Marketplace.Domain;

namespace Marketplace
{
    public class ClassifiedAdsApplicationService
    {
        public void CreateClassifiedAd(Guid id, Guid ownerId)
        {
            var classifiedAd = new ClassifiedAd(new ClassifiedAdId(id), new UserId(ownerId));
            
            // store the entity somehow
        }
    }
}