using System.Threading.Tasks;
using Marketplace.Ads.Queries.Projections;
using Raven.Client.Documents.Session;

namespace Marketplace.Ads.Queries
{
    public static class Queries
    {
        public static Task<ReadModels.ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetClassifiedAdDetails query)
            => session.LoadAsync<ReadModels.ClassifiedAdDetails>(
                query.ClassifiedAdId.ToString()
            );
    }
}