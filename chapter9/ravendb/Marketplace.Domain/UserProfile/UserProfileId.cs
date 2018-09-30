using System;
using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile
{
    public class UserProfileId : Value<UserProfileId>
    {
        public Guid Value { get; }

        public UserProfileId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "UserProfile id cannot be empty");
            
            Value = value;
        }

        public static implicit operator Guid(UserProfileId self) => self.Value;
        
        public static implicit operator UserProfileId(string value) 
            => new UserProfileId(Guid.Parse(value));

        public override string ToString() => Value.ToString();
    }
}