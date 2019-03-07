using System;

namespace Marketplace.PaidServices.Domain.ClassifiedAd
{
    public static class Events
    {
        public class PaidServiceAddedForClassifiedAd
        {
            public Guid Id { get; set; }
            public string PaidServiceType { get; set; }
        }
    }
}