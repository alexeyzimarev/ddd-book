using System;
using Marketplace.EventSourcing;

namespace Marketplace.Ads.Domain.Shared
{
    public class UserId : Value<UserId>
    {
        public Guid Value { get; private set; }

        public UserId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "User id cannot be empty");
            
            Value = value;
        }
        
        public static implicit operator Guid(UserId self) => self.Value;
    }
}