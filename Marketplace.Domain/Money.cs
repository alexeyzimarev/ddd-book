using System;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Money : Value<Money>
    {
        public static string DefaultCurrency = "EUR";
        
        public static Money FromDecimal(decimal amount, string currency, 
            ICurrencyLookup currencyLookup) =>
            new Money(amount, currency, currencyLookup);

        public static Money FromString(string amount, string currency,
            ICurrencyLookup currencyLookup) =>
            new Money(decimal.Parse(amount), currency, currencyLookup);

        protected Money(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
        {
            if (string.IsNullOrEmpty(currencyCode))
                throw new ArgumentNullException(
                    nameof(currencyCode), "Currency code must be specified");
            
            var currency = currencyLookup.FindCurrency(currencyCode);
            if (!currency.InUse)
                throw new ArgumentException($"Currency {currencyCode} is not valid");
            
            if (decimal.Round(amount, currency.DecimalPlaces) != amount)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    $"Amount in {currencyCode} cannot have more than {currency.DecimalPlaces} decimals");

            _amount = amount;
            _currency = currency;
        }

        private Money(decimal amount, CurrencyDetails currency)
        {
            _amount = amount;
            _currency = currency;
        }

        protected readonly decimal _amount;
        protected readonly CurrencyDetails _currency;

        public Money Add(Money summand)
        {
            if (_currency != summand._currency)
                throw new CurrencyMismatchException(
                    "Cannot sum amounts with different currencies");

            return new Money(_amount + summand._amount, _currency);
        }

        public Money Subtract(Money subtrahend)
        {
            if (_currency != subtrahend._currency)
                throw new CurrencyMismatchException(
                    "Cannot subtract amounts with different currencies");

            return new Money(_amount - subtrahend._amount, _currency);
        }

        public static Money operator +(Money summand1, Money summand2) =>
            summand1.Add(summand2);

        public static Money operator -(Money minuend, Money subtrahend) =>
            minuend.Subtract(subtrahend);
    }

    public class CurrencyMismatchException : Exception
    {
        public CurrencyMismatchException(string message) : base(message)
        {
        }
    }
}