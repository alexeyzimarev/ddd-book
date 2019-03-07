using System;
using Marketplace.EventSourcing;
using Marketplace.PaidServices.Domain.PaidService;

namespace Marketplace.PaidServices.Domain.ClassifiedAd
{
    public class ActivePaidService : Value<ActivePaidService>
    {
        public PaidServiceType ServiceType { get; }
        public DateTimeOffset ExpiresAt { get; }

        public static ActivePaidService Create(PaidServiceType paidServiceType, DateTimeOffset startFrom)
        {
            var expiresAt = startFrom + PaidService.PaidService.DurationFor(paidServiceType);
            return new ActivePaidService(paidServiceType, expiresAt);
        }

        private ActivePaidService(PaidServiceType paidServiceType, DateTimeOffset expiresAt)
        {
            ServiceType = paidServiceType;
            ExpiresAt = expiresAt;
        }
    }
}