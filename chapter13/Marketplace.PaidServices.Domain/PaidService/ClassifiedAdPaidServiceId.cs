using System;
using Marketplace.Framework;

namespace Marketplace.PaidServices.Domain.PaidService
{
    public class ClassifiedAdPaidServiceId : Value<ClassifiedAdPaidServiceId>
    {
        public Guid Value { get; }
        
        public ClassifiedAdPaidServiceId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "Classified Ad Paid Service id cannot be empty");
            
            Value = value;
        }
        
        public static implicit operator Guid(ClassifiedAdPaidServiceId self) => self.Value;
        
        public static implicit operator ClassifiedAdPaidServiceId(string value) 
            => new ClassifiedAdPaidServiceId(Guid.Parse(value));

        public override string ToString() => Value.ToString();
    }
}