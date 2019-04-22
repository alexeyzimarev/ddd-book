using System;
using Marketplace.EventSourcing;

namespace Marketplace.PaidServices.Domain.Orders
{
    public class OrderId : Value<OrderId>
    {
        protected OrderId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(
                    nameof(value), 
                    "The Id cannot be empty");
            
            Value = value;
        }
        
        public static implicit operator OrderId(string value)
            => new OrderId(Guid.Parse(value));

        public Guid Value { get; }
        
        public static implicit operator Guid(OrderId self) => self.Value;

        public override string ToString() => Value.ToString();
    }
}