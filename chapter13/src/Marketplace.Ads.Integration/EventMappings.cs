using static Marketplace.Ads.Integration.ClassifiedAds.Events;
using static Marketplace.EventSourcing.TypeMapper;

namespace Marketplace.Ads.Integration
{
    internal static class EventMappings
    {
        public static void MapEventTypes()
            => Map<V1.AdPublished>("AdsIntegration.AdPublished");
    }
}
