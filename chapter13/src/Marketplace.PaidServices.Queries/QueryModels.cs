using System;

namespace Marketplace.PaidServices.Queries
{
    public static class QueryModels
    {
        public class GetAdActiveServices
        {
            public Guid ClassifiedAdId { get; set; }
        }
    }
}
