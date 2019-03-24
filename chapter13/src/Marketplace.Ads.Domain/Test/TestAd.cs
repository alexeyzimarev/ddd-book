using System.Collections.Generic;
using System.Linq;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.Ads;

namespace Marketplace.Ads.Domain.Test
{
    public static class TestAd
    {
        public static (TestAdState state, IEnumerable<object> events) Create(
            ClassifiedAdId id,
            UserId ownerId)
            => Apply(
                new TestAdState(),
                new Events.ClassifiedAdCreated
                {
                    Id = id,
                    OwnerId = ownerId
                }
            );

        public static (TestAdState state, IEnumerable<object> events) SetTitle(
            TestAdState state,
            ClassifiedAdTitle title)
            => Apply(
                state, 
                new Events.ClassifiedAdTitleChanged
                {
                    Id = state.Id,
                    OwnerId = state.OwnerId,
                    Title = title
                }
            );

        static (TestAdState state, IEnumerable<object> events) Apply(
            TestAdState state,
            params object[] events)
            => (events.Aggregate(
                state, (current, @event) => current.Apply(@event)
            ), events);
    }
}