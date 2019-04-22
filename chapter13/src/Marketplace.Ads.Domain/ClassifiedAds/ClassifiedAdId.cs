using System;
using Marketplace.EventSourcing;

namespace Marketplace.Ads.Domain.ClassifiedAds
{
    public class ClassifiedAdId : AggregateId<ClassifiedAd>
    {
        public static implicit operator ClassifiedAdId(string value)
            => new ClassifiedAdId(Guid.Parse(value));

        public ClassifiedAdId(Guid value) : base(value) { }
    }
}