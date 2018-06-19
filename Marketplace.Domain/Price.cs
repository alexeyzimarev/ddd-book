using System;

namespace Marketplace.Domain
{
    public class Price : Money
    {
        private Price(decimal amount) : base(amount)
        {
            if (amount <= 0)
                throw new ArgumentException(
                    "Price cannot be negative",
                    nameof(amount));
        }
    }
}