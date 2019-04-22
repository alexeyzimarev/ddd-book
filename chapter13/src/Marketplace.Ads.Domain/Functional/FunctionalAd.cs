using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using static Marketplace.Ads.Messages.Ads.Events;

namespace Marketplace.Ads.Domain.Functional
{
    public static class FunctionalAd
    {
        public static FunctionalAdState.Result Create(
            ClassifiedAdId id,
            UserId ownerId
        )
            => new FunctionalAdState().Apply(
                new V1.ClassifiedAdCreated
                {
                    Id = id,
                    OwnerId = ownerId
                }
            );

        public static FunctionalAdState.Result SetTitle(
            FunctionalAdState state,
            ClassifiedAdTitle title
        )
            => state.Apply(
                new V1.ClassifiedAdTitleChanged
                {
                    Id = state.Id,
                    OwnerId = state.OwnerId,
                    Title = title
                }
            );
    }
}