using System;

namespace Marketplace.Ads.Queries
{
    public static class QueryModels
    {
        public class GetOwnersClassifiedAd
        {
            public Guid OwnerId { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

        public class GetClassifiedAdDetails
        {
            public Guid ClassifiedAdId { get; set; }
        }
    }
}