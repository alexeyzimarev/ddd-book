using System;
using Marketplace.EventSourcing;

namespace Marketplace.PaidServices.Domain.PaidService
{
    public class ClassifiedAdId : Value<ClassifiedAdId>
    {
        public Guid Value { get; }
        
        public ClassifiedAdId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "Classified Ad Paid Service id cannot be empty");
            
            Value = value;
        }
        
        public static implicit operator Guid(ClassifiedAdId self) => self.Value;
        
        public static implicit operator ClassifiedAdId(string value) 
            => new ClassifiedAdId(Guid.Parse(value));

        public override string ToString() => Value.ToString();
    }
}