using System;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public static class Events
    {
        public static class V1
        {
            public class OrderCreated
            {
                public Guid ClassifiedAdId { get; set; }
                public Guid SellerId { get; set; }
            }

            public class ServiceActivated
            {
                public Guid ClassifiedAdId { get; set; }
                public string ServiceType { get; set; }
                public DateTimeOffset ActiveUntil { get; set; }
            }

            public class ServiceDeactivated
            {
                public Guid ClassifiedAdId { get; set; }
                public string ServiceType { get; set; }
            }
        }
    }
}