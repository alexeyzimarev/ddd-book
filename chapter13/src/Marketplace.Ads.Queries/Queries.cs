using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Queries.Projections.ReadModels;
using static Marketplace.Ads.Queries.QueryModels;

namespace Marketplace.Ads.Queries
{
    public static class Queries
    {
        public static Task<ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            GetClassifiedAdDetails query)
            => session.LoadAsync<ClassifiedAdDetails>(
                query.ClassifiedAdId.ToString()
            );
    }
}