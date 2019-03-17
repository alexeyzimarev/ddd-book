using Marketplace.Ads.Messages.UserProfile;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Infrastructure.EventStore.TypeMapper;

namespace Marketplace.Modules.ClassifiedAds
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<ClassifiedAdCreated>("ClassifiedAdCreated");
            Map<ClassifiedAdDeleted>("ClassifiedAdDeleted");
            Map<ClassifiedAdPublished>("ClassifiedAdPublished");
            Map<ClassifiedAdTextUpdated>("");
            Map<ClassifiedAdPriceUpdated>("");
            Map<ClassifiedAdTitleChanged>("");
            Map<ClassifiedAdPictureResized>("");
            Map<ClassifiedAdSentForReview>("");
            Map<PictureAddedToAClassifiedAd>("");

            // User profile
            Map<Events.UserRegistered>("");
        }
    }
}