using System;
using Marketplace.EventSourcing;
using Marketplace.PaidServices.Domain.Services;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public class ActivePaidService : Value<ActivePaidService>
    {
        ActivePaidService(PaidService paidService, DateTimeOffset expiresAt)
        {
            Service = paidService;
            ExpiresAt = expiresAt;
        }

        public PaidService Service { get; }
        public DateTimeOffset ExpiresAt { get; }

        public static ActivePaidService Create(
            PaidService paidService,
            DateTimeOffset startFrom)
        {
            var expiresAt = startFrom + paidService.Duration;
            return new ActivePaidService(paidService, expiresAt);
        }
    }
}