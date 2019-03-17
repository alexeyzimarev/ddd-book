using System;
using Marketplace.EventSourcing;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public class ClassifiedAdId : AggregateId<ClassifiedAd>
    {
        public ClassifiedAdId(Guid value) : base(value) { }
    }
}