using System;

namespace Marketplace.Domain
{
    public class Price : Money
    {
        public Price(double amount) : base(amount)
        {
            if (amount <= 0)
                throw new ArgumentException(
                    "Price cannot be negative",
                    nameof(amount));
        }
    }
}