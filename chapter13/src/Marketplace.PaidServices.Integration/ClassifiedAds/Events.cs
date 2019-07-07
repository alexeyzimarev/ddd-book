using System;

namespace Marketplace.PaidServices.Integration.ClassifiedAds
{
    public static class Events
    {
        public static class V1
        {
            public class EnrichedAdPublished
            {
                public Guid Id { get; set; }
                public Guid SellerId { get; set; }
                public string Title { get; set; }
                public decimal Price { get; set; }
                public string PhotoUrl { get; set; }
                public string[] ActiveServices { get; set; }
                public DateTimeOffset PublishedAt { get; set; }
            }
        }
    }
}
