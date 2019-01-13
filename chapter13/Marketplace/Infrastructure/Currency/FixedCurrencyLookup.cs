using System.Collections.Generic;
using System.Linq;
using Marketplace.ClassifiedAds.Domain.Shared;

namespace Marketplace.Infrastructure.Currency
{
    public class FixedCurrencyLookup : ICurrencyLookup
    {
        private static readonly IEnumerable<ClassifiedAds.Domain.Shared.Currency> Currencies =
            new[]
            {
                new ClassifiedAds.Domain.Shared.Currency
                {
                    CurrencyCode = "EUR",
                    DecimalPlaces = 2,
                    InUse = true
                },
                new ClassifiedAds.Domain.Shared.Currency
                {
                    CurrencyCode = "USD",
                    DecimalPlaces = 2,
                    InUse = true
                }
            };

        public ClassifiedAds.Domain.Shared.Currency FindCurrency(string currencyCode)
        {
            var currency = Currencies.FirstOrDefault(x => x.CurrencyCode == currencyCode);
            return currency ?? ClassifiedAds.Domain.Shared.Currency.None;
        }
    }
}