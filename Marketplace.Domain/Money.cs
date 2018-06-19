using System;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Money : Value<Money>
    {
        private const string DefaultCurrency = "EUR";

        public static Money FromDecimal(decimal amount, string currency = DefaultCurrency) =>
            new Money(amount, currency);

        public static Money FromString(string amount, string currency = DefaultCurrency) =>
            new Money(decimal.Parse(amount), currency);

        protected Money(decimal amount, string currencyCode = "EUR")
        {
            if (decimal.Round(amount, 2) != amount)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Amount cannot have more than two decimals");

            _amount = amount;
            _currencyCode = currencyCode;
        }

        protected readonly decimal _amount;
        protected readonly string _currencyCode;

        public Money Add(Money summand)
        {
            if (_currencyCode != summand._currencyCode)
                throw new CurrencyMismatchException(
                    "Cannot sum amounts with different currencies");

            return new Money(_amount + summand._amount);
        }

        public Money Substract(Money subtrahend)
        {
            if (_currencyCode != subtrahend._currencyCode)
                throw new CurrencyMismatchException(
                    "Cannot substract amounts with different currencies");

            return new Money(_amount - subtrahend._amount);
        }

        public static Money operator +(Money summand1, Money summand2) =>
            summand1.Add(summand2);

        public static Money operator -(Money minuend, Money subtrahend) =>
            minuend.Substract(subtrahend);
    }

    public class CurrencyMismatchException : Exception
    {
        public CurrencyMismatchException(string message) : base(message)
        {
        }
    }
}