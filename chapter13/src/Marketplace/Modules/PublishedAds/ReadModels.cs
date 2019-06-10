using System;

namespace Marketplace.Modules.PublishedAds
{
    public static class ReadModels
    {
        public class PublishedAd
        {
            public string Id { get; set; }
            public string SellerId { get; set; }
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string PhotoUrl { get; set; }
            public string[] VisibilityAttributes { get; set; }
            public bool ShowOnTop { get; set; }
            public DateTimeOffset PublishedAt { get; set; }

            public static string GetDatabaseId(Guid id)
                => $"PublishedAd/{id}";
        }
    }
}
