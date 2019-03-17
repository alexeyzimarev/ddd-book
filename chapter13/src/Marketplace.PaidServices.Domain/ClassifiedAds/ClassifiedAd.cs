using System;
using System.Collections.Generic;
using Marketplace.EventSourcing;
using Marketplace.PaidServices.Domain.ClassifiedAd;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public class ClassifiedAd : AggregateRoot
    {
        List<ActivePaidService> _activePaidServices;

        public ClassifiedAd(ClassifiedAdId id) { }

        public IEnumerable<ActivePaidService> ActivePaidServices => _activePaidServices;

        protected override void When(object @event) => throw new NotImplementedException();

        protected override void EnsureValidState() => throw new NotImplementedException();
    }
}