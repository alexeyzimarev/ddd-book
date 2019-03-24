using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.Ads;

namespace Marketplace.Ads.Domain.Test
{
    public static class TestAd
    {
        public static TestAdState.Result Create(
            ClassifiedAdId id,
            UserId ownerId)
            => new TestAdState().Apply(
                new Events.ClassifiedAdCreated
                {
                    Id = id,
                    OwnerId = ownerId
                }
            );

        public static TestAdState.Result SetTitle(
            TestAdState state,
            ClassifiedAdTitle title)
            => state.Apply(
                new Events.ClassifiedAdTitleChanged
                {
                    Id = state.Id,
                    OwnerId = state.OwnerId,
                    Title = title
                }
            );
    }
}