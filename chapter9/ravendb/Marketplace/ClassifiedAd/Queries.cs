using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        public static Task<List<ReadModels.PublicClassifiedAdListItem>> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetPublishedClassifiedAds query) =>
            session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                .Where(x => x.State == Domain.ClassifiedAd.ClassifiedAd.ClassifiedAdState.Active)
                .Select(x => new ReadModels.PublicClassifiedAdListItem
                {
                    ClassifiedAdId = x.Id.Value,
                    Price = x.Price.Amount,
                    Title = x.Title.Value,
                    CurrencyCode = x.Price.Currency.CurrencyCode
                })
                .Skip(query.Page * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
    }
}