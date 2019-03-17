using System;
using Marketplace.EventSourcing;

namespace Marketplace.Ads.Domain.Shared
{
    public class UserId : AggregateId<UserProfiles.UserProfile>
    {
        public UserId(Guid value) : base(value) { }
    }
}