using System;

namespace Marketplace.Ads.Integration.ClassifiedAds
{
    public static class Events
    {
        public static class V1
        {
            public class AdPublished
            {
                public Guid Id { get; set; }
                public Guid SellerId { get; set; }
                public string Title { get; set; }
                public decimal Price { get; set; }
                public string PhotoUrl { get; set; }
                public string[] VisibilityAttributes { get; set; }
                public bool ShowOnTop { get; set; }
                public DateTimeOffset PublishedAt { get; set; }
            }
        }
    }
}
