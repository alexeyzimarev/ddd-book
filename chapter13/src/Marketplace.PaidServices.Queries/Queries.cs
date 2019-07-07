using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using static Marketplace.PaidServices.Queries.ClassifiedAds.ReadModels;
using static Marketplace.PaidServices.Queries.QueryModels;

namespace Marketplace.PaidServices.Queries
{
    public static class Queries
    {
        public static Task<AdActiveServices> Query(
            this IAsyncDocumentSession session,
            GetAdActiveServices query
        )
            => session.LoadAsync<AdActiveServices>(
                AdActiveServices.GetDatabaseId(query.ClassifiedAdId)
            );
    }
}
