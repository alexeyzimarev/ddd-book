using System.Collections.Generic;
using Marketplace.EventSourcing;
using Marketplace.PaidServices.Domain.PaidService;

namespace Marketplace.PaidServices.Domain.ClassifiedAd
{
    public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
    {
        public IEnumerable<ActivePaidService> ActivePaidServices => _activePaidServices;

        private List<ActivePaidService> _activePaidServices;
        
        public ClassifiedAd(ClassifiedAdId id)
        {
            
        }
        
        protected override void When(object @event)
        {
            throw new System.NotImplementedException();
        }

        protected override void EnsureValidState()
        {
            throw new System.NotImplementedException();
        }
    }
}