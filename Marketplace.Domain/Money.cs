using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Money : Value<Money>
    {
        private readonly double _amount;

        public Money(double amount) => _amount = amount;
        
        public Money Add(Money summand) => 
            new Money(_amount + summand._amount);
        
        public Money Substract(Money subtrahend) => 
            new Money(_amount - subtrahend._amount);
        
        public static Money operator +(Money summand1, Money summand2) =>
            summand1.Add(summand2);

        public static Money operator -(Money minuend, Money subtrahend) =>
            minuend.Substract(subtrahend);
    }
}