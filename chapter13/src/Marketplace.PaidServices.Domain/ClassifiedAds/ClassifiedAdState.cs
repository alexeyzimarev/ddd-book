using System;
using System.Collections.Generic;
using Marketplace.EventSourcing;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public class ClassifiedAdState : AggregateState<ClassifiedAdState>
    {
        public override ClassifiedAdState When(ClassifiedAdState state, object @event) => throw new NotImplementedException();

        protected override bool EnsureValidState(ClassifiedAdState newState) => throw new NotImplementedException();
        
        List<ActivePaidService> ActivePaidServices { get; set; }
            = new List<ActivePaidService>();

    }
}