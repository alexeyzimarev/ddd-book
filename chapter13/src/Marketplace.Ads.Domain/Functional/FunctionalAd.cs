using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.Ads;

namespace Marketplace.Ads.Domain.Functional
{
    public static class FunctionalAd
    {
        public static FunctionalAdState.Result Create(
            ClassifiedAdId id,
            UserId ownerId)
            => new FunctionalAdState().Apply(
                new Events.ClassifiedAdCreated
                {
                    Id = id,
                    OwnerId = ownerId
                }
            );

        public static FunctionalAdState.Result SetTitle(
            FunctionalAdState state,
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