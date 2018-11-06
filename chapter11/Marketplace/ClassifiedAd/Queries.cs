using System.Collections.Generic;
using System.Linq;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        public static ReadModels.ClassifiedAdDetails Query(
            this IEnumerable<ReadModels.ClassifiedAdDetails> items,
            QueryModels.GetPublicClassifiedAd query) 
            => items.FirstOrDefault(x => x.ClassifiedAdId == query.ClassifiedAdId);
    }
}